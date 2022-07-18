<script lang="ts">
	import { getContext } from 'svelte';
	import { MqttContextKey, type IMqttContext } from '../context/MqttContext.svelte';
	import type { IVenue } from '../models/venue';
	import { loadedGroups } from '../stores/loadedGroups';
	import { loadedVenue } from '../stores/loadedVenueStore';
	const { getMqtt } = getContext<IMqttContext>(MqttContextKey);

	const client = getMqtt();

	let activeGroups: string[];
	let activeVenue: IVenue;
	loadedVenue.subscribe((value) => (activeVenue = value));
	loadedGroups.subscribe((value) => (activeGroups = value));

	client.on('message', (topic, payload) => {
		if (topic === '/venue/load') {
			const venuePayload = JSON.parse(payload.toString());
			loadedVenue.set(venuePayload.venue);
			loadedGroups.set(venuePayload.groups);
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
