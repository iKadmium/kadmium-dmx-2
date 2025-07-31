use serde::{Deserialize, Serialize};

use crate::Venue;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct VenueUpdate {
    pub venue: Venue,
}
