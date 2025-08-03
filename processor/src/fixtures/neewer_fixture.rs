use std::collections::HashMap;

use kadmium_dmx_shared::{
    FixtureAddress, NeewerLightParams,
    dmx_fixtures::{channel::Channel, fixture_personality::FixturePersonality},
};
use tokio::sync::broadcast;

use crate::{
    effects::{attribute::Attribute, effect::Effect, neewer_hsv_to_hsv::NeewerHsvToHsv},
    fixtures::fixture::Fixture,
};

#[derive(Debug)]
pub struct NeewerFixture {
    pub name: String,
    pub address: String,
    pub attributes: HashMap<String, Attribute>,
    subscriptions: Vec<broadcast::Receiver<(String, f32)>>,
    pub effects: Vec<Box<dyn Effect<NeewerFixture> + Send + Sync>>,
}

impl NeewerFixture {
    pub fn new(
        name: String,
        address: FixtureAddress,
        group_senders: Vec<broadcast::Sender<(String, f32)>>,
    ) -> Self {
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

        let subscriptions = group_senders
            .iter()
            .map(|sender| sender.subscribe())
            .collect::<Vec<_>>();

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
