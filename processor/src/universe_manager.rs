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
    DefinitionsSet, Venue,
    venue_fixture::{VenueFixture, VenueFixtureType},
};

pub struct UniverseManager {
    group_subscriptions: HashMap<String, broadcast::Sender<(String, f32)>>,
    venue: Option<Venue>,
    universes: UniverseMap,
}

type UniverseMap = HashMap<UniverseIdentifier, UniverseContainer>;

impl UniverseManager {
    pub fn new() -> Self {
        Self {
            group_subscriptions: HashMap::new(),
            venue: None,
            universes: HashMap::new(),
        }
    }

    pub fn update_venue(&mut self, venue: Venue, definitions: &DefinitionsSet) -> Result<()> {
        info!("Updating venue configuration: {}", venue.name);

        // Clear existing fixtures and groups
        self.universes.clear();

        // Create universes based on venue configuration
        self.create_universes(&venue)?;

        self.create_groups(&venue)?;

        // Create fixtures from venue configuration
        for venue_fixture in &venue.fixtures {
            self.create_fixture(venue_fixture, definitions)?;
        }

        self.venue = Some(venue);
        info!("Venue configuration updated successfully");
        Ok(())
    }

    fn create_groups(&mut self, venue: &Venue) -> Result<()> {
        info!("Creating groups from venue configuration");

        let mut unique_groups = std::collections::HashSet::new();
        for fixture in &venue.fixtures {
            for group in &fixture.common.groups {
                unique_groups.insert(group.clone());
            }
        }
        info!("Unique groups found in fixtures: {:?}", unique_groups);

        for group in &unique_groups {
            let (tx, _) = broadcast::channel(100);
            self.group_subscriptions.insert(group.clone(), tx);
        }

        Ok(())
    }

    fn create_universes(&mut self, venue: &Venue) -> Result<()> {
        // Add DMX universes (ArtNet and sACN)
        for venue_fixture in &venue.fixtures {
            let identifier = UniverseIdentifier::from_address(&venue_fixture.common.address);
            self.universes
                .entry(identifier)
                .or_insert_with(|| match identifier.universe_type {
                    UniverseType::Sacn => {
                        UniverseContainer::Dmx(DmxUniverse::new(identifier.universe_number))
                    }
                    UniverseType::ArtNet => {
                        UniverseContainer::Dmx(DmxUniverse::new(identifier.universe_number))
                    }
                    UniverseType::Neewer => UniverseContainer::Neewer(NeewerUniverse::new()),
                });
        }
        Ok(())
    }

    fn create_fixture(
        &mut self,
        venue_fixture: &VenueFixture,
        definitions: &DefinitionsSet,
    ) -> Result<()> {
        let mut senders = Vec::new();

        for group_name in &venue_fixture.common.groups {
            let subscription = self.group_subscriptions.get(group_name).ok_or_else(|| {
                std::io::Error::new(
                    std::io::ErrorKind::NotFound,
                    format!("Group '{group_name}' not found"),
                )
            })?;
            senders.push(subscription.clone());
        }

        let universe = self
            .universes
            .get_mut(&UniverseIdentifier::from_address(
                &venue_fixture.common.address,
            ))
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

                let definition = definitions
                    .get(&(dmx_config.manufacturer.clone(), dmx_config.model.clone()))
                    .ok_or_else(|| {
                        std::io::Error::new(
                            std::io::ErrorKind::NotFound,
                            format!(
                                "Fixture definition not found for: {} / {}",
                                dmx_config.manufacturer, dmx_config.model
                            ),
                        )
                    })?;

                // Create DMX fixture
                let dmx_fixture =
                    DmxFixture::new(dmx_config, &venue_fixture.common, definition, senders);

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

                let neewer_fixture = NeewerFixture::new(
                    venue_fixture.common.name.clone(),
                    venue_fixture.common.address.clone(),
                    senders,
                );

                // Add to universe
                universe.add_neewer_fixture(neewer_fixture).map_err(|e| {
                    anyhow::anyhow!("Failed to add Neewer fixture to universe: {}", e)
                })?;
            }
        }

        Ok(())
    }

    pub fn update_group_attribute(
        &self,
        group_name: &str,
        attribute: &str,
        value: f32,
    ) -> std::io::Result<()> {
        info!(
            "Updating attribute '{}' for group '{}' to: {:?}",
            attribute, group_name, value
        );

        if let Some(sender) = self.group_subscriptions.get(group_name) {
            sender
                .send((attribute.to_string(), value))
                .map_err(std::io::Error::other)?;
            info!("Attribute update sent to group '{}'", group_name);
        } else {
            warn!("Group '{}' not found for attribute update", group_name);
            return Err(std::io::Error::new(
                std::io::ErrorKind::NotFound,
                format!("Group '{group_name}' not found"),
            ));
        }

        Ok(())
    }

    pub fn universes_iter_mut(
        &mut self,
    ) -> impl Iterator<Item = (&UniverseIdentifier, &mut UniverseContainer)> {
        self.universes.iter_mut()
    }
}
