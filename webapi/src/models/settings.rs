use crate::data_access::json_file::StoredInJsonFile;

#[derive(Debug, Clone, Default, PartialEq, Eq, serde::Serialize, serde::Deserialize)]
pub(crate) struct Settings {}

impl StoredInJsonFile<()> for Settings {
    fn path(_: ()) -> std::path::PathBuf {
        std::path::PathBuf::from("settings.json")
    }

    fn id(&self) {}
}
