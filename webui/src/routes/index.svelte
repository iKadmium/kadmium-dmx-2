<script lang="ts">
	import { getContext } from 'svelte';
	import { MqttContextKey, type IMqttContext } from '../context/MqttContext.svelte';
	import type { IVenue } from '../models/venue';
	import { loadedVenue } from '../stores/loadedVenueStore';
	const { getMqtt } = getContext<IMqttContext>(MqttContextKey);

	const client = getMqtt();

	let activeVenue: IVenue;
	loadedVenue.subscribe((value) => (activeVenue = value));

	client.on('message', (topic, payload) => {
		if (topic === '/venue/load') {
			const venuePayload = JSON.parse(payload.toString());
			loadedVenue.set(venuePayload.venue);
		}
	});
	client.subscribe('/venue/load');
</script>

<svelte:head>
	<title>Home</title>
	<meta name="description" content="Kadmium-Dmx 2" />
</svelte:head>

<section>
	<h1>Kadmium-Dmx 2</h1>
	{#if activeVenue}
		<h2>We have loaded {activeVenue.name} in {activeVenue.city}</h2>
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
