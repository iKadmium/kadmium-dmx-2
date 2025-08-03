use crate::{
    effects::effect::Effect,
    fixtures::{
        dmx_fixture::{self, DmxFixture},
        fixture,
    },
};

#[derive(Debug)]
pub struct HsvToRgb {}

impl Effect<DmxFixture> for HsvToRgb {
    fn apply(
        &self,
        fixture: &DmxFixture,
        target: &mut <dmx_fixture::DmxFixture as fixture::Fixture>::RenderTarget<'_>,
    ) -> std::io::Result<()> {
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

        let red_index = fixture.get_channel_index("Red")?;
        let green_index = fixture.get_channel_index("Green")?;
        let blue_index = fixture.get_channel_index("Blue")?;

        buffer[red_index] = (r * 255.0).round() as u8;
        buffer[green_index] = (g * 255.0).round() as u8;
        buffer[blue_index] = (b * 255.0).round() as u8;

        Ok(())
    }

    fn get_attributes(&self) -> &[&str] {
        &["Hue", "Saturation", "Brightness"]
    }
}
