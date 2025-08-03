use std::{collections::HashMap, fmt::Debug};

use crate::{effects::attribute::Attribute, fixtures::fixture::Fixture};

pub trait Effect<F: Fixture>: Send + Sync + Debug {
    fn get_attribute_value(
        &self,
        attributes: &HashMap<String, Attribute>,
        name: &str,
    ) -> std::io::Result<f32> {
        Ok(attributes
            .get(name)
            .ok_or(std::io::Error::new(
                std::io::ErrorKind::NotFound,
                format!("Attribute '{name}' not found"),
            ))?
            .get_value())
    }

    fn apply(&self, fixture: &F, target: &mut F::RenderTarget<'_>) -> std::io::Result<()>;

    fn get_attributes(&self) -> &[&str];
}

// pub fn get_effects_for_neewer_fixture()
// -> Vec<Box<dyn Effect<RenderTarget = NeewerLightParams> + Send + Sync>> {
//     let effects: Vec<Box<dyn Effect<RenderTarget = NeewerLightParams> + Send + Sync>> =
//         vec![Box::new(NeewerHsvToHsv {})];
//     effects
// }
