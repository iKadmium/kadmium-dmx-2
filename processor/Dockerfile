# Multi-stage build for RTP MIDI Source
# Stage 1: Build environment
FROM rust:bookworm AS builder

# Install musl tools for static linking and cross-compilation dependencies
# TODO: Remove protobuf-compiler install step - it's going away later
RUN apt-get update && apt-get install -y \
    musl-tools \
    musl-dev \
    gcc-multilib \
    protobuf-compiler \
    && rm -rf /var/lib/apt/lists/*

# Get the default Rust target for this platform and convert to musl variant
RUN rustc --print target-list | grep "$(rustc --print host-tuple | cut -d'-' -f1-2)-linux-musl" | head -1 > /target.txt

# Add the musl target for this architecture
RUN rustup target add $(cat /target.txt)

# Set working directory
WORKDIR /app

# Copy the shared library first (for better caching)
COPY shared/ ./shared/

# Copy Cargo files
COPY processor/Cargo.toml processor/Cargo.lock ./processor/

# Create src directory structure
RUN mkdir -p processor/src

# Create a dummy main.rs to build dependencies (for better caching)
# RUN echo "fn main() {}" > processor/src/main.rs

# Build dependencies (from the rtpmidi subdirectory)
WORKDIR /app/processor
# RUN cargo build --target $(cat /target.txt) --release

# Copy the actual source code (this will invalidate the cache)
# RUN rm src/main.rs
COPY processor/src/ ./src/

# Remove the dummy binary and rebuild with real source
RUN rm -f target/*/release/processor && \
    cargo build --target $(cat /target.txt) --release

# Stage 2: Runtime image (scratch)
FROM scratch
COPY --from=builder /app/processor/target/*/release/processor /processor
ENTRYPOINT ["/processor"]
# FROM alpine
# COPY --from=builder /app/processor/target/*/release/processor /processor
# ENTRYPOINT ["/bin/sh"]
