use std::time::Instant;

use tracing::{info, trace};

use crate::{effects::effect::Effect, fixtures::neewer_fixture::NeewerFixture};

#[derive(Debug)]
pub struct NeewerFakeStrobe {
    next_flip: Instant,
    on: bool,
}

impl NeewerFakeStrobe {
    pub fn new() -> Self {
        let next_flip = Instant::now() + std::time::Duration::from_millis(50);
        NeewerFakeStrobe {
            next_flip,
            on: true,
        }
    }
}

impl Effect<NeewerFixture> for NeewerFakeStrobe {
    fn get_attributes(&self) -> &[&str] {
        &["Strobe"]
    }

    fn apply(
        &mut self,
        fixture: &NeewerFixture,
        target: &mut <NeewerFixture as crate::fixtures::fixture::Fixture>::RenderTarget<'_>,
    ) -> std::io::Result<()> {
        if Instant::now() > self.next_flip {
            self.on = !self.on;
            self.next_flip = Instant::now() + std::time::Duration::from_millis(50);
        }

        let enabled = if let Some(attr) = fixture.attributes.get("Strobe") {
            attr.get_value() == 1.0
        } else {
            false
        };

        if !self.on && enabled {
            info!("NeewerFakeStrobe: Turning off strobe effect");
            target.brightness = 0;
        } else {
            info!("NeewerFakeStrobe: Turning on strobe effect");
        }

        Ok(())
    }
}
