use btleplug::api::{Central, Manager as _, Peripheral as _, ScanFilter, WriteType};
use btleplug::platform::{Manager, Peripheral};
use std::time::Duration;
use tokio::sync::watch;
use tokio::time::sleep;
use tracing::{debug, error, info, warn};
use uuid::Uuid;

use kadmium_dmx_shared::NeewerLightParams;

use crate::color::NeewerColor;

const WRITE_CHARACTERISTIC_UUID: Uuid = Uuid::from_u128(0x69400002_B5A3_F393_E0A9_E50E24DCCA99);

/// Represents a Neewer light fixture that we can control via Bluetooth
#[derive(Debug, Clone)]
pub struct NeweerFixture {
    pub bluetooth_address: String,
    pub update_tx: watch::Sender<NeewerLightParams>,
}

impl NeweerFixture {
    pub async fn from_address(address: &str) -> Option<Self> {
        let (update_tx, update_rx) = watch::channel(NeewerLightParams::default());

        let fixture = Self {
            bluetooth_address: address.to_string(),
            update_tx,
        };

        // Spawn the Bluetooth management task
        Self::spawn_bluetooth_task(address.to_string(), update_rx).await;

        Some(fixture)
    }

    /// Update the fixture color
    pub async fn update_color(&self, params: NeewerLightParams) {
        if self.update_tx.send(params).is_err() {
            error!("Failed to send color update - receiver dropped");
        }
    }

    /// Spawn the Bluetooth management task
    async fn spawn_bluetooth_task(
        bluetooth_address: String,
        mut update_rx: watch::Receiver<NeewerLightParams>,
    ) {
        tokio::spawn(async move {
            info!("Starting Bluetooth task for device: {}", bluetooth_address);

            let mut peripheral: Option<Peripheral> = None;

            loop {
                while peripheral.is_none()
                    || !peripheral.clone().unwrap().is_connected().await.unwrap()
                {
                    peripheral = Self::try_connect(&bluetooth_address).await;
                    if peripheral.is_none() {
                        warn!("Retrying connection in 2 seconds...");
                        sleep(Duration::from_secs(2)).await;
                    }
                }

                tokio::select! {
                    // Handle color updates - send immediately if connected
                    result = update_rx.changed() => {
                        if result.is_ok() {
                            let params = *update_rx.borrow_and_update();

                            debug!("Processing latest color update for {}: H:{} S:{} B:{}",
                                bluetooth_address, params.hue, params.saturation, params.brightness);

                            // Send immediately if connected
                            if let Some(ref device) = peripheral {
                                let color = NeewerColor::from(params);
                                if let Err(e) = Self::send_color_to_device(device, &color).await {
                                    error!("Failed to send color to device {}: {}", bluetooth_address, e);
                                    // Reset connection on failure
                                    peripheral = None;
                                } else {
                                    info!("Sent immediate color update to device: {}", bluetooth_address);
                                }
                            }

                        } else {
                            warn!("Update channel closed for device: {}", bluetooth_address);
                            break;
                        }
                        sleep(Duration::from_millis(1)).await;
                    }

                    // Reduced frequency periodic check for connection and force updates
                    _ = sleep(Duration::from_millis(100)) => {}
                }
            }

            info!("Bluetooth task ended for device: {}", bluetooth_address);
        });
    }

    /// Try to connect to a Bluetooth device
    async fn try_connect(bluetooth_address: &str) -> Option<Peripheral> {
        info!("Attempting to connect to device: {}", bluetooth_address);

        // Get the Bluetooth manager
        let manager = match Manager::new().await {
            Ok(m) => m,
            Err(e) => {
                error!("Failed to create Bluetooth manager: {}", e);
                return None;
            }
        };

        // Get the first Bluetooth adapter
        let adapters = match manager.adapters().await {
            Ok(adapters) => adapters,
            Err(e) => {
                error!("Failed to get Bluetooth adapters: {}", e);
                return None;
            }
        };

        let central = match adapters.into_iter().next() {
            Some(adapter) => adapter,
            None => {
                error!("No Bluetooth adapters found");
                return None;
            }
        };

        // Start scanning for devices
        if let Err(e) = central.start_scan(ScanFilter::default()).await {
            error!("Failed to start Bluetooth scan: {}", e);
            return None;
        }

        // Wait a bit for devices to be discovered
        tokio::time::sleep(Duration::from_secs(3)).await;

        // Find the target device by name or address
        let peripherals = match central.peripherals().await {
            Ok(peripherals) => peripherals,
            Err(e) => {
                error!("Failed to get peripherals: {}", e);
                return None;
            }
        };

        info!("Found {} Bluetooth devices during scan", peripherals.len());

        for peripheral in peripherals {
            if let Ok(Some(properties)) = peripheral.properties().await {
                debug!(
                    "Discovered device: {} ({})",
                    properties.local_name.as_deref().unwrap_or("Unknown"),
                    properties.address
                );

                if properties.address.to_string() == bluetooth_address {
                    info!(
                        "Found target device: {} ({})",
                        properties.local_name.as_deref().unwrap_or("Unknown"),
                        properties.address
                    );

                    // Try to connect
                    match peripheral.connect().await {
                        Ok(_) => {
                            info!("Connected to device: {}", bluetooth_address);

                            // Discover services
                            if let Err(e) = peripheral.discover_services().await {
                                error!(
                                    "Failed to discover services for {}: {}",
                                    bluetooth_address, e
                                );
                                let _ = peripheral.disconnect().await;
                                return None;
                            }

                            return Some(peripheral);
                        }
                        Err(e) => {
                            error!("Failed to connect to device {}: {}", bluetooth_address, e);
                        }
                    }
                }
            }
        }

        warn!("Device not found: {}", bluetooth_address);
        None
    }

    /// Send color data to the Bluetooth device
    async fn send_color_to_device(
        device: &Peripheral,
        color: &NeewerColor,
    ) -> Result<(), Box<dyn std::error::Error + Send + Sync>> {
        info!(
            "Sending color to device: H:{} S:{} B:{}",
            color.hue, color.saturation, color.brightness
        );

        // Find the write characteristic
        let characteristics = device.characteristics();
        let write_char = characteristics
            .iter()
            .find(|c| c.uuid == WRITE_CHARACTERISTIC_UUID)
            .ok_or("Write characteristic not found")?;

        let [hue_lsb, hue_msb] = color.hue.to_le_bytes();
        let mut color_cmd = [
            0x78,
            0x86,
            0x04,
            hue_lsb,
            hue_msb,
            color.saturation,
            color.brightness,
            0,
        ];
        let last_index = color_cmd.len() - 1;
        color_cmd[last_index] = Self::get_checksum(&color_cmd[0..last_index]);

        info!("Sending HSV command: {:02X?}", color_cmd);

        // Write the command to the device
        device
            .write(write_char, &color_cmd, WriteType::WithoutResponse)
            .await?;

        info!("Successfully sent color command to device");
        Ok(())
    }

    fn get_checksum(values: &[u8]) -> u8 {
        let mut checksum: u8 = 0;
        for value in values {
            checksum = checksum.wrapping_add(*value);
        }
        checksum
    }
}
