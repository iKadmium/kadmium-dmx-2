use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, PartialEq, Eq, Hash, Serialize, Deserialize)]
#[serde(tag = "type")]
#[serde(rename_all = "camelCase")]
pub enum FixtureAddress {
    Sacn { universe: u16, channel: u16 },
    ArtNet { universe: u16, channel: u16 },
    Bluetooth { uuid: String },
}
