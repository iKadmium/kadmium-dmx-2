<script lang="ts">
	import { onMount } from 'svelte';
	import Box from '../components/Box.svelte';
	import type { IFixtureDefinitionKey } from '../models/fixtureDefinition';
	import { getFixtureDefitionKeys } from '../services/fixtureDefinitionService';
	import GoTrashcan from 'svelte-icons/go/GoTrashcan.svelte';
	import GoPencil from 'svelte-icons/go/GoPencil.svelte';
	import IconContainer from '../components/IconContainer.svelte';
	import Button from '../components/Button.svelte';
	import Table from '../components/Table.svelte';

	let definitionsPromise = new Promise<IFixtureDefinitionKey[]>(() => {});
	onMount(() => {
		definitionsPromise = getFixtureDefitionKeys();
	});
</script>

<svelte:head>
	<title>Fixture Definitions</title>
	<meta name="description" content="Fixture Definitions" />
</svelte:head>

<section>
	<h1>Fixture Definitions</h1>
	<Box>
		{#await definitionsPromise}
			<p>Loading...</p>
		{:then definitions}
			<Table>
				<thead>
					<th>Manufacturer</th>
					<th>Model</th>
				</thead>
				<tbody>
					{#each definitions as definition}
						<tr>
							<td>{definition.manufacturer}</td>
							<td>{definition.model}</td>
							<td class="last-column">
								<Button><IconContainer><GoPencil /></IconContainer></Button>
								<Button><IconContainer><GoTrashcan /></IconContainer></Button>
							</td>
						</tr>
					{/each}
				</tbody>
			</Table>
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
