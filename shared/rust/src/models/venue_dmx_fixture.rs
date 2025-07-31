use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct VenueDmxFixture {
    pub manufacturer: String,
    pub model: String,
    pub personality: String,
}
