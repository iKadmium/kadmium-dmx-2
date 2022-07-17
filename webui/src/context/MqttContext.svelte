<script context="module" lang="ts">
	export interface IMqttContext {
		getMqtt: () => Client;
	}
	export const MqttContextKey = Symbol();
</script>

<script lang="ts">
	import type { Client } from 'mqtt';
	import * as mqtt from 'mqtt/dist/mqtt.min';
	import { onDestroy, setContext, onMount } from 'svelte';

	let client: Client;

	const contextValue: IMqttContext = {
		getMqtt: () => client
	};

	setContext<IMqttContext>(MqttContextKey, contextValue);

	onMount(() => {
		client = mqtt.connect('ws://localhost:9001');
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
