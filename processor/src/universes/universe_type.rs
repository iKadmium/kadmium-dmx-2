use kadmium_dmx_shared::FixtureAddress;

#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub enum UniverseType {
    Sacn,
    ArtNet,
    Neewer,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub struct UniverseIdentifier {
    pub universe_type: UniverseType,
    pub universe_number: u16,
}

impl UniverseIdentifier {
    pub fn new(universe_type: UniverseType, universe_number: u16) -> Self {
        UniverseIdentifier {
            universe_type,
            universe_number,
        }
    }

    pub fn from_address(address: &FixtureAddress) -> Self {
        match address {
            FixtureAddress::Sacn { universe, .. } => {
                UniverseIdentifier::new(UniverseType::Sacn, *universe)
            }
            FixtureAddress::ArtNet { universe, .. } => {
                UniverseIdentifier::new(UniverseType::ArtNet, *universe)
            }
            FixtureAddress::Bluetooth { .. } => UniverseIdentifier::new(UniverseType::Neewer, 0),
        }
    }
}
