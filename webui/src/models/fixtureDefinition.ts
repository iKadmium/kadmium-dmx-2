export interface IDmxChannel {
	name: string;
	min?: number;
	max?: number;
}

export interface IMovementAxis {
	minAngle: number;
	maxAngle: number;
}

export interface IFixturePersonality {
	[key: number]: IDmxChannel;
}

export interface IFixturePersonalityMap {
	[name: string]: IFixturePersonality;
}

export interface IFixtureDefinition {
	id?: string;
	manufacturer: string;
	model: string;
	personalities: IFixturePersonalityMap;
	movementAxis: {
		[name: string]: IMovementAxis;
	};
}

export interface IFixtureDefinitionKey {
	id: string;
	manufacturer: string;
	model: string;
}