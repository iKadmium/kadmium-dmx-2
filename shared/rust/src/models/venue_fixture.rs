use serde::{Deserialize, Serialize};

use crate::{venue_neewer_fixture::VenueNeewerFixture, VenueDmxFixture};

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(tag = "type")]
#[serde(rename_all = "camelCase")]
pub enum VenueFixture {
    Dmx(VenueDmxFixture),
    Neewer(VenueNeewerFixture),
}
