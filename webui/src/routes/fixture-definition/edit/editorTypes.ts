import type { IDmxChannel, IFixtureDefinition, IFixturePersonality, IFixturePersonalityMap, IMovementAxis } from "../../../models/fixtureDefinition";

export interface IEditorDmxChannelEntry extends IDmxChannel {
	address: number;
}

export interface IEditorPersonalityEntry {
	name: string;
	personality: IEditorDmxChannelEntry[];
}

export interface IEditorMovementAxisEntry {
	name: string;
	axis: IMovementAxis;
};

export interface IEditorFixtureDefinition {
	id?: string;
	manufacturer: string;
	model: string;
	personalities: IEditorPersonalityEntry[];
	movementAxis: IEditorMovementAxisEntry[];
}

export const deserialize = (definition: IFixtureDefinition) => {
	const personalities = Object.keys(definition.personalities).map((personalityName) => {
		const originalPersonality = definition.personalities[personalityName];
		const channels: IEditorDmxChannelEntry[] = Object.keys(originalPersonality).map(
			(addressStr) => {
				const address = Number(addressStr);
				const channel = originalPersonality[address];
				return {
					...channel,
					address
				};
			}
		);
		return {
			name: personalityName,
			personality: channels
		};
	});
	const movementAxis = Object.entries(definition.movementAxis).map((x) => {
		const [name, axis] = x;
		return {
			name,
			axis
		};
	});

	const result: IEditorFixtureDefinition = {
		id: definition.id,
		manufacturer: definition.manufacturer,
		model: definition.model,
		personalities,
		movementAxis
	}

	return result;
}

export const serialize = (definition: IEditorFixtureDefinition) => {
	const result: IFixtureDefinition = {
		id: definition.id,
		manufacturer: definition.manufacturer,
		model: definition.model,
		personalities: {},
		movementAxis: {}
	}

	for (const personalityEntry of definition.personalities) {
		const personality: IFixturePersonality = {};
		for (const channelEntry of personalityEntry.personality) {
			personality[channelEntry.address] = {
				name: channelEntry.name,
				min: channelEntry.min ? channelEntry.min : undefined,
				max: channelEntry.max && channelEntry.max !== 255 ? channelEntry.max : undefined
			}
		}
		result.personalities[personalityEntry.name] = personality;
	}
	for (const axisEntry of definition.movementAxis) {
		result.movementAxis[axisEntry.name] = {
			minAngle: axisEntry.axis.minAngle,
			maxAngle: axisEntry.axis.maxAngle
		}
	}

	return result;
}