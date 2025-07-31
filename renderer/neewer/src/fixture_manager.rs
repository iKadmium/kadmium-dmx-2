use anyhow::Result;
use std::collections::HashMap;
use tokio::sync::mpsc;
use tracing::{debug, error, info, warn};

use crate::neewer_fixture::NeweerFixture;
use kadmium_dmx_shared::NeewerUpdate;

/// Commands that can be sent to the FixtureManager
#[derive(Debug)]
#[allow(dead_code)]
pub enum FixtureManagerCommand {
    /// Update the collection of fixtures based on venue configuration
    UpdateVenue(Vec<NeweerFixture>),
    /// Update a specific fixture with new lighting parameters
    UpdateFixture(NeewerUpdate),
    /// Get the current list of fixture addresses (for MQTT subscription)
    GetFixtureAddresses(mpsc::Sender<Vec<String>>),
}

/// Manages Neewer fixtures and handles updates
pub struct FixtureManager {
    fixtures: HashMap<String, NeweerFixture>,
    command_rx: mpsc::Receiver<FixtureManagerCommand>,
}

impl FixtureManager {
    /// Create a new FixtureManager
    pub fn new() -> (Self, mpsc::Sender<FixtureManagerCommand>) {
        let (command_tx, command_rx) = mpsc::channel(100);

        let manager = Self {
            fixtures: HashMap::new(),
            command_rx,
        };

        (manager, command_tx)
    }

    /// Start the FixtureManager event loop
    pub async fn run(mut self) -> Result<()> {
        info!("FixtureManager started");

        while let Some(command) = self.command_rx.recv().await {
            match command {
                FixtureManagerCommand::UpdateVenue(fixtures) => {
                    self.handle_venue_update(fixtures).await;
                }
                FixtureManagerCommand::UpdateFixture(update) => {
                    self.handle_fixture_update(update).await;
                }
                FixtureManagerCommand::GetFixtureAddresses(response_tx) => {
                    let addresses: Vec<String> = self.fixtures.keys().cloned().collect();
                    if let Err(e) = response_tx.send(addresses).await {
                        error!("Failed to send fixture addresses: {}", e);
                    }
                }
            }
        }

        info!("FixtureManager stopped");
        Ok(())
    }

    /// Handle venue configuration updates
    async fn handle_venue_update(&mut self, fixtures: Vec<NeweerFixture>) {
        info!("Updating venue with {} Neewer fixtures", fixtures.len());

        // Clear existing fixtures
        self.fixtures.clear();

        // Add new fixtures
        for fixture in fixtures {
            info!("Adding Neewer fixture: {fixture:?}");
            self.fixtures
                .insert(fixture.bluetooth_address.clone(), fixture);
        }

        info!("Venue update complete: {} fixtures", self.fixtures.len());
    }

    /// Handle fixture lighting updates
    async fn handle_fixture_update(&mut self, update: NeewerUpdate) {
        for (bt_address, light_params) in update.fixtures {
            debug!(
                "Updating fixture {} with H:{} S:{} B:{}",
                bt_address, light_params.hue, light_params.saturation, light_params.brightness
            );

            if let Some(fixture) = self.fixtures.get(&bt_address) {
                // Send update to the fixture's task
                fixture.update_color(light_params).await;
            } else {
                warn!("Received update for unknown fixture: {}", bt_address);
            }
        }
    }
}
