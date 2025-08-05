use rumqttc::{AsyncClient, ClientError};

use crate::fixtures::fixture::{Fixture, FixtureAccessors};

/// Trait for universe implementations that can contain fixtures of type F
pub trait Universe: Send + Sync {
    type FixtureType: Fixture + FixtureAccessors;

    fn fixtures_mut(&mut self) -> &mut Vec<Self::FixtureType>;

    /// Update all fixtures in the universe
    fn update(&mut self) -> std::io::Result<()> {
        for fixture in self.fixtures_mut() {
            fixture.update()?;
        }
        Ok(())
    }

    /// Render all fixtures in the universe
    fn render(&mut self) -> std::io::Result<()>;

    /// Send update data
    async fn send_update(&self, sender: &AsyncClient) -> Result<(), ClientError>;
}
