use crate::{
    fixtures::{dmx_fixture::DmxFixture, neewer_fixture::NeewerFixture},
    universes::{dmx_universe::DmxUniverse, neewer_universe::NeewerUniverse},
};
use bytes::BytesMut;
use std::collections::HashMap;
use tokio::sync::broadcast;

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

    pub fn update_fixture_subscriptions(&mut self, fixture_name: &str, attribute_receivers: HashMap<String, broadcast::Receiver<f32>>) {
        match self {
            UniverseContainer::Dmx(dmx) => {
                dmx.update_fixture_subscriptions(fixture_name, attribute_receivers);
            }
            UniverseContainer::Neewer(neewer) => {
                neewer.update_fixture_subscriptions(fixture_name, attribute_receivers);
            }
        }
    }

    pub fn update(&mut self) -> std::io::Result<()> {
        match self {
            UniverseContainer::Dmx(dmx) => {
                dmx.update()?;
                Ok(())
            }
            UniverseContainer::Neewer(neewer) => {
                neewer.update()?;
                Ok(())
            }
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
