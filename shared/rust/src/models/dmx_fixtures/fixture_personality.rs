use std::collections::HashMap;

use serde::{Deserialize, Serialize};

use crate::dmx_fixtures::channel::Channel;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct FixturePersonality {
    pub name: String,
    pub channels: HashMap<String, Channel>,
}

impl FixturePersonality {
    pub fn new(name: String, channels: HashMap<String, Channel>) -> Self {
        FixturePersonality { name, channels }
    }
}
