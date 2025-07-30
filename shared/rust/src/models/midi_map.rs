use serde::{Deserialize, Serialize};
use std::collections::HashMap;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct MidiMap {
    pub groups: HashMap<u8, String>,
    pub attributes: HashMap<u8, String>,
}

impl MidiMap {
    pub fn new() -> Self {
        Self {
            groups: HashMap::new(),
            attributes: HashMap::new(),
        }
    }

    pub fn get_group(&self, channel: u8) -> Option<&String> {
        self.groups.get(&channel)
    }

    pub fn get_attribute(&self, cc_number: u8) -> Option<&String> {
        self.attributes.get(&cc_number)
    }
}

impl Default for MidiMap {
    fn default() -> Self {
        Self::new()
    }
}

// we map MIDI channels to groups
// we map MIDI cc numbers to attributes
