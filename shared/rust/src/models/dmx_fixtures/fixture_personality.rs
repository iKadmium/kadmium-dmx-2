use std::collections::HashMap;

use crate::dmx_fixtures::channel::Channel;

pub struct FixturePersonality {
    pub name: String,
    pub channels: HashMap<u16, Channel>,
}
