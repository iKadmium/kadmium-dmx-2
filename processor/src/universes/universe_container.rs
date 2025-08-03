use crate::{
    fixtures::{dmx_fixture::DmxFixture, neewer_fixture::NeewerFixture},
    universes::{dmx_universe::DmxUniverse, neewer_universe::NeewerUniverse},
};
use bytes::BytesMut;

/// Type-safe container for different universe types
#[derive(Debug)]
pub enum UniverseContainer {
    Dmx(DmxUniverse),
    Neewer(NeewerUniverse),
}

impl UniverseContainer {
    /// Add a Neewer fixture (only works if this is a Neewer universe)
    pub fn add_neewer_fixture(&mut self, fixture: NeewerFixture) -> Result<(), &'static str> {
        match self {
            UniverseContainer::Neewer(neewer) => {
                neewer.add_neewer_fixture(fixture);
                Ok(())
            }
            _ => Err("Cannot add Neewer fixture to non-Neewer universe"),
        }
    }

    pub fn add_dmx_fixture(&mut self, fixture: DmxFixture) -> Result<(), &'static str> {
        match self {
            UniverseContainer::Dmx(dmx) => {
                dmx.add_dmx_fixture(fixture);
                Ok(())
            }
            _ => Err("Cannot add DMX fixture to non-DMX universe"),
        }
    }

    pub fn render(&mut self) -> std::io::Result<()> {
        match self {
            UniverseContainer::Dmx(dmx) => {
                dmx.render();
                Ok(())
            }
            UniverseContainer::Neewer(neewer) => {
                neewer.render();
                Ok(())
            }
        }
    }

    pub fn get_update(&self, buf: &mut BytesMut) -> std::io::Result<String> {
        match self {
            UniverseContainer::Dmx(dmx) => dmx.get_update(buf),
            UniverseContainer::Neewer(neewer) => neewer.get_update(buf),
        }
    }
}
