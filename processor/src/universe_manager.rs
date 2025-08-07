use std::collections::HashMap;

use anyhow::{Result, anyhow};
use tokio::sync::watch;
use tracing::{debug, info, warn};

use crate::fixtures::fixture::FixtureAccessors;
use crate::universes::{dmx_universe::DmxUniverse, neewer_universe::NeewerUniverse, universe::Universe};
use kadmium_dmx_shared::{DefinitionsSet, MidiMap, Venue, universes::universe_identifier::UniverseIdentifier, venue_fixture::VenueFixture};

pub struct UniverseManager {
    // Map from group -> (attribute -> Vec of attribute watch senders to update)
    attribute_channels: HashMap<String, HashMap<String, Vec<watch::Sender<f32>>>>,
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
                // Initialize with empty vec - will be populated when venues are configured
                group_channels.insert(attribute_name.clone(), Vec::new());
                debug!("Created channel for group '{}', attribute '{}'", group_name, attribute_name);
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

    pub fn update_venue(&mut self, venue: Venue, definitions: DefinitionsSet) -> Result<()> {
        info!("Updating venue configuration: {}", &venue.name);

        // Clear existing fixtures and groups
        self.dmx_universes.clear();
        self.neewer_universes.clear();

        // Create fixtures from venue configuration
        for universe in &venue.universes {
            match &universe.identifier {
                UniverseIdentifier::ArtNet { universe_id } | UniverseIdentifier::Sacn { universe_id } => {
                    let dmx_fixtures = universe
                        .fixtures
                        .iter()
                        .filter_map(|fixture| match fixture {
                            VenueFixture::Dmx(dmx_fixture) => Some(dmx_fixture.clone()),
                            _ => None,
                        })
                        .collect();
                    self.dmx_universes
                        .entry(universe.identifier.clone())
                        .or_insert(DmxUniverse::new(*universe_id, dmx_fixtures, &definitions)?);
                }
                UniverseIdentifier::Neewer { address } => {
                    if universe.fixtures.len() != 1 {
                        return Err(anyhow!("Wrong number of fixtures for Neewer universe"));
                    }
                    if let Some(VenueFixture::Neewer(neewer_fixture)) = universe.fixtures.first() {
                        self.neewer_universes
                            .entry(universe.identifier.clone())
                            .or_insert(NeewerUniverse::new(address.clone(), neewer_fixture.clone()));
                    }
                }
            }
        }

        self.venue = Some(venue);

        // Update subscriptions for all fixtures if we have MIDI map channels
        if !self.attribute_channels.is_empty() {
            self.update_all_fixture_subscriptions();
        }

        info!("Venue configuration updated successfully");
        Ok(())
    }

    fn update_all_fixture_subscriptions(&mut self) {
        info!("Collecting fixture attribute senders for group control");

        // Clear existing attribute channels
        for group_channels in self.attribute_channels.values_mut() {
            for senders in group_channels.values_mut() {
                senders.clear();
            }
        }

        if let Some(_venue) = &self.venue {
            // Collect attribute senders from all DMX fixtures
            for (universe_id, universe) in &mut self.dmx_universes {
                info!("Processing DMX universe: {:?}", universe_id);
                for fixture in universe.fixtures_mut() {
                    for group_name in &fixture.groups {
                        if let Some(group_channels) = self.attribute_channels.get_mut(group_name) {
                            for attribute_name in fixture.attributes().keys() {
                                if let Some(senders) = group_channels.get_mut(attribute_name) {
                                    if let Some(attribute) = fixture.attributes().get(attribute_name) {
                                        // Get a clone of the attribute's sender to add to the group
                                        let sender = attribute.get_sender();
                                        senders.push(sender);
                                        debug!("Added attribute '{}' from fixture '{}' to group '{}'", attribute_name, fixture.name, group_name);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Collect attribute senders from all Neewer fixtures
            for (universe_id, universe) in &mut self.neewer_universes {
                info!("Processing Neewer universe: {:?}", universe_id);
                for fixture in universe.fixtures_mut() {
                    for group_name in &fixture.groups {
                        if let Some(group_channels) = self.attribute_channels.get_mut(group_name) {
                            for attribute_name in fixture.attributes().keys() {
                                if let Some(senders) = group_channels.get_mut(attribute_name) {
                                    if let Some(attribute) = fixture.attributes().get(attribute_name) {
                                        // Get a clone of the attribute's sender to add to the group
                                        let sender = attribute.get_sender();
                                        senders.push(sender);
                                        debug!("Added attribute '{}' from fixture '{}' to group '{}'", attribute_name, fixture.name, group_name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        info!("Fixture attribute sender collection complete");
    }

    pub fn update_group_attribute(&self, group_name: &str, attribute: &str, value: f32) -> std::io::Result<()> {
        debug!("Updating attribute '{}' for group '{}' to: {:?}", attribute, group_name, value);

        // Look for the specific group, then attribute channel
        if let Some(group_channels) = self.attribute_channels.get(group_name) {
            if let Some(senders) = group_channels.get(attribute) {
                let mut updated_count = 0;
                for sender in senders {
                    if sender.send(value).is_ok() {
                        updated_count += 1;
                    }
                }
                debug!(
                    "Attribute update sent to {} fixtures in group '{}', attribute '{}'",
                    updated_count, group_name, attribute
                );
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
