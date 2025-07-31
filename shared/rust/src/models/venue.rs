use serde::{Deserialize, Serialize};

use crate::{location::Location, venue_fixture::VenueFixture};

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Venue {
    pub id: String,
    pub name: String,
    pub location: Location,
    pub capacity: Option<u32>,

    pub fixtures: Vec<VenueFixture>,
}
