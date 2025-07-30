use rumqttc::{Client, MqttOptions, QoS};
use tracing::{error, info, warn};

/// Publishes the MIDI map configuration as a retained MQTT message
pub async fn publish_midi_map() -> Result<(), Box<dyn std::error::Error + Send + Sync>> {
    // Connect as MQTT client to publish the retained MIDI map
    let mqtt_options = MqttOptions::new("webapi-publisher", "localhost", 1883);
    let (mqtt_client, mut connection) = Client::new(mqtt_options, 10);

    // Read and publish the MIDI map as retained message
    let midi_map_json = std::fs::read("config/midi_map.json")
        .map_err(|e| format!("Failed to read MIDI map configuration file: {e}"))?;

    info!("MIDI map JSON size: {} bytes", midi_map_json.len());

    info!("Publishing MIDI map to MQTT broker with retain=true");
    if let Err(e) = mqtt_client.publish("config/midi_map", QoS::AtLeastOnce, true, midi_map_json) {
        error!("Failed to publish MIDI map: {e}");
        return Err(Box::new(e));
    }

    // Process MQTT events to ensure publish completes
    for _ in 0..5 {
        match connection.eventloop.poll().await {
            Ok(event) => {
                info!("MQTT event: {:?}", event);
            }
            Err(e) => {
                warn!("MQTT eventloop error during publish: {e}");
                break;
            }
        }
    }

    info!("MIDI map publish process completed");
    Ok(())
}
