import type { IFixtureDefinition, IFixtureDefinitionKey } from "../models/fixtureDefinition";
import { getServiceUri } from "./backendService"

const getBaseUri = () => getServiceUri('fixtureDefinition/');

export const getFixtureDefitionKeys = async () => {
	const uri = getBaseUri();
	const result = await fetch(uri);
	const json = await result.json() as IFixtureDefinitionKey[];
	return json;
}

export const getFixtureDefition = async (id: string) => {
	const uri = new URL(id, getBaseUri());
	const result = await fetch(uri);
	const json = await result.json() as IFixtureDefinition;
	return json;
}