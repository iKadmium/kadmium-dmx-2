use bytes::BufMut;
use kadmium_dmx_shared::{Message, NeewerLightParams, NeewerUpdate};

use crate::fixtures::neewer_fixture::NeewerFixture;

#[derive(Debug)]
pub struct NeewerUniverse {
    pub update: NeewerUpdate,
    pub fixtures: Vec<NeewerFixture>,
}

impl NeewerUniverse {
    pub fn new() -> Self {
        NeewerUniverse {
            update: NeewerUpdate::default(),
            fixtures: Vec::new(),
        }
    }

    pub fn add_neewer_fixture(&mut self, fixture: NeewerFixture) {
        self.update.fixtures.insert(
            fixture.address.to_string(),
            NeewerLightParams {
                hue: 0,
                saturation: 0,
                brightness: 0,
            },
        );

        self.fixtures.push(fixture);
    }

    pub fn render(&mut self) {
        for fixture in &self.fixtures {
            let update = self
                .update
                .fixtures
                .get_mut(&fixture.address.to_string())
                .unwrap();

            update.hue = fixture.channels.hue;
            update.saturation = fixture.channels.saturation;
            update.brightness = fixture.channels.brightness;
        }
    }

    pub fn get_update(&self, buf: &mut impl BufMut) -> std::io::Result<String> {
        self.update.encode(buf).map_err(std::io::Error::other)?;

        Ok("bt/neewer".to_string())
    }
}
