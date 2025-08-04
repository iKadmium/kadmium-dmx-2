use std::collections::HashMap;

use anyhow::Result;
use tokio::sync::broadcast;
use tracing::{info, warn};

use crate::{
    fixtures::{dmx_fixture::DmxFixture, neewer_fixture::NeewerFixture},
    universes::{
        dmx_universe::DmxUniverse,
        neewer_universe::NeewerUniverse,
        universe::Universe,
        universe_type::{UniverseIdentifier, UniverseType},
    },
};
use kadmium_dmx_shared::{
    DefinitionsSet, MidiMap, Venue,
    venue_fixture::{VenueFixture, VenueFixtureType},
};

pub struct UniverseManager {
    // Map from group -> (attribute -> broadcast sender)
    attribute_channels: HashMap<String, HashMap<String, broadcast::Sender<f32>>>,
    midi_map: Option<MidiMap>,
    venue: Option<Venue>,
    dmx_universes: HashMap<UniverseIdentifier, DmxUniverse>,
    neewer_universes: HashMap<UniverseIdentifier, NeewerUniverse>,
}

impl UniverseManager {
    pub fn new() -> Self {
        Self {
            attribute_channels: HashMap::new(),
            midi_map: None,
            venue: None,
            dmx_universes: HashMap::new(),
            neewer_universes: HashMap::new(),
        }
    }

    pub fn update_midi_map(&mut self, midi_map: MidiMap) -> Result<()> {
        info!("Updating MIDI map configuration");

        // Clear existing channels
        self.attribute_channels.clear();

        // Create channels for all (group, attribute) combinations in the MIDI map
        for group_name in midi_map.groups.values() {
            let mut group_channels = HashMap::new();

            for attribute_name in midi_map.attributes.values() {
                let (tx, _) = broadcast::channel(100);
                group_channels.insert(attribute_name.clone(), tx);
                info!("Created channel for group '{}', attribute '{}'", group_name, attribute_name);
            }

            self.attribute_channels.insert(group_name.clone(), group_channels);
        }

        self.midi_map = Some(midi_map);

        // Update subscriptions for existing fixtures if we have a venue
        if self.venue.is_some() {
            self.update_all_fixture_subscriptions();
        }

        info!("MIDI map configuration updated successfully");
        Ok(())
    }

    pub fn update_venue(&mut self, venue: Venue, definitions: &DefinitionsSet) -> Result<()> {
        info!("Updating venue configuration: {}", venue.name);

        // Clear existing fixtures and groups
        self.dmx_universes.clear();
        self.neewer_universes.clear();

        // Create universes based on venue configuration
        self.create_universes(&venue)?;

        // Create fixtures from venue configuration
        for venue_fixture in &venue.fixtures {
            self.create_fixture(venue_fixture, definitions)?;
        }

        self.venue = Some(venue);

        // Update subscriptions for all fixtures if we have MIDI map channels
        if !self.attribute_channels.is_empty() {
            self.update_all_fixture_subscriptions();
        }

        info!("Venue configuration updated successfully");
        Ok(())
    }

    fn create_universes(&mut self, venue: &Venue) -> Result<()> {
        // Add DMX universes (ArtNet and sACN) and Neewer universes
        for venue_fixture in &venue.fixtures {
            let identifier = UniverseIdentifier::from_address(&venue_fixture.common.address);
            match identifier.universe_type {
                UniverseType::Sacn | UniverseType::ArtNet => {
                    self.dmx_universes
                        .entry(identifier)
                        .or_insert_with(|| DmxUniverse::new(identifier.universe_number));
                }
                UniverseType::Neewer => {
                    self.neewer_universes.entry(identifier).or_insert_with(NeewerUniverse::new);
                }
            }
        }
        Ok(())
    }

