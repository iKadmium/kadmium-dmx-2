use crate::hsv_color::HsvColor;

pub struct ColorWheel {
    pub sections: Vec<ColorWheelSection>,
}

pub struct ColorWheelSection {
    pub name: String,
    pub start: u16,
    pub end: u16,
    pub color: HsvColor,
}
