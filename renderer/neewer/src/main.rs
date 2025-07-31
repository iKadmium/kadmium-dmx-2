mod color;
mod fixture_manager;
mod mqtt_manager;
mod neewer_fixture;

use anyhow::Result;
use tracing::info;
use tracing_subscriber::fmt;

use fixture_manager::FixtureManager;
use mqtt_manager::MqttManager;

#[tokio::main]
async fn main() -> Result<()> {
    // Initialize tracing
    fmt::init();

    info!("Starting Neewer renderer...");

    // Configuration from environment variables
    let mqtt_host = std::env::var("MQTT_HOST").unwrap_or_else(|_| "localhost".to_string());
    let mqtt_port = std::env::var("MQTT_PORT")
        .unwrap_or_else(|_| "1883".to_string())
        .parse::<u16>()
        .unwrap_or(1883);

    info!("Connecting to MQTT broker at {}:{}", mqtt_host, mqtt_port);

    // Create FixtureManager
    let (fixture_manager, fixture_manager_tx) = FixtureManager::new();

    // Create MQTT manager
    let _mqtt_manager = MqttManager::new(&mqtt_host, mqtt_port, fixture_manager_tx).await?;

    info!("All systems started, running FixtureManager...");

    // Run the FixtureManager (this will block until the application shuts down)
    fixture_manager.run().await?;

    info!("Neewer renderer stopped");
    Ok(())
}
