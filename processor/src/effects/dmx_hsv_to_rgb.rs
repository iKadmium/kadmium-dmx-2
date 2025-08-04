use kadmium_dmx_shared::dmx_fixtures::fixture_personality::FixturePersonality;

use crate::{
    effects::effect::Effect,
    fixtures::{
        dmx_fixture::{self, DmxFixture},
        fixture,
    },
};

#[derive(Debug)]
pub struct HsvToRgb {
    red_index: usize,
    green_index: usize,
    blue_index: usize,
}
impl HsvToRgb {
    pub(crate) fn new(personality: &FixturePersonality, address: u16) -> Self {
        let red_index = (personality.channels.get("Red").unwrap().address + address) as usize;
        let green_index = (personality.channels.get("Green").unwrap().address + address) as usize;
        let blue_index = (personality.channels.get("Blue").unwrap().address + address) as usize;

        Self {
            red_index,
            green_index,
            blue_index,
        }
    }
}

impl Effect<DmxFixture> for HsvToRgb {
    fn render(&self, fixture: &DmxFixture, target: &mut <dmx_fixture::DmxFixture as fixture::Fixture>::RenderTarget<'_>) -> std::io::Result<()> {
        let buffer = target;

        let h = self.get_attribute_value(&fixture.attributes, "Hue")?;
        let s = self.get_attribute_value(&fixture.attributes, "Saturation")?;
        let v = self.get_attribute_value(&fixture.attributes, "Brightness")?;

        let i = (h * 6.0).floor() as u32;
        let f = h * 6.0 - i as f32;
        let p = v * (1.0 - s);
        let q = v * (1.0 - f * s);
        let t = v * (1.0 - (1.0 - f) * s);

        let (r, g, b) = match i % 6 {
            0 => (v, t, p),
            1 => (q, v, p),
            2 => (p, v, t),
            3 => (p, q, v),
            4 => (t, p, v),
            5 => (v, p, q),
            _ => (0.0, 0.0, 0.0), // Should not happen
        };

        buffer[self.red_index] = (r * 255.0).round() as u8;
        buffer[self.green_index] = (g * 255.0).round() as u8;
        buffer[self.blue_index] = (b * 255.0).round() as u8;

        Ok(())
    }

    fn get_attributes(&self) -> &[&str] {
        &["Hue", "Saturation", "Brightness"]
    }

    fn valid_for_fixture(personality: &FixturePersonality) -> bool {
        personality.channels.contains_key("Red") && personality.channels.contains_key("Green") && personality.channels.contains_key("Blue")
    }
}
