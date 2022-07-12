# Processor
This takes incoming MQTT messages setting various attributes of fixtures (or, more likely, groups) and outputs them back to the MQTT DMX stream.

## Topics listened to
* `/fixture/{universeId}/{attribute}` => Set the attribute on that single fixture. Attributes are a 32 bit float, mostly normalised from 0 to 1.
* `/group/{groupName}/{attribute}` => Set the attribute for that fixture group. Attributes are a 32 bit float, mostly normalised from 0 to 1.
* `/venue/load` => Load a venue. The JSON format is described below.

## Topics transmitted
* `/universe/{universeId}` => DMX values for that universe. The payload is an array of 512 bytes.

## Venue format

```
{
	"fixtureDefinitions": [
		{
			"manufacturer": "Someone",
			"model": "Something",
			"personalities": {
				"7ch": {
					"1": {
						"name": "Red"
					},
					"2": {
						"name": "Green",
						"min": 0,
						"max": 255
					},
					"3": {
						"name": "Blue",
						"min": 0,
						"max": 255
					},
					"4": {
						"name": "PanCoarse",
						"min": 0,
						"max": 255
					},
					"5": {
						"name": "PanFine"
					},
					"6": {
						"name": "TiltCoarse"
					},
					"7": {
						"name": "TiltFine"
					}
				}
			},
			"movementAxis": {
				"Pan": {
					"minAngle": -270,
					"maxAngle": 270
				},
				"Tilt": {
					"minAngle": -135,
					"maxAngle": 135
				}
			}
		}
	],
	"venue": {
		"name": "My venue",
		"universes": {
			"1": {
				"name": "First universe",
				"fixtures": {
					"1": {
						"manufacturer": "Someone",
						"model": "Something",
						"personality": "7ch",
						"groups": [
							"vocalist"
						],
						"options": {
							"BrightnessLimiter": {
								"maxBrightness": 0.8
							},
							"AxisConstrainer": {
								"Pan": {
									"min": -90,
									"max": 90
								},
								"Tilt": {
									"min": 0,
									"max": 90
								}
							}
						}
					}
				}
			}
		}
	}
}
```
