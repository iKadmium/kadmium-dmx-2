<script context="module" lang="ts">
	export interface IMqttContext {
		getMqtt: () => Client;
	}
	export const MqttContextKey = Symbol();
</script>

<script lang="ts">
	import type { Client } from 'mqtt';
	import * as mqtt from 'mqtt/dist/mqtt.min';
	import { onDestroy, setContext, onMount, getContext } from 'svelte';
	import { EnvironmentContextKey, type IEnvironmentContext } from './EnvironmentContext.svelte';

	let client: Client;

	const contextValue: IMqttContext = {
		getMqtt: () => client
	};

	setContext<IMqttContext>(MqttContextKey, contextValue);

	const apiRoot = getContext<IEnvironmentContext>(EnvironmentContextKey).getApiRoot();

	onMount(async () => {
		const mqttRoot = new URL(`ws://${(await apiRoot).hostname}:9001`);
		client = mqtt.connect(mqttRoot.toString());
	});

	onDestroy(() => {
		if (client) {
			client.end();
		}
	});
</script>

{#if client}
	<slot />
{/if}
