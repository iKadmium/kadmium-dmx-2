use kadmium_dmx_shared::{DefinitionsSet, VenueDmxFixture};
use rumqttc::{AsyncClient, ClientError, QoS};

use crate::fixtures::{
    dmx_fixture::DmxFixture,
    fixture::Fixture,
};
use crate::universes::universe::Universe;

#[derive(Debug)]
pub struct DmxUniverse {
    pub universe_number: u16,
    pub fixtures: Vec<DmxFixture>, // (fixture, groups)
    pub channels: [u8; 512],
}

impl DmxUniverse {
    pub fn new(universe_number: u16, fixtures: Vec<VenueDmxFixture>, definitions: &DefinitionsSet) -> std::io::Result<Self> {
        let fixtures = fixtures
            .into_iter()
            .filter_map(|fixture| {
                definitions
                    .get(&(fixture.manufacturer.clone(), fixture.model.clone()))
                    .map(|definition| DmxFixture::new(&fixture, definition))
            })
            .collect();

        Ok(Self {
            universe_number,
            fixtures,
            channels: [0; 512], // Initialize all channels to 0
        })
    }
}

impl Universe for DmxUniverse {
    type FixtureType = DmxFixture;

    fn fixtures_mut(&mut self) -> &mut Vec<Self::FixtureType> {
        &mut self.fixtures
    }

    fn render(&mut self) -> std::io::Result<()> {
        for fixture in &mut self.fixtures {
            // Serialize each fixture's DMX channels into the buffer
            fixture.render(&mut self.channels)?;
        }
        Ok(())
    }

    async fn send_update(&self, client: &AsyncClient) -> Result<(), ClientError> {
        client
            .publish(
                format!("dmx/universe/{}", self.universe_number),
                QoS::AtLeastOnce,
                false,
                self.channels[..].to_vec(),
            )
            .await?;
        Ok(())
    }
}
