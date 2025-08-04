use std::collections::HashMap;

use anyhow::Result;
use tokio::sync::broadcast;
use tracing::{info, warn};

use crate::{
    fixtures::{dmx_fixture::DmxFixture, neewer_fixture::NeewerFixture},
    universes::{
        dmx_universe::DmxUniverse,
        neewer_universe::NeewerUniverse,
        universe_container::UniverseContainer,
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
    universes: UniverseMap,
}

type UniverseMap = HashMap<UniverseIdentifier, UniverseContainer>;

impl UniverseManager {
    pub fn new() -> Self {
        Self {
            attribute_channels: HashMap::new(),
            midi_map: None,
            venue: None,
            universes: HashMap::new(),
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
        self.universes.clear();

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
        // Add DMX universes (ArtNet and sACN)
        for venue_fixture in &venue.fixtures {
            let identifier = UniverseIdentifier::from_address(&venue_fixture.common.address);
            self.universes.entry(identifier).or_insert_with(|| match identifier.universe_type {
                UniverseType::Sacn => UniverseContainer::Dmx(DmxUniverse::new(identifier.universe_number)),
                UniverseType::ArtNet => UniverseContainer::Dmx(DmxUniverse::new(identifier.universe_number)),
                UniverseType::Neewer => UniverseContainer::Neewer(NeewerUniverse::new()),
            });
        }
        Ok(())
    }

    fn create_fixture(&mut self, venue_fixture: &VenueFixture, definitions: &DefinitionsSet) -> Result<()> {
        let universe = self
            .universes
            .get_mut(&UniverseIdentifier::from_address(&venue_fixture.common.address))
            .ok_or(std::io::Error::new(
                std::io::ErrorKind::NotFound,
                format!("DMX universe not found: {:?}", venue_fixture.common.address),
            ))?;

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

                // Add to universe
                universe
                    .add_dmx_fixture(dmx_fixture)
                    .map_err(|e| anyhow::anyhow!("Failed to add DMX fixture to universe: {}", e))?;
            }
            VenueFixtureType::Neewer => {
                info!(
                    "Creating Neewer fixture '{}' at address {:?}",
                    venue_fixture.common.name, venue_fixture.common.address
                );

                // Create Neewer fixture
                let neewer_fixture = NeewerFixture::new(venue_fixture.common.name.clone(), venue_fixture.common.address.clone());

                // Add to universe
                universe
                    .add_neewer_fixture(neewer_fixture)
                    .map_err(|e| anyhow::anyhow!("Failed to add Neewer fixture to universe: {}", e))?;
            }
        }

        Ok(())
    }

    fn update_all_fixture_subscriptions(&mut self) {
        info!("Updating subscriptions for all existing fixtures");

        if let Some(venue) = &self.venue {
            for venue_fixture in &venue.fixtures {
                if let Some(universe) = self.universes.get_mut(&UniverseIdentifier::from_address(&venue_fixture.common.address)) {
                    // Create new attribute receivers for this fixture's groups
                    let mut attribute_receivers = HashMap::new();
                    for group_name in &venue_fixture.common.groups {
                        if let Some(group_channels) = self.attribute_channels.get(group_name) {
                            for (attribute_name, sender) in group_channels {
                                let receiver = sender.subscribe();
                                attribute_receivers.insert(attribute_name.clone(), receiver);
                            }
                        }
                    }

                    // Update the fixture's subscriptions
                    universe.update_fixture_subscriptions(&venue_fixture.common.name, attribute_receivers);
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

    pub fn universes_iter_mut(&mut self) -> impl Iterator<Item = (&UniverseIdentifier, &mut UniverseContainer)> {
        self.universes.iter_mut()
    }
}
