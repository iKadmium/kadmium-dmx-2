<script type="ts" async>
	import { page } from '$app/stores';
	import { Accordion, Button, Col, Form, FormGroup, Input, Row } from 'sveltestrap';

	import { goto } from '$app/navigation';
	import { getContext } from 'svelte';
	import AccordionItemWithDeleteButton from '../../../components/AccordionItemWithDeleteButton.svelte';
	import { BackendContextKey, type IBackendContext } from '../../../context/BackendContext.svelte';
	import type { IFixtureDefinition, IMovementAxis } from '../../../models/fixtureDefinition';
	import {
		deserialize,
		serialize,
		type IEditorDmxChannelEntry,
		type IEditorFixtureDefinition,
		type IEditorMovementAxisEntry,
		type IEditorPersonalityEntry
	} from './editorTypes';

	const { fixtureDefinitionService } = getContext<IBackendContext>(BackendContextKey);
	let definitionPromise = new Promise<IFixtureDefinition>(() => {});
	let definition: IEditorFixtureDefinition;

	const id = $page.params.id;

	const onSubmit = async () => {
		const serialized = serialize(definition);
		if (definition.id) {
			await fixtureDefinitionService.update(id, serialized);
		} else {
			await fixtureDefinitionService.create(serialized);
		}
		goto('/fixture-definition');
	};

	const addPersonality = () => {
		const newPersonality: IEditorPersonalityEntry = {
			name: 'New Personality',
			personality: [
				{
					address: 1,
					name: 'New Channel',
					min: 0,
					max: 255
				}
			]
		};

		definition.personalities.push(newPersonality);
		definition = definition;
	};

	const addAxis = () => {
		const newAxis: IMovementAxis = {
			minAngle: -270,
			maxAngle: 270
		};

		definition.movementAxis.push({
			name: 'New Axis',
			axis: newAxis
		});

		definition = definition;
	};

	const addChannel = (personality: IEditorPersonalityEntry) => {
		const maxChannel = personality.personality
			.map((x) => x.address)
			.reduce((previous, value) => (previous = previous > value ? previous : value));
		const newChannel: IEditorDmxChannelEntry = {
			address: maxChannel + 1,
			name: 'New channel',
			min: 0,
			max: 255
		};
		personality.personality.push(newChannel);
		definition = definition;
	};

	function deleteItem<TItem>(arr: TItem[], item: TItem) {
		const index = arr.indexOf(item);
		arr.splice(index, 1);
		definition = definition;
	}

	const deletePersonality = (personality: IEditorPersonalityEntry) => {
		deleteItem(definition.personalities, personality);
	};

	const deleteAxis = (axis: IEditorMovementAxisEntry) => {
		deleteItem(definition.movementAxis, axis);
	};

	const deleteChannel = (channel: IEditorDmxChannelEntry, personality: IEditorPersonalityEntry) => {
		deleteItem(personality.personality, channel);
	};

	if (id !== 'new') {
		definitionPromise = fixtureDefinitionService.read(id);
	} else {
		const newDef: IFixtureDefinition = {
			manufacturer: '',
			model: '',
			movementAxis: {},
			personalities: {}
		};
		definitionPromise = Promise.resolve(newDef);
	}
	definitionPromise.then((def) => (definition = deserialize(def)));
</script>

<svelte:head>
	<title>Editing Fixture Definition</title>
	<meta name="description" content="Fixture Definitions" />
</svelte:head>

<h1>Fixture Definition Editor</h1>

{#await definitionPromise}
	Loading...
{:then}
	<Form>
		<h2>Metadata</h2>
		<FormGroup floating label="Manufacturer">
			<Input
				value={definition.manufacturer}
				on:change={(e) => (definition.manufacturer = e.currentTarget?.value)}
			/>
		</FormGroup>
		<FormGroup floating label="Model">
			<Input
				value={definition.model}
				on:change={(e) => (definition.model = e.currentTarget?.value)}
			/>
		</FormGroup>

		<h2>Personalities</h2>
		<Accordion>
			{#each definition.personalities as personality}
				<AccordionItemWithDeleteButton
					title={personality.name}
					on:click={() => deletePersonality(personality)}
				>
					<FormGroup floating label="Name">
						<Input
							value={personality.name}
							on:change={(e) => (personality.name = e.currentTarget?.value)}
						/>
					</FormGroup>
					<h3>DMX Channels</h3>
					<Accordion>
						{#each personality.personality as channel}
							<AccordionItemWithDeleteButton
								title={`${channel.address} - ${channel.name}`}
								on:click={() => deleteChannel(channel, personality)}
							>
								<Row>
									<Col md="3">
										<FormGroup floating label="Address">
											<Input
												value={channel.address}
												type="number"
												on:change={(e) => (channel.address = e.currentTarget?.value)}
											/>
										</FormGroup>
									</Col>
									<Col md="3">
										<FormGroup floating label="Name">
											<Input
												value={channel.name}
												on:change={(e) => (channel.name = e.currentTarget?.value)}
											/>
										</FormGroup>
									</Col>
									<Col md="3">
										<FormGroup floating label="Min Value">
											<Input
												value={channel.min}
												type="number"
												min="0"
												max="255"
												on:change={(e) => (channel.min = e.currentTarget?.value)}
											/>
										</FormGroup>
									</Col>
									<Col md="3">
										<FormGroup floating label="Max Value">
											<Input
												value={channel.max}
												type="number"
												min="0"
												max="255"
												on:change={(e) => (channel.max = e.currentTarget?.value)}
											/>
										</FormGroup>
									</Col>
								</Row>
							</AccordionItemWithDeleteButton>
						{/each}
					</Accordion>
					<Button color="success" type="button" on:click={() => addChannel(personality)}>
						Add Channel
					</Button>
				</AccordionItemWithDeleteButton>
			{/each}
			<Button color="success" type="button" on:click={() => addPersonality()}>
				Add Personality
			</Button>
		</Accordion>

		<h2>Movement Axis</h2>
		<Accordion>
			{#each definition.movementAxis as axis}
				<AccordionItemWithDeleteButton title={axis.name} on:click={() => deleteAxis(axis)}>
					<Row>
						<Col md="6">
							<FormGroup floating label="Name">
								<Input value={axis.name} on:change={(e) => (axis.name = e.currentTarget?.value)} />
							</FormGroup>
						</Col>
						<Col md="3">
							<FormGroup floating label="Min Angle (degrees)">
								<Input
									value={axis.axis.minAngle}
									type="number"
									on:change={(e) => (axis.axis.minAngle = e.currentTarget?.value)}
								/>
							</FormGroup>
						</Col>
						<Col md="3">
							<FormGroup floating label="Max Angle (degrees)">
								<Input
									value={axis.axis.maxAngle}
									type="number"
									on:change={(e) => (axis.axis.maxAngle = e.currentTarget?.value)}
								/>
							</FormGroup>
						</Col>
					</Row>
				</AccordionItemWithDeleteButton>
			{/each}
			<Button color="success" type="button" on:click={() => addAxis()}>Add Axis</Button>
		</Accordion>
		<Button color="primary" type="button" on:click={onSubmit}>Submit</Button>
	</Form>
{/await}
