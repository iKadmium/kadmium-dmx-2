<script context="module" lang="ts">
	export const BackendContextKey = Symbol();
	export interface IBackendContext {
		fixtureDefinitionService: FixtureDefinitionService;
		venueService: VenueService;
	}
</script>

<script lang="ts">
	import { getContext, setContext } from 'svelte';
	import { FixtureDefinitionService } from '../services/fixtureDefinitionService';
	import { VenueService } from '../services/venueService';
	import { EnvironmentContextKey, type IEnvironmentContext } from './EnvironmentContext.svelte';

	const { getApiRoot } = getContext<IEnvironmentContext>(EnvironmentContextKey);

	const contextValue: IBackendContext = {
		fixtureDefinitionService: new FixtureDefinitionService(getApiRoot()),
		venueService: new VenueService(getApiRoot())
	};
	setContext<IBackendContext>(BackendContextKey, contextValue);
</script>

{#if contextValue}
	<slot />
{/if}
