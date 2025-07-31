use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Location {
    pub address: Option<String>,
    pub city: String,
    pub state: Option<String>,
    pub country: Option<String>,
    pub postal_code: Option<String>,
}
