import type { IVenue, IVenueKey } from "../models/venue";
import { getServiceUri } from "./backendService"

const getBaseUri = () => getServiceUri('venue/');

export const getVenueKeys = async () => {
	const uri = getBaseUri();
	const result = await fetch(uri);
	const json = await result.json() as IVenueKey[];
	return json;
}

export const getVenue = async (id: string) => {
	const uri = new URL(id, getBaseUri());
	const result = await fetch(uri);
	const json = await result.json() as IVenue;
	return json;
}

export const getVenuePayload = async (id: string) => {
	const uri = new URL(`${id}/payload`, getBaseUri());
	const result = await fetch(uri);
	const json = await result.json() as IVenue;
	return json;
}

export const deleteVenue = async (id: string) => {
	const uri = new URL(`${id}/payload`, getBaseUri());
	await fetch(
		uri,
		{
			method: 'DELETE'
		}
	);
}