use crate::{universes::universe_identifier::UniverseIdentifier, VenueFixture};
use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct VenueUniverse {
    #[serde(flatten)]
    pub identifier: UniverseIdentifier,
    pub fixtures: Vec<VenueFixture>,
}
