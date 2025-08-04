use std::collections::HashMap;

use kadmium_dmx_shared::{FixtureAddress, NeewerLightParams, dmx_fixtures::fixture_personality::FixturePersonality, venue_fixture::CommonFixtureProperties};
use tokio::sync::broadcast;

use crate::{
    effects::{attribute::Attribute, effect::Effect, neewer_fake_strobe::NeewerFakeStrobe, neewer_hsv_to_hsv::NeewerHsvToHsv},
    fixtures::fixture::Fixture,
    macros::fixture_accessors,
};

#[derive(Debug)]
pub struct NeewerFixture {
    pub name: String,
    pub address: String,
    pub attributes: HashMap<String, Attribute>,
    subscriptions: HashMap<String, broadcast::Receiver<f32>>,
    pub effects: Vec<Box<dyn Effect<NeewerFixture> + Send + Sync>>,
    pub groups: Vec<String>,
}

// Generate the accessor methods
fixture_accessors!(NeewerFixture);

impl NeewerFixture {
    pub fn new(common: CommonFixtureProperties) -> Self {
        let mut attributes = HashMap::new();

        // Create a dummy personality since create_effects ignores it for Neewer fixtures
        let dummy_personality = FixturePersonality::new("Dummy".to_string(), HashMap::new());
        let effects = Self::create_effects(&dummy_personality, &"");

        for effect in &effects {
            for attribute in effect.get_attributes() {
                if !attributes.contains_key(*attribute) {
                    attributes.insert((*attribute).to_string(), Attribute::new((*attribute).to_string(), 0.0));
                }
            }
        }

        let address = if let FixtureAddress::Bluetooth { uuid } = common.address {
            uuid.to_string()
        } else {
            panic!("Neewer fixtures must use Bluetooth addresses");
        };

        NeewerFixture {
            name: common.name.clone(),
            address,
            attributes,
            subscriptions: HashMap::new(),
            effects,
            groups: common.groups.clone(),
        }
    }
}

impl Fixture for NeewerFixture {
    type RenderTarget<'a> = NeewerLightParams;
    type AddressType<'a> = &'a str;

    fn create_effects(_personality: &FixturePersonality, _address: &Self::AddressType<'_>) -> Vec<Box<dyn Effect<Self> + Send + Sync>> {
        let effects: Vec<Box<dyn Effect<Self> + Send + Sync>> = vec![Box::new(NeewerHsvToHsv {}), Box::new(NeewerFakeStrobe::new())];
        effects
    }
}
