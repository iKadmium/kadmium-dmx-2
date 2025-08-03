mod effects;
mod fixture_manager;
mod fixtures;
mod mqtt_manager;
mod universes;

use std::{
    collections::HashMap,
    env,
    sync::{Arc, Mutex},
};

use anyhow::Result;
use bytes::BytesMut;
use tokio::sync::mpsc;
use tracing::{error, info};

use fixture_manager::FixtureManager;
use mqtt_manager::{MqttManager, MqttMessage};

#[tokio::main]
async fn main() -> Result<()> {
    // Initialize logging
    tracing_subscriber::fmt::init();

    info!("Starting DMX Processor...");

    // Get MQTT broker configuration from environment variables
    let mqtt_host = env::var("MQTT_HOST").unwrap_or_else(|_| "localhost".to_string());
    let mqtt_port = env::var("MQTT_PORT")
        .unwrap_or_else(|_| "1883".to_string())
        .parse::<u16>()
        .unwrap_or(1883);
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

    // Spawn background task for MQTT message processing
    let fixture_manager = Arc::new(Mutex::new(FixtureManager::new()));
    let universes = Arc::new(Mutex::new(HashMap::new()));

    let fm_for_mqtt = fixture_manager.clone();
    let univ_for_mqtt = universes.clone();

    tokio::spawn(async move {
        while let Some(message) = message_receiver.recv().await {
            match message {
                MqttMessage::VenueUpdate(venue_update) => {
                    if let (Ok(mut fm), Ok(mut universes)) =
                        (fm_for_mqtt.try_lock(), univ_for_mqtt.try_lock())
                    {
                        if let Err(e) = fm.update_venue(
                            venue_update.venue,
                            &venue_update.definitions,
                            &mut universes,
                        ) {
                            error!("Failed to update venue configuration: {}", e);
                        }
                    }
                }
                MqttMessage::GroupAttributeUpdate {
                    group_name,
                    attribute,
                    value,
                } => {
                    if let Ok(mut fm) = fm_for_mqtt.try_lock() {
                        if let Err(e) = fm.update_group_attribute(&group_name, &attribute, value) {
                            error!("Failed to update group attribute: {}", e);
                        }
                    }
                }
            }
        }
        error!("MQTT message channel closed, background task exiting...");
    });

    // Main update loop at 40 Hz (25ms interval)
    let mut interval = tokio::time::interval(tokio::time::Duration::from_millis(25));

    loop {
        interval.tick().await;

        for (_, universe_container) in universes.lock().unwrap().iter_mut() {
            let mut send_payload = BytesMut::with_capacity(512); // Placeholder payload, adjust as needed

            if let Err(e) = universe_container.render() {
                error!("Failed to render universe: {}", e);
            }

            let topic = match universe_container.get_update(&mut send_payload) {
                Ok(topic) => topic,
                Err(e) => {
                    error!("Failed to get update for universe: {}", e);
                    continue;
                }
            };

            if let Err(e) = mqtt_client
                .publish(
                    format!("{topic}/update"),
                    rumqttc::QoS::AtLeastOnce,
                    false,
                    send_payload.freeze(),
                )
                .await
            {
                error!("Failed to publish universe update for {topic}: {}", e);
            }
        }
    }
}
