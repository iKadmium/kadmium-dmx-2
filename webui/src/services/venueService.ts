import type { IVenue, IVenueKey } from "../models/venue";
import type { IVenuePayload } from "../models/venuePayload";
import { BackendService } from "./backendService";

export class VenueService extends BackendService<IVenueKey, IVenue>
{
	async getBaseUri(): Promise<URL> {
		return new URL('venue/', await this.apiRoot);
	}

	public async getVenuePayload(id: string): Promise<IVenuePayload> {
		const uri = new URL(`${id}/payload`, await this.getBaseUri());
		const result = await fetch(uri);
		const json = await result.json() as IVenuePayload;
		return json;
	}

}