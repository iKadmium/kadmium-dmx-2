use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct MovementAxis {
    pub name: String,
    pub min_degrees: i16,
    pub max_degrees: i16,
}
