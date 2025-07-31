pub struct HsvColor {
    pub hue: f32,        // 0.0 to 360.0
    pub saturation: f32, // 0.0 to 1.0
    pub value: f32,      // 0.0 to 1.0
}
impl HsvColor {
    pub fn new(hue: f32, saturation: f32, value: f32) -> Self {
        Self {
            hue,
            saturation,
            value,
        }
    }

    pub fn to_rgb(&self) -> (u8, u8, u8) {
        let c = self.value * self.saturation;
        let x = c * (1.0 - ((self.hue / 60.0) % 2.0 - 1.0).abs());
        let m = self.value - c;

        let (r, g, b) = if self.hue < 60.0 {
            (c, x, 0.0)
        } else if self.hue < 120.0 {
            (x, c, 0.0)
        } else if self.hue < 180.0 {
            (0.0, c, x)
        } else if self.hue < 240.0 {
            (0.0, x, c)
        } else if self.hue < 300.0 {
            (x, 0.0, c)
        } else {
            (c, 0.0, x)
        };

        (
            ((r + m) * 255.0) as u8,
            ((g + m) * 255.0) as u8,
            ((b + m) * 255.0) as u8,
        )
    }
}
