use bytes::BytesMut;
use kadmium_dmx_shared::{Message, NeewerLightParams, NeewerUpdate};
use std::collections::HashMap;
use tokio::sync::broadcast;
use tracing::error;

use crate::fixtures::{
    fixture::{Fixture, FixtureAccessors},
    neewer_fixture::NeewerFixture,
};
use crate::universes::universe::Universe;

#[derive(Debug)]
pub struct NeewerUniverse {
    pub update_message: NeewerUpdate,
    pub fixtures: Vec<(NeewerFixture, Vec<String>)>,
}

impl NeewerUniverse {
    pub fn new() -> Self {
        NeewerUniverse {
            update_message: NeewerUpdate::default(),
            fixtures: Vec::new(),
        }
    }
}

impl Universe for NeewerUniverse {
    type FixtureType = NeewerFixture;

    fn add_fixture(&mut self, fixture: Self::FixtureType, groups: Vec<String>) {
        self.update_message.fixtures.insert(
            fixture.address.to_string(),
            NeewerLightParams {
                hue: 0,
                saturation: 0,
                brightness: 0,
            },
        );

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

    fn update(&mut self) -> std::io::Result<()> {
        for (fixture, _) in &mut self.fixtures {
            fixture.update()?;
        }
        Ok(())
    }

    fn render(&mut self) -> std::io::Result<()> {
        for (fixture, _) in &mut self.fixtures {
            let render_target = self
                .update_message
                .fixtures
                .get_mut(&fixture.address)
                .ok_or_else(|| std::io::Error::new(std::io::ErrorKind::NotFound, format!("No render target found for fixture {}", fixture.name)))?;

            fixture.render(render_target)?;
        }
        Ok(())
    }

    fn get_update(&self, buf: &mut BytesMut) -> std::io::Result<String> {
        self.update_message.encode(buf).map_err(std::io::Error::other)?;

        Ok("bt/neewer".to_string())
    }
}
