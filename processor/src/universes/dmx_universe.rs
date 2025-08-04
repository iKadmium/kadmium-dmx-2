use bytes::BufMut;
use tracing::error;
use std::collections::HashMap;
use tokio::sync::broadcast;

use crate::fixtures::{dmx_fixture::DmxFixture, fixture::{Fixture, FixtureAccessors}};

#[derive(Debug)]
pub struct DmxUniverse {
    pub universe_number: u16,
    pub fixtures: Vec<DmxFixture>,
    pub channels: Box<[u8; 512]>,
}

impl DmxUniverse {
    pub fn new(universe_number: u16) -> Self {
        Self {
            universe_number,
            fixtures: Vec::new(),
            channels: Box::new([0; 512]), // Initialize all channels to 0
        }
    }

    pub fn update(&mut self) -> std::io::Result<()> {
        for fixture in &mut self.fixtures {
            fixture.update()?;
        }
        Ok(())
    }

    pub fn render(&mut self) {
        for fixture in &mut self.fixtures {
            // Serialize each fixture's DMX channels into the buffer
            if let Err(e) = fixture.render(&mut self.channels) {
                error!("Error rendering fixture: {e}");
            }
        }
    }

    pub fn get_update(&self, buf: &mut impl BufMut) -> std::io::Result<String> {
        buf.put_slice(&self.channels[..]);
        Ok(format!("dmx/universe/{}", self.universe_number))
    }

    pub(crate) fn add_dmx_fixture(&mut self, fixture: DmxFixture) {
        self.fixtures.push(fixture);
    }

    pub fn update_fixture_subscriptions(&mut self, fixture_name: &str, attribute_receivers: HashMap<String, broadcast::Receiver<f32>>) {
        for fixture in &mut self.fixtures {
            if fixture.name == fixture_name {
                fixture.update_subscriptions(attribute_receivers);
                return;
            }
        }
    }
}
