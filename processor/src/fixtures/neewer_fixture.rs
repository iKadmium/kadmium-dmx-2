use std::collections::HashMap;

use kadmium_dmx_shared::{NeewerLightParams, VenueFixtureCommon, dmx_fixtures::fixture_personality::FixturePersonality};

use crate::{
    effects::{attribute::Attribute, effect::Effect, neewer_fake_strobe::NeewerFakeStrobe, neewer_hsv_to_hsv::NeewerHsvToHsv},
    fixtures::fixture::Fixture,
    macros::fixture_accessors,
};

#[derive(Debug)]
pub struct NeewerFixture {
    #[allow(dead_code)]
    pub name: String,
    pub attributes: HashMap<String, Attribute>,
    pub effects: Vec<Box<dyn Effect<NeewerFixture> + Send + Sync>>,
    pub groups: Vec<String>,
}

// Generate the accessor methods
fixture_accessors!(NeewerFixture);

impl NeewerFixture {
    pub fn new(common: VenueFixtureCommon) -> Self {
        let mut attributes = HashMap::new();

        // Create a dummy personality since create_effects ignores it for Neewer fixtures
        let dummy_personality = FixturePersonality::new("Dummy".to_string(), HashMap::new());
        let effects = Self::create_effects(&dummy_personality);

        for effect in &effects {
            for attribute in effect.get_attributes() {
                if !attributes.contains_key(*attribute) {
                    attributes.insert((*attribute).to_string(), Attribute::new((*attribute).to_string(), 0.0));
                }
            }
        }

        NeewerFixture {
            name: common.name.clone(),
            attributes,
            effects,
            groups: common.groups.clone(),
        }
    }
}

impl Fixture for NeewerFixture {
    type RenderTarget<'a> = NeewerLightParams;

    fn create_effects(_personality: &FixturePersonality) -> Vec<Box<dyn Effect<Self> + Send + Sync>> {
        let effects: Vec<Box<dyn Effect<Self> + Send + Sync>> = vec![Box::new(NeewerHsvToHsv {}), Box::new(NeewerFakeStrobe::new())];
        effects
    }
}
