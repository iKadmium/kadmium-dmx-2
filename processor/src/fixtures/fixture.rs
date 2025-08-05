use kadmium_dmx_shared::dmx_fixtures::fixture_personality::FixturePersonality;

use crate::effects::{attribute::Attribute, effect::Effect};

pub trait FixtureAccessors {
    // accessors
    fn attributes(&self) -> &std::collections::HashMap<String, Attribute>;
    fn effects_mut(&mut self) -> &mut Vec<Box<dyn Effect<Self> + Send + Sync>>;
    fn effects(&self) -> &Vec<Box<dyn Effect<Self> + Send + Sync>>;
}

pub trait Fixture: Send + std::marker::Sized + FixtureAccessors {
    type RenderTarget<'a>: ?Sized
    where
        Self: 'a;

    fn create_effects(personality: &FixturePersonality) -> Vec<Box<dyn Effect<Self> + Send + Sync>>;

    fn update(&mut self) -> std::io::Result<()> {
        // Only update effects - group control now happens directly via attribute watch channels
        for effect in self.effects_mut() {
            effect.update()?;
        }
        Ok(())
    }

    fn render(&self, target: &mut Self::RenderTarget<'_>) -> std::io::Result<()> {
        for effect in self.effects() {
            effect.render(self.attributes(), target)?;
        }

        Ok(())
    }
}
