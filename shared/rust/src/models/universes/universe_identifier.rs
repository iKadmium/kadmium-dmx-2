use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug, Clone, Hash, PartialEq, Eq)]
#[serde(rename_all = "camelCase", tag = "type")]
pub enum UniverseIdentifier {
    Sacn { universe_id: u16 },
    ArtNet { universe_id: u16 },
    Neewer { address: String },
}