    fn create_fixture(&mut self, venue_fixture: &VenueFixture, definitions: &DefinitionsSet) -> Result<()> {
        let identifier = UniverseIdentifier::from_address(&venue_fixture.common.address);

        match &venue_fixture.fixture_type {
            VenueFixtureType::Dmx(dmx_config) => {
                info!(
                    "Creating DMX fixture '{}' at address {:?}",
                    venue_fixture.common.name, venue_fixture.common.address
                );

                let definition = definitions.get(&(dmx_config.manufacturer.clone(), dmx_config.model.clone())).ok_or_else(|| {
                    std::io::Error::new(
                        std::io::ErrorKind::NotFound,
                        format!("Fixture definition not found for: {} / {}", dmx_config.manufacturer, dmx_config.model),
                    )
                })?;

                // Create DMX fixture
                let dmx_fixture = DmxFixture::new(dmx_config, &venue_fixture.common, definition);

                // Add to DMX universe
                let universe = self.dmx_universes.get_mut(&identifier).ok_or(std::io::Error::new(
                    std::io::ErrorKind::NotFound,
                    format!("DMX universe not found: {:?}", venue_fixture.common.address),
                ))?;

                let groups = dmx_fixture.groups.clone();
                universe.add_fixture(dmx_fixture, groups);
            }
            VenueFixtureType::Neewer => {
                info!(
                    "Creating Neewer fixture '{}' at address {:?}",
                    venue_fixture.common.name, venue_fixture.common.address
                );

                // Create Neewer fixture
                let neewer_fixture = NeewerFixture::new(venue_fixture.common.clone());

                // Add to Neewer universe
                let universe = self.neewer_universes.get_mut(&identifier).ok_or(std::io::Error::new(
                    std::io::ErrorKind::NotFound,
                    format!("Neewer universe not found: {:?}", venue_fixture.common.address),
                ))?;

                let groups = neewer_fixture.groups.clone();
                universe.add_fixture(neewer_fixture, groups);
            }
        }

        Ok(())
    }

    fn update_all_fixture_subscriptions(&mut self) {
        info!("Updating subscriptions for all existing fixtures");

        if let Some(venue) = &self.venue {
            // Group fixtures by universe and collect all groups for each universe
            let mut dmx_universe_groups: HashMap<UniverseIdentifier, Vec<String>> = HashMap::new();
            let mut neewer_universe_groups: HashMap<UniverseIdentifier, Vec<String>> = HashMap::new();

            for venue_fixture in &venue.fixtures {
                let identifier = UniverseIdentifier::from_address(&venue_fixture.common.address);

                match identifier.universe_type {
                    UniverseType::Sacn | UniverseType::ArtNet => {
                        let groups = dmx_universe_groups.entry(identifier).or_default();
                        for group_name in &venue_fixture.common.groups {
                            if !groups.contains(group_name) {
                                groups.push(group_name.clone());
                            }
                        }
                    }
                    UniverseType::Neewer => {
                        let groups = neewer_universe_groups.entry(identifier).or_default();
                        for group_name in &venue_fixture.common.groups {
                            if !groups.contains(group_name) {
                                groups.push(group_name.clone());
                            }
                        }
                    }
                }
            }

            // Update subscriptions for each DMX universe
            for (universe_id, _groups) in dmx_universe_groups {
                if let Some(universe) = self.dmx_universes.get_mut(&universe_id) {
                    universe.update_all_fixture_subscriptions(&self.attribute_channels);
                }
            }

            // Update subscriptions for each Neewer universe
            for (universe_id, _groups) in neewer_universe_groups {
                if let Some(universe) = self.neewer_universes.get_mut(&universe_id) {
                    universe.update_all_fixture_subscriptions(&self.attribute_channels);
                }
            }
        }
    }

    pub fn update_group_attribute(&self, group_name: &str, attribute: &str, value: f32) -> std::io::Result<()> {
        info!("Updating attribute '{}' for group '{}' to: {:?}", attribute, group_name, value);

        // Look for the specific group, then attribute channel
        if let Some(group_channels) = self.attribute_channels.get(group_name) {
            if let Some(sender) = group_channels.get(attribute) {
                sender.send(value).map_err(std::io::Error::other)?;
                info!("Attribute update sent to group '{}', attribute '{}'", group_name, attribute);
                return Ok(());
            }
        }

        warn!("Channel for group '{}', attribute '{}' not found", group_name, attribute);
        Err(std::io::Error::new(
            std::io::ErrorKind::NotFound,
            format!("Channel for group '{group_name}', attribute '{attribute}' not found"),
        ))
    }

    pub fn dmx_universes_iter_mut(&mut self) -> impl Iterator<Item = (&UniverseIdentifier, &mut DmxUniverse)> {
        self.dmx_universes.iter_mut()
    }

    pub fn neewer_universes_iter_mut(&mut self) -> impl Iterator<Item = (&UniverseIdentifier, &mut NeewerUniverse)> {
        self.neewer_universes.iter_mut()
    }
}
