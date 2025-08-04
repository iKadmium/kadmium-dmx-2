use bytes::BytesMut;
use std::collections::HashMap;
use tokio::sync::broadcast;

use crate::fixtures::fixture::{Fixture, FixtureAccessors};

/// Trait for universe implementations that can contain fixtures of type F
pub trait Universe: Send + Sync {
    type FixtureType: Fixture + FixtureAccessors;

    fn fixtures_mut(&mut self) -> &mut Vec<(Self::FixtureType, Vec<String>)>;

    /// Add a fixture to this universe along with its group memberships
    fn add_fixture(&mut self, fixture: Self::FixtureType, groups: Vec<String>);

    /// Update subscriptions for all fixtures based on current channel map
    /// This is the implementation-specific version
    fn update_all_fixture_subscriptions(&mut self, group_channels: &HashMap<String, HashMap<String, broadcast::Sender<f32>>>);

    /// Update all fixtures in the universe
    fn update(&mut self) -> std::io::Result<()> {
        for (fixture, _) in self.fixtures_mut() {
            fixture.update()?;
        }
        Ok(())
    }

    /// Render all fixtures in the universe
    fn render(&mut self) -> std::io::Result<()>;

    /// Get serialized update data for transmission
    fn get_update(&self, buf: &mut BytesMut) -> std::io::Result<String>;
}
