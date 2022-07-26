<script lang="ts">
	import type { OnMessageCallback } from 'mqtt/dist/mqtt.min';

	import { getContext, onDestroy } from 'svelte';
	import { MqttContextKey, type IMqttContext } from '../context/MqttContext.svelte';
	import type { IVenue } from '../models/venue';
	import type { IVenuePayload } from '../models/venuePayload';
	import { loadedGroups } from '../stores/loadedGroups';
	import { loadedVenue } from '../stores/loadedVenueStore';
	const { getMqtt } = getContext<IMqttContext>(MqttContextKey);

	const client = getMqtt();

	let activeGroups: string[];
	let activeVenue: IVenue;
	loadedVenue.subscribe((value) => (activeVenue = value));
	loadedGroups.subscribe((value) => (activeGroups = value));

	const listener = (topic: string, payload: OnMessageCallback) => {
		if (topic === '/venue/load') {
			const venuePayload = JSON.parse(payload.toString()) as IVenuePayload;
			loadedVenue.set(venuePayload.venue);
			loadedGroups.set(venuePayload.groups);
		}
	};
	client.on('message', listener);
	client.subscribe('/venue/load');

	onDestroy(() => {
		client.unsubscribe('/venue/load');
		client.removeListener('message', listener);
	});
</script>

<svelte:head>
	<title>Home</title>
	<meta name="description" content="Kadmium-Dmx 2" />
</svelte:head>

<section>
	<h1>Kadmium-Dmx 2</h1>
	{#if activeVenue}
		<h2>{activeVenue.name} ({activeVenue.city})</h2>
		<h3>Groups</h3>
		<ul>
			{#each activeGroups as activeGroup}
				<li>{activeGroup}</li>
			{/each}
		</ul>
	{:else}
		<h2>Loading</h2>
	{/if}
</section>

<style>
	section {
		display: flex;
		flex-direction: column;
		justify-content: center;
		align-items: center;
		flex: 1;
	}

	h1 {
		width: 100%;
	}
</style>
