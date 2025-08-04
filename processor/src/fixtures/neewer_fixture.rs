use std::collections::HashMap;

use kadmium_dmx_shared::{
    FixtureAddress, NeewerLightParams,
    dmx_fixtures::{channel::Channel, fixture_personality::FixturePersonality},
};
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
}

// Generate the accessor methods
fixture_accessors!(NeewerFixture);

impl NeewerFixture {
    pub fn new(name: String, address: FixtureAddress) -> Self {
        let mut attributes = HashMap::new();
        let channels = HashMap::from([
            ("Hue".to_string(), Channel::new("Hue".to_string(), 1)),
            ("Saturation".to_string(), Channel::new("Saturation".to_string(), 2)),
            ("Brightness".to_string(), Channel::new("Brightness".to_string(), 3)),
        ]);
        let personality = FixturePersonality::new("Neewer Personality".to_string(), channels);
        let address = if let FixtureAddress::Bluetooth { uuid } = address {
            uuid.to_string()
        } else {
            panic!("Neewer fixtures must use Bluetooth addresses");
        };
        let effects = Self::create_effects(&personality, &address.as_ref());
        for effect in &effects {
            for attribute in effect.get_attributes() {
                if !attributes.contains_key(*attribute) {
                    attributes.insert((*attribute).to_string(), Attribute::new((*attribute).to_string(), 0.0));
                }
            }
        }

        let subscriptions = HashMap::new();

        NeewerFixture {
            name,
            address,
            attributes,
            effects,
            subscriptions,
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
