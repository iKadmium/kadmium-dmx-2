use std::collections::HashMap;

use serde::{Deserialize, Serialize};

use crate::{dmx_fixtures::fixture_definition::FixtureDefinition, Venue};

pub type DefinitionsSet = HashMap<(String, String), FixtureDefinition>;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct VenueUpdate {
    pub venue: Venue,
    pub definitions: DefinitionsSet,
}
