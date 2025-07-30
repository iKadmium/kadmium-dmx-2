use rumqttd::{Broker, Config};
use std::time::Duration;
use tokio::time::sleep;
use tracing::error;

/// Starts the MQTT broker in a background task
pub async fn start_broker(config: Config) -> Result<(), Box<dyn std::error::Error + Send + Sync>> {
    let mut broker = Broker::new(config);

    // Spawn broker in background task so it doesn't block
    tokio::spawn(async move {
        if let Err(e) = broker.start() {
            error!("MQTT broker failed to start: {e}");
        }
    });

    // Give the broker a moment to start up
    sleep(Duration::from_millis(100)).await;

    Ok(())
}
