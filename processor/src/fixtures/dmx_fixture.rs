use std::collections::HashMap;

use kadmium_dmx_shared::{
    FixtureAddress, VenueDmxFixture,
    dmx_fixtures::{
        fixture_definition::FixtureDefinition, fixture_personality::FixturePersonality,
    },
    venue_fixture::CommonFixtureProperties,
};
use tokio::sync::broadcast;

use crate::{
    effects::{attribute::Attribute, dmx_hsv_to_rgb::HsvToRgb, effect::Effect},
    fixtures::fixture::Fixture,
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
    subscriptions: Vec<broadcast::Receiver<(String, f32)>>,
    pub effects: Vec<Box<dyn Effect<DmxFixture> + Send + Sync>>,
}

impl DmxFixture {
    pub fn new(
        config: &VenueDmxFixture,
        common: &CommonFixtureProperties,
        definition: &FixtureDefinition,
        group_senders: Vec<broadcast::Sender<(String, f32)>>,
    ) -> Self {
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

        let effects = Self::create_effects(personality);

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

        let subscriptions = group_senders
            .iter()
            .map(|sender| sender.subscribe())
            .collect::<Vec<_>>();

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

    pub fn get_channel_index(&self, channel_name: &str) -> std::io::Result<usize> {
        self.channel_indices
            .get(channel_name)
            .ok_or(std::io::Error::new(
                std::io::ErrorKind::NotFound,
                format!(
                    "Channel '{channel_name}' not found in fixture '{}'",
                    self.name
                ),
            ))
            .cloned()
    }
}

impl Fixture for DmxFixture {
    type RenderTarget<'a> = [u8; 512];

    fn render(&mut self, target: &mut Self::RenderTarget<'_>) -> std::io::Result<()> {
        self.subscriptions.iter_mut().for_each(|subscription| {
            while let Ok((attribute_name, value)) = subscription.try_recv() {
                self.attributes
                    .get(&attribute_name)
                    .map(|attr| attr.set_value(value).unwrap_or(()))
                    .unwrap_or_else(|| {
                        eprintln!(
                            "Attribute '{}' not found in fixture '{}'",
                            attribute_name, self.name
                        );
                    });
            }
        });

        // Temporarily move effects out to avoid borrowing conflicts
        let mut effects = std::mem::take(&mut self.effects);

        for effect in &mut effects {
            effect.apply(self, target)?;
        }

        // Move effects back
        self.effects = effects;
        Ok(())
    }

    fn create_effects(
        personality: &FixturePersonality,
    ) -> Vec<Box<dyn Effect<Self> + Send + Sync>> {
        let mut effects: Vec<Box<dyn Effect<Self> + Send + Sync>> = Vec::new();

        if personality.channels.contains_key("Red")
            && personality.channels.contains_key("Green")
            && personality.channels.contains_key("Blue")
        {
            let hsv_to_rgb = HsvToRgb {};
            effects.push(Box::new(hsv_to_rgb));
        }
        effects
    }
}
