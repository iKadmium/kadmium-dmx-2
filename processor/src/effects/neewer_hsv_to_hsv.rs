use crate::{
    effects::effect::Effect,
    fixtures::{fixture::Fixture, neewer_fixture::NeewerFixture},
};

#[derive(Debug)]
pub struct NeewerHsvToHsv;

impl Effect<NeewerFixture> for NeewerHsvToHsv {
    fn render(&self, fixture: &NeewerFixture, params: &mut <NeewerFixture as Fixture>::RenderTarget<'_>) -> std::io::Result<()> {
        let hue = self.get_attribute_value(&fixture.attributes, "Hue")?;
        let saturation = self.get_attribute_value(&fixture.attributes, "Saturation")?;
        let brightness = self.get_attribute_value(&fixture.attributes, "Brightness")?;

        params.hue = (hue * 360.0) as u32;
        params.saturation = (saturation * 100.0) as u32;
        params.brightness = (brightness * 100.0) as u32;

        Ok(())
    }

    fn get_attributes(&self) -> &[&str] {
        &["Hue", "Saturation", "Brightness"]
    }

    fn valid_for_fixture(personality: &kadmium_dmx_shared::dmx_fixtures::fixture_personality::FixturePersonality) -> bool {
        personality.channels.contains_key("Hue") && personality.channels.contains_key("Saturation") && personality.channels.contains_key("Brightness")
    }
}
