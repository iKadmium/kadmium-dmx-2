use bytes::BufMut;
use kadmium_dmx_shared::{Message, NeewerLightParams, NeewerUpdate};
use tracing::error;
use std::collections::HashMap;
use tokio::sync::broadcast;

use crate::fixtures::{fixture::{Fixture, FixtureAccessors}, neewer_fixture::NeewerFixture};

#[derive(Debug)]
pub struct NeewerUniverse {
    pub update_message: NeewerUpdate,
    pub fixtures: Vec<NeewerFixture>,
}

impl NeewerUniverse {
    pub fn new() -> Self {
        NeewerUniverse {
            update_message: NeewerUpdate::default(),
            fixtures: Vec::new(),
        }
    }

    pub fn add_neewer_fixture(&mut self, fixture: NeewerFixture) {
        self.update_message.fixtures.insert(
            fixture.address.to_string(),
            NeewerLightParams {
                hue: 0,
                saturation: 0,
                brightness: 0,
            },
        );

        self.fixtures.push(fixture);
    }

    pub fn update(&mut self) -> std::io::Result<()> {
        for fixture in &mut self.fixtures {
            fixture.update()?;
        }

        Ok(())
    }

    pub fn render(&mut self) {
        for fixture in &mut self.fixtures {
            let render_target = self.update_message.fixtures.get_mut(&fixture.address.to_string()).unwrap();

            if let Err(e) = fixture.render(render_target) {
                error!("Failed to render fixture '{}': {}", fixture.name, e);
            }
        }
    }

    pub fn get_update(&self, buf: &mut impl BufMut) -> std::io::Result<String> {
        self.update_message.encode(buf).map_err(std::io::Error::other)?;

        Ok("bt/neewer".to_string())
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
