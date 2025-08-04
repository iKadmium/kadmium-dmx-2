use std::collections::HashMap;

use kadmium_dmx_shared::dmx_fixtures::fixture_personality::FixturePersonality;
use tracing::error;

use crate::effects::{attribute::Attribute, effect::Effect};

pub trait FixtureAccessors {
    // accessors
    fn attributes(&self) -> &std::collections::HashMap<String, Attribute>;
    fn subscriptions_mut(&mut self) -> &mut HashMap<String, tokio::sync::broadcast::Receiver<f32>>;
    fn effects_mut(&mut self) -> &mut Vec<Box<dyn Effect<Self> + Send + Sync>>;
    fn effects(&self) -> &Vec<Box<dyn Effect<Self> + Send + Sync>>;

    // subscription management
    fn update_subscriptions(&mut self, attribute_receivers: HashMap<String, tokio::sync::broadcast::Receiver<f32>>);
}

pub trait Fixture: Send + std::marker::Sized + FixtureAccessors {
    type RenderTarget<'a>
    where
        Self: 'a;
    type AddressType<'a>
    where
        Self: 'a;

    fn create_effects(personality: &FixturePersonality, address: &Self::AddressType<'_>) -> Vec<Box<dyn Effect<Self> + Send + Sync>>;

    fn set_value(&self, attribute: &str, value: f32) -> std::io::Result<()> {
        let attribute_value = self.attributes().get(attribute);
        attribute_value
            .ok_or_else(|| std::io::Error::new(std::io::ErrorKind::NotFound, format!("Attribute '{attribute}' not found")))?
            .set_value(value)
            .map_err(|_| std::io::Error::new(std::io::ErrorKind::InvalidInput, format!("Failed to set value for attribute '{attribute}'")))
    }

    fn update(&mut self) -> std::io::Result<()> {
        let mut updates = HashMap::new();

        for effect in self.effects_mut() {
            effect.update()?;
        }

        for (attribute_name, subscription) in self.subscriptions_mut() {
            match subscription.try_recv() {
                Ok(value) => {
                    updates.insert(attribute_name.clone(), value);
                }
                Err(tokio::sync::broadcast::error::TryRecvError::Empty) => {
                    // No messages available - this is normal, not an error
                }
                Err(tokio::sync::broadcast::error::TryRecvError::Closed) => {
                    error!("Subscription channel closed for attribute: {}", attribute_name);
                }
                Err(tokio::sync::broadcast::error::TryRecvError::Lagged(skipped)) => {
                    error!("Subscription lagged for attribute '{}', skipped {} messages", attribute_name, skipped);
                }
            }
        }

        for (attribute, value) in updates {
            self.set_value(&attribute, value)?;
        }
        Ok(())
    }

    fn render(&self, target: &mut Self::RenderTarget<'_>) -> std::io::Result<()> {
        for effect in self.effects() {
            effect.render(self, target)?;
        }

        Ok(())
    }
}
