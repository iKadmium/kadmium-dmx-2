use std::collections::HashMap;

use kadmium_dmx_shared::{
    FixtureAddress, VenueDmxFixture,
    dmx_fixtures::{fixture_definition::FixtureDefinition, fixture_personality::FixturePersonality},
    venue_fixture::CommonFixtureProperties,
};
use tokio::sync::broadcast;

use crate::{
    effects::{attribute::Attribute, dmx_hsv_to_rgb::HsvToRgb, effect::Effect},
    fixtures::fixture::Fixture,
    macros::fixture_accessors,
};

#[derive(Debug)]
#[allow(dead_code)]
pub struct DmxFixture {
    pub name: String,
    pub dmx_address: u16,
    pub dmx_universe: u16,
    channel_indices: HashMap<String, usize>,
    pub attributes: HashMap<String, Attribute>,
    pub manufacturer: String,
    pub model: String,
    pub personality: String,
    subscriptions: HashMap<String, broadcast::Receiver<f32>>,
    pub effects: Vec<Box<dyn Effect<DmxFixture> + Send + Sync>>,
}

// Generate the accessor methods
fixture_accessors!(DmxFixture);

impl DmxFixture {
    pub fn new(config: &VenueDmxFixture, common: &CommonFixtureProperties, definition: &FixtureDefinition) -> Self {
        let mut channel_indices = HashMap::new();
        let mut attributes = HashMap::new();

        let personality = definition
            .personalities
            .get(&config.personality)
            .expect("Personality not found in fixture definition");

        let (dmx_address, dmx_universe) = match common.address {
            FixtureAddress::Sacn { universe, channel } => (channel, universe),
            FixtureAddress::ArtNet { universe, channel } => (channel, universe),
            _ => panic!("DMX fixtures must use DMX addresses"),
        };

        for (name, channel) in &personality.channels {
            channel_indices.insert(name.clone(), (channel.address + dmx_address) as usize);
        }

        let effects = Self::create_effects(personality, &dmx_address);

        for effect in &effects {
            for attribute in effect.get_attributes() {
                if !attributes.contains_key(*attribute) {
                    attributes.insert((*attribute).to_string(), Attribute::new((*attribute).to_string(), 0.0));
                }
            }
        }

        let subscriptions = HashMap::new();

        DmxFixture {
            name: common.name.clone(),
            dmx_address,
            dmx_universe,
            channel_indices,
            attributes,
            effects,
            manufacturer: config.manufacturer.clone(),
            model: config.model.clone(),
            personality: config.personality.clone(),
            subscriptions,
        }
    }
}

impl Fixture for DmxFixture {
    type RenderTarget<'a> = [u8; 512];
    type AddressType<'a> = u16;

    fn create_effects(personality: &FixturePersonality, address: &Self::AddressType<'_>) -> Vec<Box<dyn Effect<Self> + Send + Sync>> {
        let mut effects: Vec<Box<dyn Effect<Self> + Send + Sync>> = Vec::new();

        if HsvToRgb::valid_for_fixture(personality) {
            effects.push(Box::new(HsvToRgb::new(personality, *address)));
        }
        effects
    }
}
