use tokio::sync::watch;

#[derive(Debug)]
pub struct Attribute {
    pub name: String,
    rx: watch::Receiver<f32>,
    tx: watch::Sender<f32>,
}

impl Attribute {
    pub fn new(name: String, value: f32) -> Self {
        let (tx, rx) = watch::channel(value);
        Attribute { name, rx, tx }
    }

    pub fn set_value(&self, value: f32) -> Result<(), watch::error::SendError<f32>> {
        self.tx.send(value)
    }

    pub fn get_value(&self) -> f32 {
        *self.rx.borrow()
    }
}
