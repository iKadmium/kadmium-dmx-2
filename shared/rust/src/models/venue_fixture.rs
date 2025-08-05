use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct VenueFixtureCommon {
    pub name: String,
    pub groups: Vec<String>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct VenueDmxFixture {
    #[serde(flatten)]
    pub common: VenueFixtureCommon,

    pub manufacturer: String,
    pub model: String,
    pub personality: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase", tag = "type")]
pub enum VenueFixture {
    Dmx(VenueDmxFixture),
    Neewer(VenueFixtureCommon),
}
