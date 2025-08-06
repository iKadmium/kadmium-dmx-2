use std::{collections::HashMap, time::Instant};

use kadmium_dmx_shared::{NeewerLightParams, dmx_fixtures::fixture_personality::FixturePersonality};
use tracing::trace;

use crate::{
    effects::{attribute::Attribute, effect::Effect},
    fixtures::neewer_fixture::NeewerFixture,
};

#[derive(Debug)]
pub struct NeewerFakeStrobe {
    next_flip: Instant,
    on: bool,
}

impl NeewerFakeStrobe {
    pub fn new() -> Self {
        let next_flip = Instant::now() + std::time::Duration::from_millis(50);
        NeewerFakeStrobe { next_flip, on: true }
    }
}

impl Effect<NeewerFixture> for NeewerFakeStrobe {
    fn get_attributes(&self) -> &[&str] {
        &["Strobe"]
    }

    fn update(&mut self) -> std::io::Result<()> {
        if Instant::now() > self.next_flip {
            self.on = !self.on;
            self.next_flip = Instant::now() + std::time::Duration::from_millis(50);
        }
        Ok(())
    }

    fn render(&self, attributes: &HashMap<String, Attribute>, target: &mut NeewerLightParams) -> std::io::Result<()> {
        let enabled = self.get_attribute_value(attributes, "Strobe")? == 1.0;

        if !self.on && enabled {
            trace!("NeewerFakeStrobe: Turning off strobe effect");
            target.brightness = 0;
        } else {
            trace!("NeewerFakeStrobe: Turning on strobe effect");
        }

        Ok(())
    }

    fn valid_for_fixture(personality: &FixturePersonality) -> bool
    where
        Self: Sized,
    {
        personality.channels.contains_key("Brightness") && !personality.channels.contains_key("Strobe")
    }
}
