use std::collections::HashMap;

use serde::{Deserialize, Serialize};

use crate::dmx_fixtures::{
    color_wheel::ColorWheel, fixture_personality::FixturePersonality, movement_axis::MovementAxis,
};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct FixtureDefinition {
    pub manufacturer: String,
    pub model: String,
    pub personalities: HashMap<String, FixturePersonality>,
    pub movement_axis: HashMap<String, MovementAxis>,
    pub color_wheel: Option<ColorWheel>,
}
