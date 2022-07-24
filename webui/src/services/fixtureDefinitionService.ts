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

export const updateFixtureDefinition = async (id: string, definition: IFixtureDefinition) => {
	const uri = new URL(id, getBaseUri());
	const result = await fetch(
		uri,
		{
			headers: {
				'Content-Type': 'application/json'
			},
			method: 'PUT',
			body: JSON.stringify(definition),
		}
	);
	const json = await result.json() as IFixtureDefinition;
	return json;
}

export const deleteFixtureDefinition = async (id: string) => {
	const uri = new URL(id, getBaseUri());
	await fetch(
		uri,
		{
			method: 'DELETE'
		}
	);
}

export const addFixtureDefinition = async (definition: IFixtureDefinition) => {
	const uri = new URL(getBaseUri());
	await fetch(
		uri,
		{
			headers: {
				'Content-Type': 'application/json'
			},
			method: 'POST',
			body: JSON.stringify(definition),
		}
	);
}