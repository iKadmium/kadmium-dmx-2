# Multi-stage build for Neewer Renderer
# Stage 1: Build environment
FROM rust:bookworm AS builder

# Install tools for dynamic linking and cross-compilation dependencies
# TODO: Remove protobuf-compiler install step - it's going away later
RUN apt-get update && apt-get install -y \
    protobuf-compiler \
    libdbus-1-dev \
    pkg-config \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy the shared library first (for better caching)
COPY shared/ ./shared/

# Copy Cargo files
COPY renderer/neewer/Cargo.toml renderer/neewer/Cargo.lock ./renderer/neewer/

# Create src directory structure
RUN mkdir -p renderer/neewer/src

# Create a dummy main.rs to build dependencies (for better caching)
# RUN echo "fn main() {}" > renderer/neewer/src/main.rs

# Build dependencies (from the neewer subdirectory)
WORKDIR /app/renderer/neewer
# RUN cargo build --release

# Copy the actual source code (this will invalidate the cache)
COPY renderer/neewer/src/ ./src/

# Remove the dummy binary and rebuild with real source
RUN rm -f target/release/neewer-renderer && \
    cargo build --release

# Stage 2: Runtime image (Debian slim - reliable glibc compatibility)
FROM debian:bookworm-slim
RUN apt-get update && apt-get install -y \
    libdbus-1-3 \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean

COPY --from=builder /app/renderer/neewer/target/release/neewer-renderer /neewer-renderer
RUN chmod +x /neewer-renderer

# Run as root for now to avoid D-Bus permission issues
ENTRYPOINT ["/neewer-renderer"]
