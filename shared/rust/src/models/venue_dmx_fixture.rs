use crate::fixture_address::FixtureAddress;
use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct VenueDmxFixture {
    pub name: String,
    pub manufacturer: String,
    pub model: String,
    pub personality: String,
    pub address: FixtureAddress,
}
