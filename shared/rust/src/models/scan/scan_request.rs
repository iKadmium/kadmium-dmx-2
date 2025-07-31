use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ScanRequest {
    pub timestamp: u64, // Unix timestamp in seconds
}
