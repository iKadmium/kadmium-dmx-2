use anyhow::Result;
use btleplug::api::{Central, Manager as _, Peripheral as _, ScanFilter};
use btleplug::platform::Manager;
use kadmium_dmx_shared::venue_fixture::VenueFixtureType;
use rumqttc::{AsyncClient, Event, MqttOptions, Packet, QoS};
use std::time::Duration;
use tokio::sync::mpsc;
use tokio::time;
use tracing::{debug, error, info, warn};

use crate::fixture_manager::FixtureManagerCommand;
use crate::neewer_fixture::NeweerFixture;
use kadmium_dmx_shared::{
    FixtureAddress, Message, NeewerScanItem, NeewerScanResult, NeewerUpdate, ScanRequest,
    VenueUpdate,
};

/// MQTT manager that listens for venue configuration updates and fixture control messages
#[allow(dead_code)]
pub struct MqttManager {
    client: AsyncClient,
    fixture_manager_tx: mpsc::Sender<FixtureManagerCommand>,
}

impl MqttManager {
    /// Create a new MQTT manager
    pub async fn new(
        mqtt_host: &str,
        mqtt_port: u16,
        fixture_manager_tx: mpsc::Sender<FixtureManagerCommand>,
    ) -> Result<Self> {
        let mut mqtt_options = MqttOptions::new("neewer-renderer", mqtt_host, mqtt_port);
        mqtt_options.set_keep_alive(std::time::Duration::from_secs(60));

        let (client, mut eventloop) = AsyncClient::new(mqtt_options, 10);

        let manager = Self {
            client: client.clone(),
            fixture_manager_tx: fixture_manager_tx.clone(),
        };

        // Spawn task to handle MQTT events
        let fixture_manager_tx_clone = fixture_manager_tx.clone();
        let client_clone = client.clone();
        tokio::spawn(async move {
            loop {
                match eventloop.poll().await {
                    Ok(event) => {
                        if let Err(e) =
                            Self::handle_mqtt_event(event, &fixture_manager_tx_clone, &client_clone)
                                .await
                        {
                            error!("Error handling MQTT event: {}", e);
                        }
                    }
                    Err(e) => {
                        error!("MQTT connection error: {}", e);
                        tokio::time::sleep(std::time::Duration::from_secs(5)).await;
                    }
                }
            }
        });

        Ok(manager)
    }

    /// Handle incoming MQTT events
    async fn handle_mqtt_event(
        event: Event,
        fixture_manager_tx: &mpsc::Sender<FixtureManagerCommand>,
        mqtt_client: &AsyncClient,
    ) -> Result<()> {
        match event {
            Event::Incoming(Packet::Publish(publish)) => {
                if publish.topic == "config/venue" {
                    // Handle venue configuration updates
                    match Self::parse_venue_update(&publish.payload).await {
                        Ok(neewer_fixtures) => {
                            info!(
                                "Received venue update with {} Neewer fixtures",
                                neewer_fixtures.len()
                            );

                            // Send venue update to FixtureManager
                            if let Err(e) = fixture_manager_tx
                                .send(FixtureManagerCommand::UpdateVenue(neewer_fixtures.clone()))
                                .await
                            {
                                error!("Failed to send venue update to FixtureManager: {}", e);
                            }
                        }
                        Err(e) => {
                            error!("Failed to parse venue update: {}", e);
                        }
                    }
                } else if publish.topic.starts_with("bt/neewer/update") {
                    // Handle Neewer fixture updates
                    match Self::parse_neewer_update(&publish.payload) {
                        Ok(neewer_update) => {
                            let fixture_count = neewer_update.fixtures.len();
                            debug!("Received Neewer update for {} fixtures", fixture_count);

                            // Send fixture update to FixtureManager
                            if let Err(e) = fixture_manager_tx
                                .send(FixtureManagerCommand::UpdateFixture(neewer_update))
                                .await
                            {
                                error!("Failed to send fixture update to FixtureManager: {}", e);
                            }
                        }
                        Err(e) => {
                            error!("Failed to parse Neewer update: {}", e);
                        }
                    }
                } else if publish.topic == "bt/neewer/scan" {
                    // Handle Bluetooth scan requests
                    match Self::parse_scan_request(&publish.payload) {
                        Ok(_scan_request) => {
                            info!("Received Bluetooth scan request");

                            // Perform Bluetooth scan
                            match Self::perform_bluetooth_scan().await {
                                Ok(scan_result) => {
                                    // Publish scan results
                                    if let Err(e) =
                                        Self::publish_scan_result(mqtt_client, scan_result).await
                                    {
                                        error!("Failed to publish scan result: {}", e);
                                    }
                                }
                                Err(e) => {
                                    error!("Failed to perform Bluetooth scan: {}", e);
                                }
                            }
                        }
                        Err(e) => {
                            error!("Failed to parse scan request: {}", e);
                        }
                    }
                } else {
                    warn!("Received message on unexpected topic: {}", publish.topic);
                }
            }
            Event::Incoming(Packet::ConnAck(_)) => {
                info!("Connected to MQTT broker");
                // Subscribe to config/venue topic initially
                mqtt_client
                    .subscribe("config/venue", QoS::AtLeastOnce)
                    .await?;

                // Subscribe to neewer fixture updates topic
                mqtt_client
                    .subscribe("bt/neewer/update", QoS::AtLeastOnce)
                    .await?;

                // Subscribe to neewer scan requests topic
                mqtt_client
                    .subscribe("bt/neewer/scan", QoS::AtLeastOnce)
                    .await?;
            }
            Event::Incoming(Packet::SubAck(_)) => {
                info!("Successfully subscribed to topic");
            }
            _ => {}
        }
        Ok(())
    }

