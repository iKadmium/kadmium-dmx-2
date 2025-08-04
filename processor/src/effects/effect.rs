use std::{collections::HashMap, fmt::Debug};

use kadmium_dmx_shared::dmx_fixtures::fixture_personality::FixturePersonality;

use crate::{effects::attribute::Attribute, fixtures::fixture::Fixture};

pub trait Effect<F: Fixture>: Send + Sync + Debug {
    fn get_attribute_value(&self, attributes: &HashMap<String, Attribute>, name: &str) -> std::io::Result<f32> {
        Ok(attributes
            .get(name)
            .ok_or(std::io::Error::new(std::io::ErrorKind::NotFound, format!("Attribute '{name}' not found")))?
            .get_value())
    }

    fn update(&mut self) -> std::io::Result<()> {
        Ok(())
    }
    fn render(&self, fixture: &F, target: &mut F::RenderTarget<'_>) -> std::io::Result<()>;
    fn valid_for_fixture(personality: &FixturePersonality) -> bool
    where
        Self: Sized;

    fn get_attributes(&self) -> &[&str];
}
