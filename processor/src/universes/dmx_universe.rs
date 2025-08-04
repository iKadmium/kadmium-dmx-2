use bytes::{BufMut, BytesMut};
use std::collections::HashMap;
use tokio::sync::broadcast;

use crate::fixtures::{
    dmx_fixture::DmxFixture,
    fixture::{Fixture, FixtureAccessors},
};
use crate::universes::universe::Universe;

#[derive(Debug)]
pub struct DmxUniverse {
    pub universe_number: u16,
    pub fixtures: Vec<(DmxFixture, Vec<String>)>, // (fixture, groups)
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
}

impl Universe for DmxUniverse {
    type FixtureType = DmxFixture;

    fn fixtures_mut(&mut self) -> &mut Vec<(Self::FixtureType, Vec<String>)> {
        &mut self.fixtures
    }

    fn add_fixture(&mut self, fixture: Self::FixtureType, groups: Vec<String>) {
        self.fixtures.push((fixture, groups));
    }

    fn update_all_fixture_subscriptions(&mut self, group_channels: &HashMap<String, HashMap<String, broadcast::Sender<f32>>>) {
        // Update each fixture with subscriptions for its specific groups
        for (fixture, fixture_groups) in &mut self.fixtures {
            // Each fixture gets its own copy of receivers for its groups
            let mut fixture_receivers = HashMap::new();
            for group_name in fixture_groups {
                if let Some(group_attrs) = group_channels.get(group_name) {
                    for (attribute_name, sender) in group_attrs {
                        let receiver = sender.subscribe();
                        fixture_receivers.insert(attribute_name.clone(), receiver);
                    }
                }
            }
            fixture.update_subscriptions(fixture_receivers);
        }
    }

    fn render(&mut self) -> std::io::Result<()> {
        for (fixture, _) in &mut self.fixtures {
            // Serialize each fixture's DMX channels into the buffer
            fixture.render(&mut self.channels)?;
        }
        Ok(())
    }

    fn get_update(&self, buf: &mut BytesMut) -> std::io::Result<String> {
        buf.put_slice(&self.channels[..]);
        Ok(format!("dmx/universe/{}", self.universe_number))
    }
}
