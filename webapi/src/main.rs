mod controllers;
pub(crate) mod data_access;
mod midi_publisher;
mod models;
mod mqtt_broker;
mod web_server;

use rumqttd::Config;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error + Send + Sync>> {
    // Initialize tracing for logging
    tracing_subscriber::fmt()
        .with_env_filter(
            tracing_subscriber::EnvFilter::try_from_default_env()
                .unwrap_or_else(|_| tracing_subscriber::EnvFilter::new("info")),
        )
        .init();

    // Load MQTT broker configuration
    let config = config::Config::builder()
        .add_source(config::File::with_name("rumqttd.toml"))
        .build()?;

    let rumqttd_config: Config = config.try_deserialize()?;

    // Start MQTT broker
    mqtt_broker::start_broker(rumqttd_config).await?;

    // Publish MIDI map configuration
    midi_publisher::publish_midi_map().await?;

    // Setup and start web server
    let app = web_server::setup_web_server().await?;
    web_server::start_server(app, "[::]:3000").await?;

    Ok(())
}
