export interface IMovementAxis {
	minAngle: number;
	maxAngle: number;
}

export interface IFixturePersonality {
	[key: number]: {
		name: string;
		min?: number;
		max?: number;
	}
}

export interface IFixtureDefinition {
	id: string;
	manufacturer: string;
	model: string;
	personalities: {
		[name: string]: IFixturePersonality;
	};
	movementAxis: {
		[name: string]: IMovementAxis;
	};
}

export interface IFixtureDefinitionKey {
	id: string;
	manufacturer: string;
	model: string;
}