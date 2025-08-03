use kadmium_dmx_shared::{Message, NeewerLightParams, NeewerUpdate};
use rumqttc::{AsyncClient, Event, MqttOptions, QoS};
use std::collections::HashMap;
use tokio::time::{Duration, interval};
use tracing::{info, warn};

/// Publishes the MIDI map configuration as a retained MQTT message
#[allow(dead_code)]
pub async fn test_neewer_light() -> Result<(), std::io::Error> {
    // Connect as MQTT client to publish the retained MIDI map
    let mqtt_options = MqttOptions::new("neewer-update-publisher", "localhost", 1883);
    let (mqtt_client, mut event_loop) = AsyncClient::new(mqtt_options, 10);

    // Create a timer that fires every 20ms
    let mut timer = interval(Duration::from_millis(20));

    // Example fixtures with cycling colors
    let mut hue_counter = 0u32;

    // Create reusable structures to avoid allocations
    let mut fixtures = HashMap::new();
    fixtures.insert(
        "CB:11:33:33:A3:67".to_string(),
        NeewerLightParams {
            hue: 0,
            saturation: 100,
            brightness: 100,
        },
    );

    let mut fixture_updates = NeewerUpdate { fixtures };

    // Run both tasks concurrently
    tokio::select! {
        // Handle MQTT event loop
        _ = async {
            loop {
                match event_loop.poll().await {
                    Ok(Event::Incoming(packet)) => {
                        info!("Received MQTT packet: {:?}", packet);
                    }
                    Ok(Event::Outgoing(packet)) => {
                        info!("Sent MQTT packet: {:?}", packet);
                    }
                    Err(e) => {
                        warn!("MQTT connection error: {:?}", e);
                        break;
                    }
                }
            }
        } => {},

        // Send NeewerUpdate packets every 20ms
        _ = async {
            loop {
                timer.tick().await;

                // Update the hue value in the existing fixture
                if let Some(params) = fixture_updates.fixtures.get_mut("CB:11:33:33:A3:67") {
                    params.hue = hue_counter % 360;
                }

                // Serialize the protobuf message
                let payload = fixture_updates.encode_to_vec();

                // Publish to bt/neewer/update topic
                if let Err(e) = mqtt_client.publish("bt/neewer/update", QoS::AtMostOnce, false, payload).await {
                    warn!("Failed to publish NeewerUpdate: {:?}", e);
                } else {
                    info!("Published NeewerUpdate with hue offset {}", hue_counter);
                }

                hue_counter = (hue_counter + 1) % 360;
            }
        } => {}
    }

    Ok(())
}
