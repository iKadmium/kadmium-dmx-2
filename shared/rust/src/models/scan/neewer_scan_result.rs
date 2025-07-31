use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct NeewerScanResult {
    pub timestamp: u64, // Unix timestamp in seconds
    pub devices: Vec<NeewerScanItem>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct NeewerScanItem {
    pub local_name: String,
    pub address: String,
}
