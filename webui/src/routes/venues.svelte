<script lang="ts">
	import { getContext, onMount } from 'svelte';
	import GoPencil from 'svelte-icons/go/GoPencil.svelte';
	import GoRocket from 'svelte-icons/go/GoRocket.svelte';
	import GoTrashcan from 'svelte-icons/go/GoTrashcan.svelte';
	import Box from '../components/Box.svelte';
	import Button from '../components/Button.svelte';
	import IconContainer from '../components/IconContainer.svelte';
	import Table from '../components/Table.svelte';
	import { MqttContextKey, type IMqttContext } from '../context/MqttContext.svelte';
	import type { IVenueKey } from '../models/venue';
	import { getVenueKeys, getVenuePayload } from '../services/venueService';
	import GoDiffAdded from 'svelte-icons/go/GoDiffAdded.svelte';

	let venuesPromise = new Promise<IVenueKey[]>(() => {});
	const mqttContext = getContext<IMqttContext>(MqttContextKey);

	onMount(() => {
		venuesPromise = getVenueKeys();
	});

	const handleLaunchClick = async (venueKey: IVenueKey) => {
		const venuePayload = await getVenuePayload(venueKey.id);
		console.log(venuePayload);
		mqttContext.getMqtt().publish('/venue/load', JSON.stringify(venuePayload), { retain: true });
	};
</script>

<svelte:head>
	<title>Venues</title>
	<meta name="description" content="Venues" />
</svelte:head>

<section>
	<h1>Venues</h1>
	<Box>
		{#await venuesPromise}
			<p>Loading...</p>
		{:then venues}
			<Table>
				<thead>
					<th>Name</th>
					<th>City</th>
				</thead>
				<tbody>
					{#each venues as venue}
						<tr>
							<td>{venue.name}</td>
							<td>{venue.city}</td>
							<td class="last-column">
								<Button on:click={() => handleLaunchClick(venue)}>
									<IconContainer>
										<GoRocket />
									</IconContainer>
								</Button>
								<Button><IconContainer><GoPencil /></IconContainer></Button>
								<Button><IconContainer><GoTrashcan /></IconContainer></Button>
							</td>
						</tr>
					{/each}
				</tbody>
			</Table>
			<Button><IconContainer><GoDiffAdded /></IconContainer> Add Venue</Button>
		{:catch error}
			<p>Error: {error}</p>
		{/await}
	</Box>
</section>

<style>
	.last-column {
		text-align: right;
	}
</style>
