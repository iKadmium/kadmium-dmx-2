use kadmium_dmx_shared::NeewerLightParams;

#[derive(Debug, Clone, Default)]
pub struct NeewerColor {
    pub hue: u16,
    pub saturation: u8,
    pub brightness: u8,
}

impl PartialEq<NeewerLightParams> for NeewerColor {
    fn eq(&self, other: &NeewerLightParams) -> bool {
        self.brightness == (other.brightness as u8)
            && self.hue == (other.hue as u16)
            && self.saturation == (other.saturation as u8)
    }
}

impl From<NeewerLightParams> for NeewerColor {
    fn from(value: NeewerLightParams) -> Self {
        Self {
            brightness: value.brightness as u8,
            hue: value.hue as u16,
            saturation: value.saturation as u8,
        }
    }
}
