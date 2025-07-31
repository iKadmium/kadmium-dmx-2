# Neewer Bluetooth Light Renderer

This application listens to MQTT messages for venue configuration and lighting control updates, and communicates with Neewer Bluetooth lights.

## Architecture

### Core Components

1. **FixtureManager** (`src/fixture_manager.rs`)
   - Owns and manages the collection of Neewer fixtures
   - Handles venue configuration updates 
   - Processes lighting control commands via protobuf messages
   - Communicates with Bluetooth devices (TODO: implementation pending)

2. **MqttManager** (`src/mqtt_manager.rs`)
   - Connects to MQTT broker
   - Subscribes to `/config/venue` for venue configuration updates
   - Dynamically subscribes to `/bt/neewer/{bt-address}` topics for each fixture
   - Parses JSON venue updates and protobuf lighting commands
   - Forwards all commands to FixtureManager via channels

3. **NeweerFixture** (`src/neewer_fixture.rs`)
   - Represents a Neewer light fixture with metadata
   - Created from venue configuration (filters only Neewer address types)
   - Contains Bluetooth address and fixture information

### Communication Flow

```
MQTT Broker → MqttManager → FixtureManager → Bluetooth Device
```

### Message Types

1. **Venue Updates** (JSON on `/config/venue`)
   - Contains full venue configuration with fixtures
   - Only Neewer fixtures (with `FixtureAddress::Neewer` addresses) are processed
   - Triggers subscription to bulk lighting control topic

2. **Lighting Updates** (Protobuf on `/bt/neewer`)
   - Contains a map of Bluetooth addresses to HSV lighting parameters
   - Each update can control multiple fixtures simultaneously
   - Processed as `NeewerUpdate` protobuf messages with `NeewerLightParams`
   - Uses HSV color space: Hue (0-360°), Saturation (0-255), Brightness (0-255)

### Configuration

Environment variables:
- `MQTT_HOST` (default: "mqtt")
- `MQTT_PORT` (default: "1883")

### Dependencies

- **rumqttc**: MQTT client
- **prost**: Protocol buffer support
- **btleplug**: Bluetooth LE communication (for future implementation)
- **tokio**: Async runtime
- **tracing**: Logging

### Usage

```bash
# Set MQTT broker details
export MQTT_HOST=localhost
export MQTT_PORT=1883

# Run the renderer
cargo run
```

### Next Steps

1. **Bluetooth Implementation**: Complete the `send_to_neewer_light()` function in FixtureManager
2. **Device Discovery**: Add Bluetooth device scanning and connection management
3. **Error Handling**: Implement retry logic for failed Bluetooth communications
4. **Testing**: Add unit tests and integration tests with mock MQTT and Bluetooth

### Example MQTT Messages

#### Venue Update (`/config/venue`)
```json
{
  "venue": {
    "id": "venue1",
    "name": "Main Stage",
    "location": { "city": "Example City" },
    "capacity": null,
    "fixtures": {
      "neewer_address_1": {
        "name": "LED Panel 1",
        "manufacturer": "Neewer",
        "model": "RGB660",
        "personality": "rgb",
        "connection_type": "bluetooth",
        "address": {
          "Neewer": { "address": "AA:BB:CC:DD:EE:FF" }
        }
      }
    }
  }
}
```

#### Lighting Update (`/bt/neewer`)
Protobuf message with map structure:
```
fixtures: {
  "AA:BB:CC:DD:EE:FF": {
    hue: 180,        // 0-360 degrees
    saturation: 255, // 0-255
    brightness: 128  // 0-255
  },
  "11:22:33:44:55:66": {
    hue: 0,
    saturation: 255,
    brightness: 200
  }
}
```
