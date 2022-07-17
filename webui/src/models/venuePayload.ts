import type { IFixtureDefinition } from "./fixtureDefinition";
import type { IVenue } from "./venue";

export interface IVenuePayload {
	fixtureDefinitions: IFixtureDefinition[];
	groups: string[];
	venue: IVenue;
}