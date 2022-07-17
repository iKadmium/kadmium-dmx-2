export interface IBrightnessLimiterOptions {
	maxBrightness: number;
}

export interface IAxisConstrainerOptions {
	[axis: string]: {
		min: number;
		max: number;
	}
}

export interface IFixtureInstance {
	groups: string[];
	manufacturer: string;
	model: string;
	options: {
		BrightnessLimiter?: IBrightnessLimiterOptions;
		AxisConstrainer?: IAxisConstrainerOptions;
	}
	personality: string;
}

export interface IUniverse {
	name: string;
	fixtures: {
		[address: number]: IFixtureInstance;
	}
}

export interface IVenue {
	id: string;
	name: string;
	city: string;
	universes: {
		[universeId: number]: IUniverse;
	}
}

export interface IVenueKey {
	id: string;
	name: string;
	city: string;
}