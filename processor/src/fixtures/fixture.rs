use kadmium_dmx_shared::dmx_fixtures::fixture_personality::FixturePersonality;

use crate::effects::effect::Effect;

pub trait Fixture: Send + std::marker::Sized {
    type RenderTarget<'a>
    where
        Self: 'a;

    fn create_effects(personality: &FixturePersonality)
    -> Vec<Box<dyn Effect<Self> + Send + Sync>>;

    fn render(&mut self, target: &mut Self::RenderTarget<'_>) -> std::io::Result<()>;
}
