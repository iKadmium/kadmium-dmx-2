use serde::{Serialize, de::DeserializeOwned};
use std::{io::Result, path::PathBuf};
use tokio::{fs::File, io::AsyncReadExt};

const ROOT_DIR: &str = "data";

pub(super) async fn get_json(path: &PathBuf) -> Result<String> {
    let path = PathBuf::from(ROOT_DIR).join(path);
    let mut buffer = String::new();
    let mut result = File::open(&path).await?;
    result.read_to_string(&mut buffer).await?;
    Ok(buffer)
}

pub(super) async fn save_json(path: &PathBuf, contents: &str) -> Result<()> {
    let path = PathBuf::from(ROOT_DIR).join(path);
    // Ensure the data directory exists
    if let Some(parent) = path.parent() {
        tokio::fs::create_dir_all(parent).await?;
    }
    tokio::fs::write(path, contents).await?;
    Ok(())
}

pub trait StoredInJsonFile<IdType>: Sized + Clone + DeserializeOwned + Serialize {
    fn path(id: IdType) -> PathBuf;
    fn id(&self) -> IdType;

    async fn load(id: IdType) -> Result<Self> {
        let contents = get_json(&Self::path(id)).await?;
        let item: Self = serde_json::from_str(&contents)?;
        Ok(item)
    }

    async fn save(&self) -> Result<()> {
        let contents = serde_json::to_string(self)?;
        save_json(&Self::path(self.id()), contents.as_str()).await?;
        Ok(())
    }
}
