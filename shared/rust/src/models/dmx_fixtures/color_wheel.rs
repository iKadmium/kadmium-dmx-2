use serde::{Deserialize, Serialize};

use crate::hsv_color::HsvColor;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ColorWheel {
    pub sections: Vec<ColorWheelSection>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ColorWheelSection {
    pub name: String,
    pub start: u16,
    pub end: u16,
    pub color: HsvColor,
}
