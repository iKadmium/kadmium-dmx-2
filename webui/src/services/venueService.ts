import type { IVenue, IVenueKey } from "../models/venue";
import type { IVenuePayload } from "../models/venuePayload";
import { BackendService } from "./backendService";

export class VenueService extends BackendService<IVenueKey, IVenue>
{
	async activate(id: string): Promise<void> {
		const uri = new URL(`${id}/activate`, await this.getBaseUri());
		await fetch(uri,
			{
				method: "POST"
			}
		);
	}

	async getBaseUri(): Promise<URL> {
		return new URL('venue/', await this.apiRoot);
	}
}