use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, PartialEq, Eq, Hash, Serialize, Deserialize)]
pub enum FixtureAddress {
    Sacn { universe: u16, channel: u16 },
    ArtNet { universe: u16, channel: u16 },
    Bluetooth { uuid: String },
}
