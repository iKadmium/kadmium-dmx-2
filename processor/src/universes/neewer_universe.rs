use bytes::BytesMut;
use kadmium_dmx_shared::{Message, NeewerLightParams, VenueFixtureCommon};
use rumqttc::{ClientError, QoS};

use crate::fixtures::{
    fixture::Fixture,
    neewer_fixture::NeewerFixture,
};
use crate::universes::universe::Universe;

#[derive(Debug)]
pub struct NeewerUniverse {
    pub update_message: NeewerLightParams,
    pub fixtures: Vec<NeewerFixture>,
    topic: String,
    message: BytesMut,
}

impl NeewerUniverse {
    pub fn new(address: String, fixture: VenueFixtureCommon) -> Self {
        let topic = format!("bt/neewer/update/{address}");
        NeewerUniverse {
            update_message: NeewerLightParams::default(),
            fixtures: vec![NeewerFixture::new(fixture)],
            topic,
            message: BytesMut::new(),
        }
    }
}

impl Universe for NeewerUniverse {
    type FixtureType = NeewerFixture;

    fn fixtures_mut(&mut self) -> &mut Vec<Self::FixtureType> {
        &mut self.fixtures
    }

    fn render(&mut self) -> std::io::Result<()> {
        self.message.clear();
        for fixture in &mut self.fixtures {
            fixture.render(&mut self.update_message)?;
        }
        self.update_message.encode(&mut self.message)?;
        Ok(())
    }

    async fn send_update(&self, mqtt_client: &rumqttc::AsyncClient) -> Result<(), ClientError> {
        mqtt_client
            .publish_bytes(self.topic.clone(), QoS::AtLeastOnce, false, self.message.clone().freeze())
            .await
    }
}
