<script lang="ts">
	import { onMount } from 'svelte';
	import GoDiffAdded from 'svelte-icons/go/GoDiffAdded.svelte';
	import GoPencil from 'svelte-icons/go/GoPencil.svelte';
	import GoTrashcan from 'svelte-icons/go/GoTrashcan.svelte';
	import { Button, Table } from 'sveltestrap';
	import IconContainer from '../../components/IconContainer.svelte';
	import type { IFixtureDefinition, IFixtureDefinitionKey } from '../../models/fixtureDefinition';
	import {
		addFixtureDefinition,
		deleteFixtureDefinition,
		getFixtureDefitionKeys
	} from '../../services/fixtureDefinitionService';

	let definitionsPromise = new Promise<IFixtureDefinitionKey[]>(() => {});

	const handleDeleteClick = async (key: IFixtureDefinitionKey) => {
		await deleteFixtureDefinition(key.id);
		const defs = await definitionsPromise;
		const index = defs.indexOf(key);
		defs.splice(index, 1);
		definitionsPromise = definitionsPromise;
	};

	onMount(async () => {
		definitionsPromise = getFixtureDefitionKeys();
	});
</script>

<svelte:head>
	<title>Fixture Definitions</title>
	<meta name="description" content="Fixture Definitions" />
</svelte:head>

<section>
	<h1>Fixture Definitions</h1>
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
							<a href={`fixture-definition/edit/${definition.id}`} class="btn btn-warning">
								<IconContainer><GoPencil /></IconContainer>
							</a>
							<Button color="danger" on:click={() => handleDeleteClick(definition)}
								><IconContainer><GoTrashcan /></IconContainer></Button
							>
						</td>
					</tr>
				{/each}
			</tbody>
		</Table>
		<a class="btn btn-success" href="fixture-definition/edit/new">
			<IconContainer><GoDiffAdded /></IconContainer>
			Add Fixture Definition
		</a>
	{:catch error}
		<p>Unable to fetch Fixture Definitions:</p>
		<pre>{error}</pre>
	{/await}
</section>

<style>
	.last-column {
		text-align: right;
	}
</style>