    /// Parse venue update JSON and extract Neewer fixtures
    async fn parse_venue_update(payload: &[u8]) -> Result<Vec<NeweerFixture>> {
        let json_str = std::str::from_utf8(payload)?;
        let venue_update: VenueUpdate = serde_json::from_str(json_str)?;

        let mut neewer_fixtures = Vec::new();

        let update_fixtures = venue_update.venue.fixtures.iter();
        let update_neewer_fixtures = update_fixtures
            .filter(|fixture| matches!(&fixture.fixture_type, VenueFixtureType::Neewer));

        // Extract Neewer fixtures from the venue
        for fixture in update_neewer_fixtures {
            if let FixtureAddress::Bluetooth { uuid } = &fixture.common.address {
                if let Some(neewer_fixture) = NeweerFixture::from_address(uuid).await {
                    neewer_fixtures.push(neewer_fixture);
                }
            }
        }

        Ok(neewer_fixtures)
    }

    /// Parse Neewer update protobuf
    fn parse_neewer_update(payload: &[u8]) -> Result<NeewerUpdate> {
        let update = NeewerUpdate::decode(payload)?;
        Ok(update)
    }

    /// Parse scan request JSON
    fn parse_scan_request(payload: &[u8]) -> Result<ScanRequest> {
        let json_str = std::str::from_utf8(payload)?;
        let scan_request: ScanRequest = serde_json::from_str(json_str)?;
        Ok(scan_request)
    }

    /// Perform Bluetooth scan for Neewer devices
    async fn perform_bluetooth_scan() -> Result<NeewerScanResult> {
        info!("Starting Bluetooth scan for Neewer devices");

        // Get the Bluetooth manager
        let manager = Manager::new().await?;

        // Get the first Bluetooth adapter
        let adapters = manager.adapters().await?;
        let central = match adapters.into_iter().next() {
            Some(adapter) => adapter,
            None => {
                return Err(anyhow::anyhow!("No Bluetooth adapters found"));
            }
        };

        // Start scanning for devices
        central.start_scan(ScanFilter::default()).await?;

        // Wait for devices to be discovered
        time::sleep(Duration::from_secs(5)).await;

        // Stop scanning
        central.stop_scan().await?;

        // Find Neewer devices
        let peripherals = central.peripherals().await?;
        let mut neewer_devices = Vec::new();

        info!("Found {} Bluetooth devices during scan", peripherals.len());

        for peripheral in peripherals {
            if let Ok(Some(properties)) = peripheral.properties().await {
                if let Some(local_name) = &properties.local_name {
                    // Check if this is a Neewer device (look for "LEDBlue" or "Neewer" in the name)
                    if local_name.starts_with("NEEWER") {
                        let device_info = NeewerScanItem {
                            address: properties.address.to_string(),
                            local_name: local_name.clone(),
                        };
                        info!("Found Neewer device: {device_info:?}");
                        neewer_devices.push(device_info);
                    }
                }
            }
        }

        let scan_result = NeewerScanResult {
            timestamp: std::time::SystemTime::now()
                .duration_since(std::time::UNIX_EPOCH)
                .unwrap()
                .as_secs(),
            devices: neewer_devices,
        };

        info!(
            "Bluetooth scan completed, found {} Neewer devices",
            scan_result.devices.len()
        );
        Ok(scan_result)
    }

    /// Publish scan results to MQTT
    async fn publish_scan_result(
        mqtt_client: &AsyncClient,
        scan_result: NeewerScanResult,
    ) -> Result<()> {
        let payload = serde_json::to_vec(&scan_result)?;

        mqtt_client
            .publish("bt/neewer/scan_result", QoS::AtLeastOnce, false, payload)
            .await?;

        info!(
            "Published scan result with {} devices",
            scan_result.devices.len()
        );
        Ok(())
    }
}
