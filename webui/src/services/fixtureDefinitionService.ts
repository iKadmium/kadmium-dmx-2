import type { IFixtureDefinition, IFixtureDefinitionKey } from "../models/fixtureDefinition";
import { BackendService } from "./backendService";

export class FixtureDefinitionService extends BackendService<IFixtureDefinitionKey, IFixtureDefinition> {
	async getBaseUri(): Promise<URL> {
		return new URL('fixturedefinition/', await this.apiRoot);
	}
}