use std::time::Duration;

use anyhow::Result;
use rumqttc::{AsyncClient, Event, MqttOptions, Packet, QoS};
use tokio::sync::mpsc;
use tracing::{error, info, warn};

use kadmium_dmx_shared::VenueUpdate;

#[derive(Debug, Clone)]
pub enum MqttMessage {
    VenueUpdate(VenueUpdate),
    GroupAttributeUpdate {
        group_name: String,
        attribute: String,
        value: f32,
    },
}

pub struct MqttManager {
    client: AsyncClient,
}

impl MqttManager {
    pub fn new(
        broker_host: &str,
        broker_port: u16,
        client_id: &str,
        message_sender: mpsc::UnboundedSender<MqttMessage>,
    ) -> Result<Self> {
        let mut mqttoptions = MqttOptions::new(client_id, broker_host, broker_port);
        mqttoptions.set_keep_alive(Duration::from_secs(60));

        let (client, mut eventloop) = AsyncClient::new(mqttoptions, 10);

        // Spawn task to handle MQTT events
        let sender_clone = message_sender.clone();
        tokio::spawn(async move {
            loop {
                match eventloop.poll().await {
                    Ok(Event::Incoming(Packet::Publish(publish))) => {
                        if let Err(e) = Self::handle_mqtt_message(
                            &publish.topic,
                            &publish.payload,
                            &sender_clone,
                        ) {
                            error!("Failed to handle MQTT message: {}", e);
                        }
                    }
                    Ok(_) => {}
                    Err(e) => {
                        error!("MQTT connection error: {}", e);
                        tokio::time::sleep(Duration::from_secs(5)).await;
                    }
                }
            }
        });

        Ok(Self { client })
    }

    pub async fn start(&mut self) -> Result<()> {
        info!("Starting MQTT manager...");

        // Subscribe to venue configuration updates
        self.client
            .subscribe("config/venue", QoS::AtLeastOnce)
            .await?;

        // Subscribe to group attribute updates
        self.client
            .subscribe("groups/+/+", QoS::AtLeastOnce)
            .await?;

        info!("MQTT manager started successfully");
        Ok(())
    }

    fn handle_mqtt_message(
        topic: &str,
        payload: &[u8],
        sender: &mpsc::UnboundedSender<MqttMessage>,
    ) -> Result<()> {
        info!("Received MQTT message on topic '{}'", topic);

        if topic == "config/venue" {
            let payload_str = std::str::from_utf8(payload)?;
            match serde_json::from_str::<VenueUpdate>(payload_str) {
                Ok(venue) => {
                    if let Err(e) = sender.send(MqttMessage::VenueUpdate(venue)) {
                        error!("Failed to send venue update message: {}", e);
                    }
                }
                Err(e) => {
                    error!("Failed to parse venue configuration: {}", e);
                }
            }
        } else if topic.starts_with("groups/") {
            // Parse topic: groups/{group_name}/{attribute}
            let topic_parts: Vec<&str> = topic.split('/').collect();
            if topic_parts.len() == 3 {
                let group_name = topic_parts[1].to_string();
                let attribute = topic_parts[2].to_string();
                let value = f32::from_be_bytes(payload.try_into().unwrap());

                if let Err(e) = sender.send(MqttMessage::GroupAttributeUpdate {
                    group_name,
                    attribute,
                    value,
                }) {
                    error!("Failed to send group attribute update message: {}", e);
                }
            } else {
                warn!("Invalid group topic format: {}", topic);
            }
        }

        Ok(())
    }

    pub fn get_client(&self) -> &AsyncClient {
        &self.client
    }
}
