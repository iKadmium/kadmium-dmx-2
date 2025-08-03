use std::collections::HashMap;

use kadmium_dmx_shared::{
    FixtureAddress, NeewerLightParams,
    dmx_fixtures::{channel::Channel, fixture_personality::FixturePersonality},
};

use crate::{
    effects::{attribute::Attribute, effect::Effect, neewer_hsv_to_hsv::NeewerHsvToHsv},
    fixtures::fixture::Fixture,
};

#[derive(Debug)]
pub struct NeewerFixture {
    pub name: String,
    pub address: String,
    pub attributes: HashMap<String, Attribute>,
    pub channels: NeewerLightParams,
    pub effects: Vec<Box<dyn Effect<NeewerFixture> + Send + Sync>>,
}

impl NeewerFixture {
    pub fn new(name: String, address: FixtureAddress) -> Self {
        let mut attributes = HashMap::new();
        let channels = HashMap::from([
            ("Hue".to_string(), Channel::new("Hue".to_string(), 1)),
            (
                "Saturation".to_string(),
                Channel::new("Saturation".to_string(), 2),
            ),
            (
                "Brightness".to_string(),
                Channel::new("Brightness".to_string(), 3),
            ),
        ]);
        let personality = FixturePersonality::new("Neewer Personality".to_string(), channels);
        let effects = Self::create_effects(&personality);
        for effect in &effects {
            for attribute in effect.get_attributes() {
                if !attributes.contains_key(*attribute) {
                    attributes.insert(
                        (*attribute).to_string(),
                        Attribute::new((*attribute).to_string(), 0.0),
                    );
                }
            }
        }

        let address = if let FixtureAddress::Bluetooth { uuid } = address {
            uuid.to_string()
        } else {
            panic!("Neewer fixtures must use Bluetooth addresses");
        };

        NeewerFixture {
            name,
            address,
            attributes,
            channels: NeewerLightParams {
                hue: 0,
                saturation: 0,
                brightness: 0,
            },
            effects,
        }
    }
}

impl Fixture for NeewerFixture {
    type RenderTarget<'a> = NeewerLightParams;

    fn render(&mut self, target: &mut Self::RenderTarget<'_>) -> std::io::Result<()> {
        for effect in &self.effects {
            effect.apply(self, target)?;
        }
        Ok(())
    }

    fn create_effects(
        _personality: &kadmium_dmx_shared::dmx_fixtures::fixture_personality::FixturePersonality,
    ) -> Vec<Box<dyn Effect<Self> + Send + Sync>> {
        let effects: Vec<Box<dyn Effect<Self> + Send + Sync>> = vec![Box::new(NeewerHsvToHsv {})];
        effects
    }
}
