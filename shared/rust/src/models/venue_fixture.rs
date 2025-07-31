use serde::{Deserialize, Serialize};

use crate::{FixtureAddress, VenueDmxFixture};

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct CommonFixtureProperties {
    pub name: String,
    pub address: FixtureAddress,
    pub groups: Vec<String>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(tag = "type")]
#[serde(rename_all = "camelCase")]
pub struct VenueFixture {
    #[serde(flatten)]
    pub common: CommonFixtureProperties,
    #[serde(flatten)]
    pub fixture_type: VenueFixtureType,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(tag = "type")]
#[serde(rename_all = "camelCase")]
pub enum VenueFixtureType {
    Dmx(VenueDmxFixture),
    Neewer,
}
