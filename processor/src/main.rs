mod effects;
mod fixtures;
mod macros;
mod mqtt_manager;
mod universe_manager;
mod universes;

use std::env;

use anyhow::Result;
use bytes::BytesMut;
use tokio::sync::mpsc;
use tracing::{error, info};

use mqtt_manager::{MqttManager, MqttMessage};
use universe_manager::UniverseManager;

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
                MqttMessage::VenueUpdate(venue_update) => {
                    if let Err(e) = universe_manager.update_venue(venue_update.venue, &venue_update.definitions) {
                        error!("Failed to update venue configuration: {}", e);
                    }
                }
                MqttMessage::MidiMapUpdate(midi_map) => {
                    if let Err(e) = universe_manager.update_midi_map(midi_map) {
                        error!("Failed to update MIDI map configuration: {}", e);
                    }
                }
                MqttMessage::GroupAttributeUpdate { group_name, attribute, value } => {
                    if let Err(e) = universe_manager.update_group_attribute(&group_name, &attribute, value) {
                        error!("Failed to update group attribute: {}", e);
                    }
                }
            }
        }

        // Render and publish universe updates
        for (_, universe_container) in universe_manager.universes_iter_mut() {
            let mut send_payload = BytesMut::with_capacity(512); // Placeholder payload, adjust as needed

            if let Err(e) = universe_container.update() {
                error!("Failed to update universe: {}", e);
                continue;
            }

            if let Err(e) = universe_container.render() {
                error!("Failed to render universe: {}", e);
                continue;
            }

            let topic = match universe_container.get_update(&mut send_payload) {
                Ok(topic) => topic,
                Err(e) => {
                    error!("Failed to get update for universe: {}", e);
                    continue;
                }
            };

            if let Err(e) = mqtt_client
                .publish(format!("{topic}/update"), rumqttc::QoS::AtLeastOnce, false, send_payload.freeze())
                .await
            {
                error!("Failed to publish universe update for {topic}: {}", e);
            }
        }
    }
}
