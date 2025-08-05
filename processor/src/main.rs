mod effects;
mod fixtures;
mod macros;
mod mqtt_manager;
mod universe_manager;
mod universes;

use std::env;

use anyhow::Result;
use tokio::sync::mpsc;
use tracing::{error, info};

use mqtt_manager::{MqttManager, MqttMessage};
use universe_manager::UniverseManager;

use crate::universes::universe::Universe;

#[tokio::main]
async fn main() -> Result<()> {
    // Initialize logging
    tracing_subscriber::fmt::init();

    info!("Starting DMX Processor...");

    // Get MQTT broker configuration from environment variables
    let mqtt_host = env::var("MQTT_HOST").unwrap_or_else(|_| "localhost".to_string());
    let mqtt_port = env::var("MQTT_PORT").unwrap_or_else(|_| "1883".to_string()).parse::<u16>().unwrap_or(1883);
    let client_id = env::var("MQTT_CLIENT_ID").unwrap_or_else(|_| "dmx-processor".to_string());

    info!("Connecting to MQTT broker at {}:{}", mqtt_host, mqtt_port);

    // Create message channel
    let (message_sender, mut message_receiver) = mpsc::unbounded_channel::<MqttMessage>();

    // Create and start MQTT manager
    let mut mqtt_manager = MqttManager::new(&mqtt_host, mqtt_port, &client_id, message_sender)?;
    mqtt_manager.start().await?;

    info!("DMX Processor started successfully");

    // Clone the MQTT manager for the update task
    let mqtt_client = mqtt_manager.get_client().clone();

    let mut universe_manager = UniverseManager::new();

    // Main update loop at 40 Hz (25ms interval)
    let mut interval = tokio::time::interval(tokio::time::Duration::from_millis(25));

    loop {
        interval.tick().await;

        // Process MQTT messages non-blockingly
        while let Ok(message) = message_receiver.try_recv() {
            match message {
                MqttMessage::Venue(venue_update) => {
                    if let Err(e) = universe_manager.update_venue(venue_update.venue, venue_update.definitions) {
                        error!("Failed to update venue configuration: {}", e);
                    }
                }
                MqttMessage::MidiMap(midi_map) => {
                    if let Err(e) = universe_manager.update_midi_map(midi_map) {
                        error!("Failed to update MIDI map configuration: {}", e);
                    }
                }
                MqttMessage::GroupAttribute { group_name, attribute, value } => {
                    if let Err(e) = universe_manager.update_group_attribute(&group_name, &attribute, value) {
                        error!("Failed to update group attribute: {}", e);
                    }
                }
            }
        }

        // Render and publish universe updates - DMX universes
        for (_, dmx_universe) in universe_manager.dmx_universes_iter_mut() {
            if let Err(e) = dmx_universe.update() {
                error!("Failed to update DMX universe: {}", e);
                continue;
            }

            if let Err(e) = dmx_universe.render() {
                error!("Failed to render DMX universe: {}", e);
                continue;
            }

            if let Err(e) = dmx_universe.send_update(&mqtt_client).await {
                error!("Failed to send DMX universe update: {}", e);
                continue;
            }
        }

        // Render and publish universe updates - Neewer universes
        for (_, neewer_universe) in universe_manager.neewer_universes_iter_mut() {
            if let Err(e) = neewer_universe.update() {
                error!("Failed to update Neewer universe: {}", e);
                continue;
            }

            if let Err(e) = neewer_universe.render() {
                error!("Failed to render Neewer universe: {}", e);
                continue;
            }

            if let Err(e) = neewer_universe.send_update(&mqtt_client).await {
                error!("Failed to send Neewer universe update: {}", e);
                continue;
            }
        }
    }
}
