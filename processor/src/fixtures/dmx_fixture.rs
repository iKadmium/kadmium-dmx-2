use std::collections::HashMap;

use kadmium_dmx_shared::{
    VenueDmxFixture,
    dmx_fixtures::{fixture_definition::FixtureDefinition, fixture_personality::FixturePersonality},
};

use crate::{
    effects::{attribute::Attribute, dmx_hsv_to_rgb::HsvToRgb, effect::Effect},
    fixtures::fixture::Fixture,
    macros::fixture_accessors,
};

#[derive(Debug)]
#[allow(dead_code)]
pub struct DmxFixture {
    pub name: String,
    pub attributes: HashMap<String, Attribute>,
    pub manufacturer: String,
    pub model: String,
    pub personality: String,
    pub effects: Vec<Box<dyn Effect<DmxFixture> + Send + Sync>>,
    pub groups: Vec<String>,
}

// Generate the accessor methods
fixture_accessors!(DmxFixture);

impl DmxFixture {
    pub fn new(config: &VenueDmxFixture, definition: &FixtureDefinition) -> Self {
        let mut channel_indices = HashMap::new();
        let mut attributes = HashMap::new();

        let personality = definition
            .personalities
            .get(&config.personality)
            .expect("Personality not found in fixture definition");

        for (name, channel) in &personality.channels {
            channel_indices.insert(name.clone(), channel.address as usize);
        }

        let effects = Self::create_effects(personality);

        for effect in &effects {
            for attribute in effect.get_attributes() {
                if !attributes.contains_key(*attribute) {
                    attributes.insert((*attribute).to_string(), Attribute::new((*attribute).to_string(), 0.0));
                }
            }
        }

        DmxFixture {
            name: config.common.name.clone(),
            attributes,
            effects,
            manufacturer: config.manufacturer.clone(),
            model: config.model.clone(),
            personality: config.personality.clone(),
            groups: config.common.groups.clone(),
        }
    }
}

impl Fixture for DmxFixture {
    type RenderTarget<'a> = [u8];

    fn create_effects(personality: &FixturePersonality) -> Vec<Box<dyn Effect<Self> + Send + Sync>> {
        let mut effects: Vec<Box<dyn Effect<Self> + Send + Sync>> = Vec::new();

        if HsvToRgb::valid_for_fixture(personality) {
            effects.push(Box::new(HsvToRgb::new(personality)));
        }
        effects
    }
}
