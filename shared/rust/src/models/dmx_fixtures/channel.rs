use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Channel {
    pub name: String,
    pub address: u16,
}

impl Channel {
    pub fn new(name: String, address: u16) -> Self {
        Channel { name, address }
    }
}
