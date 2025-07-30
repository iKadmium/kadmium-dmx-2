use axum::Router;
use axum::routing::get_service;
use std::sync::Arc;
use tokio::sync::RwLock;
use tower_http::{
    services::{ServeDir, ServeFile},
    trace::{DefaultMakeSpan, DefaultOnRequest, TraceLayer},
};
use tracing::Level;

use crate::controllers::settings::settings_controller;
use crate::data_access::json_file::StoredInJsonFile;
use crate::models::settings::Settings;

const SPA_DIR: &str = "assets";

/// Sets up and configures the web server with all routes and middleware
pub async fn setup_web_server() -> Result<Router, Box<dyn std::error::Error + Send + Sync>> {
    // Load settings
    let initial_settings = match Settings::load(()).await {
        Ok(s) => s,
        Err(e) => {
            eprintln!("Failed to load settings: {e}. Using default settings.");
            Settings::default()
        }
    };
    let settings_state = Arc::new(RwLock::new(initial_settings));

    // Create a service to serve the index.html file
    let index_html_service = ServeFile::new(format!("{SPA_DIR}/index.html"));

    // Create the static file service with a fallback to index.html
    let static_service = get_service(ServeDir::new(SPA_DIR).fallback(index_html_service));

    let api_router = Router::new()
        .nest(
            "/api/settings",
            settings_controller(settings_state.clone()),
        )
        .layer(
            TraceLayer::new_for_http()
                .make_span_with(DefaultMakeSpan::new())
                .on_request(DefaultOnRequest::new().level(Level::INFO))
                .on_response(|response: &axum::http::Response<_>, latency: std::time::Duration, _span: &tracing::Span| {
                    let status = response.status();
                    if status.is_client_error() || status.is_server_error() {
                        tracing::warn!(latency = ?latency, status = %status, "response finished");
                    } else {
                        tracing::info!(latency = ?latency, status = %status, "response finished");
                    }
                })
        );

    let app = Router::new().merge(api_router).fallback(static_service);
    Ok(app)
}

/// Starts the web server on the specified address
pub async fn start_server(app: Router, addr: &str) -> Result<(), Box<dyn std::error::Error + Send + Sync>> {
    let listener = tokio::net::TcpListener::bind(addr).await?;
    tracing::info!(addr = %listener.local_addr()?, "Server running");
    axum::serve(listener, app).await?;
    Ok(())
}
