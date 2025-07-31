use rumqttc::{AsyncClient, Event, MqttOptions, QoS};
use tracing::{info, warn};

/// Publishes the MIDI map configuration as a retained MQTT message
pub async fn publish_config() -> Result<(), std::io::Error> {
    // Connect as MQTT client to publish the retained MIDI map
    let mqtt_options = MqttOptions::new("webapi-publisher", "localhost", 1883);
    let (mqtt_client, mut event_loop) = AsyncClient::new(mqtt_options, 10);

    // Read and publish the MIDI map as retained message
    let midi_map_json = tokio::fs::read("config/midi_map.json")
        .await
        .map_err(std::io::Error::other)?;

    info!("Publishing MIDI map to MQTT broker with retain=true");
    send_and_wait_for_ack(
        &mqtt_client,
        &mut event_loop,
        "config/midi_map",
        midi_map_json,
    )
    .await?;

    let venue_json = tokio::fs::read("config/venue.json")
        .await
        .map_err(std::io::Error::other)?;

    info!("Publishing venue configuration to MQTT broker with retain=true");
    send_and_wait_for_ack(&mqtt_client, &mut event_loop, "config/venue", venue_json).await?;

    info!("Config publish process completed");
    Ok(())
}

async fn send_and_wait_for_ack(
    mqtt_client: &AsyncClient,
    event_loop: &mut rumqttc::EventLoop,
    topic: &str,
    payload: Vec<u8>,
) -> Result<(), std::io::Error> {
    // Publish the message with retain flag
    mqtt_client
        .publish(topic, QoS::AtLeastOnce, true, payload)
        .await
        .map_err(std::io::Error::other)?;

    // Wait for acknowledgment
    let mut ack_received = false;
    for _ in 0..10 {
        match event_loop.poll().await {
            Ok(event) => {
                info!("MQTT event: {:?}", event);
                match event {
                    Event::Outgoing(outgoing) => {
                        info!("Outgoing packet: {:?}", outgoing);
                    }
                    Event::Incoming(incoming) => {
                        info!("Incoming packet: {:?}", incoming);
                        // Check if this is a PubAck by matching the packet type
                        if let rumqttc::Incoming::PubAck(_) = incoming {
                            info!("Received publish acknowledgment");
                            ack_received = true;
                            break;
                        }
                    }
                }
            }
            Err(e) => {
                warn!("MQTT eventloop error during publish: {e}");
                break;
            }
        }
    }

    if !ack_received {
        return Err(std::io::Error::new(
            std::io::ErrorKind::TimedOut,
            "Did not receive publish acknowledgment within timeout",
        ));
    }

    Ok(())
}
