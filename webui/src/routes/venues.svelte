<script lang="ts">
	import { getContext, onMount } from 'svelte';
	import GoDiffAdded from 'svelte-icons/go/GoDiffAdded.svelte';
	import GoRocket from 'svelte-icons/go/GoRocket.svelte';
	import GoTrashcan from 'svelte-icons/go/GoTrashcan.svelte';
	import { Button } from 'sveltestrap';
	import IconContainer from '../components/IconContainer.svelte';
	import Table from '../components/Table.svelte';
	import { MqttContextKey, type IMqttContext } from '../context/MqttContext.svelte';
	import type { IVenueKey } from '../models/venue';
	import { deleteVenue, getVenueKeys, getVenuePayload } from '../services/venueService';

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

	const handleDeleteClick = async (venueKey: IVenueKey) => {
		await deleteVenue(venueKey.id);
	};
</script>

<svelte:head>
	<title>Venues</title>
	<meta name="description" content="Venues" />
</svelte:head>

<section>
	<h1>Venues</h1>
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
							<Button on:click={() => handleLaunchClick(venue)} color="primary">
								<IconContainer>
									<GoRocket />
								</IconContainer>
							</Button>
							<Button on:click={() => handleDeleteClick(venue)} color="danger">
								<IconContainer><GoTrashcan /></IconContainer>
							</Button>
						</td>
					</tr>
				{/each}
			</tbody>
		</Table>
		<Button color="success">
			<IconContainer><GoDiffAdded /></IconContainer>
			Add Venue
		</Button>
	{:catch error}
		<p>Unable to fetch Venues:</p>
		<pre>{error}</pre>
	{/await}
</section>

<style>
	.last-column {
		text-align: right;
	}
</style>
