use kadmium_dmx_shared::MidiMap;
use midi_types::MidiMessage;
use rtpmidi::sessions::{
    events::event_handling::MidiMessageEvent, invite_responder::InviteResponder,
    rtp_midi_session::RtpMidiSession,
};
use rumqttc::{AsyncClient, Event, Packet, QoS};
use std::sync::Arc;
use std::time::Duration;
use tokio::sync::RwLock;
use tracing::{debug, error, info, trace, warn};

#[tokio::main]
async fn main() {
    // Initialize tracing subscriber
    tracing_subscriber::fmt()
        .with_env_filter(
            tracing_subscriber::EnvFilter::try_from_default_env()
                .unwrap_or_else(|_| tracing_subscriber::EnvFilter::new("info")),
        )
        .init();

    // Shared MIDI map
    let midi_map = Arc::new(RwLock::new(MidiMap::new()));

    // Start RTP MIDI session
    let session = RtpMidiSession::start(5004, "kadmium-dmx", 123456_u32, InviteResponder::Accept)
        .await
        .expect("Failed to start RtpMidiSession");

    // Shared MQTT client for MIDI messages - will be updated on each reconnection
    let mqtt_client_for_midi = Arc::new(RwLock::new(None::<AsyncClient>));

    // Add MIDI message listener ONCE - outside the reconnection loop
    let midi_map_for_listener = midi_map.clone();
    let mqtt_client_clone = mqtt_client_for_midi.clone();
    session
        .add_listener(MidiMessageEvent, move |(message, _timestamp)| {
            let midi_map = midi_map_for_listener.clone();
            let mqtt_client_ref = mqtt_client_clone.clone();

            tokio::spawn(async move {
                // Only handle MIDI if we have an active MQTT client
                if let Some(mqtt_client) = mqtt_client_ref.read().await.as_ref() {
                    handle_midi_message(message, midi_map, mqtt_client.clone()).await;
                }
            });
        })
        .await;

    // Main loop with connection handling
    loop {
        match run_mqtt_client(midi_map.clone(), mqtt_client_for_midi.clone()).await {
            Ok(()) => {
                info!("MQTT client exited normally");
                break;
            }
            Err(e) => {
                error!("MQTT connection error: {e}");
                // Clear the MQTT client so MIDI messages aren't processed during downtime
                *mqtt_client_for_midi.write().await = None;
                warn!("Reconnecting in 5 seconds...");
                tokio::time::sleep(Duration::from_secs(5)).await;
            }
        }
    }
}

async fn run_mqtt_client(
    midi_map: Arc<RwLock<MidiMap>>,
    mqtt_client_for_midi: Arc<RwLock<Option<AsyncClient>>>,
) -> Result<(), Box<dyn std::error::Error + Send + Sync>> {
    let mqtt_options = rumqttc::MqttOptions::new("kadmium-dmx", "localhost", 1883);
    let (mqtt_client, mut eventloop) = AsyncClient::new(mqtt_options, 100);

    // Subscribe to the config topic
    if let Err(e) = mqtt_client
        .subscribe("config/midi_map", QoS::AtMostOnce)
        .await
    {
        error!("Failed to subscribe to config/midi_map: {e}");
    }

    // Store the MQTT client so the MIDI listener can use it
    *mqtt_client_for_midi.write().await = Some(mqtt_client.clone());

    // Handle MQTT events and Ctrl+C signal
    loop {
        tokio::select! {
            event_result = eventloop.poll() => {
                match event_result {
                    Ok(Event::Incoming(Packet::Publish(publish))) => {
                        if publish.topic == "config/midi_map" {
                            if let Ok(json_str) = std::str::from_utf8(&publish.payload) {
                                match serde_json::from_str::<MidiMap>(json_str) {
                                    Ok(new_midi_map) => {
                                        let mut map = midi_map.write().await;
                                        *map = new_midi_map;
                                        info!("Updated MIDI map from MQTT");
                                    }
                                    Err(e) => {
                                        error!("Failed to deserialize MIDI map: {e}");
                                    }
                                }
                            }
                        }
                    }
                    Ok(_) => {}
                    Err(e) => {
                        return Err(Box::new(e));
                    }
                }
            }
            _ = tokio::signal::ctrl_c() => {
                info!("Received Ctrl+C, shutting down gracefully");
                return Ok(());
            }
        }
    }
}

async fn handle_midi_message(
    event: MidiMessage,
    midi_map: Arc<RwLock<MidiMap>>,
    mqtt_client: AsyncClient,
) {
    trace!("Received MIDI message: {:?}", event);

    // Extract MIDI data - assuming it's a control change message
    if let MidiMessage::ControlChange(channel, cc, value) = event {
        // Check if it's a Control Change message (status byte 0xB0-0xBF)

        let map = midi_map.read().await;

        if let (Some(group), Some(attribute)) =
            (map.get_group(channel.into()), map.get_attribute(cc.into()))
        {
            // Normalize value from 0-127 to 0.0-1.0
            let int_value: u8 = value.into();
            let normalized_value = int_value as f32 / 127.0;

            let topic = format!("groups/{group}/{attribute}");
            let payload = normalized_value.to_be_bytes();

            if let Err(e) = mqtt_client
                .publish(topic.clone(), QoS::AtLeastOnce, true, payload)
                .await
            {
                error!("Failed to publish MQTT message to {topic}: {e}");
            } else {
                trace!("Published {normalized_value} to {topic}");
            }
        }
    }
}
