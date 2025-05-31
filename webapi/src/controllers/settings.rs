use std::sync::Arc;

use axum::{Json, Router, extract::State, http::StatusCode, response::IntoResponse, routing::get};
use tokio::sync::RwLock;

use crate::{Settings, data_access::json_file::StoredInJsonFile};

pub fn settings_controller(settings: Arc<RwLock<Settings>>) -> Router {
    Router::new()
        .route("/", get(get_settings).put(update_settings))
        .with_state(settings)
}

async fn get_settings(State(settings_state): State<Arc<RwLock<Settings>>>) -> Json<Settings> {
    // Changed to Arc<RwLock<Settings>>
    let settings_read_guard = settings_state.read().await;
    Json(settings_read_guard.clone())
}

async fn update_settings(
    State(settings_state): State<Arc<RwLock<Settings>>>, // Added State extractor for settings_state
    Json(new_settings): Json<Settings>,
) -> impl IntoResponse {
    match new_settings.save().await {
        Ok(_) => {
            let mut settings_write_guard = settings_state.write().await; // Acquire write lock
            *settings_write_guard = new_settings; // Update the in-memory settings
            StatusCode::OK
        }
        Err(e) => {
            tracing::error!("Failed to save settings: {}", e);
            StatusCode::INTERNAL_SERVER_ERROR
        }
    }
}
