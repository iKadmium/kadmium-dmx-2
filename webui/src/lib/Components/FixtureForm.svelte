<script lang="ts" module>
	export interface FixtureFormProps {
		fixture: Fixture;
		onSubmit: (fixture: Fixture) => void;
	}
</script>

<script lang="ts">
	import type { Fixture, Personality } from '../types/fixture.js';
	import PersonalityEditor from './PersonalityEditor.svelte';

	let { fixture: initialFixture, onSubmit }: FixtureFormProps = $props();

	let fixture = $state<Fixture>(initialFixture);

	let autoNamePersonalities = $state(true);
	let activePersonalityIndex = $state(0);

	// Handler functions for PersonalityEditor
	function handlePersonalitiesChange(newPersonalities: Personality[]): void {
		fixture.personalities = newPersonalities;
	}

	function handleActivePersonalityIndexChange(index: number): void {
		activePersonalityIndex = index;
	}

	$effect(() => {
		if (autoNamePersonalities) {
			fixture.personalities.forEach((personality, index) => {
				personality.name = `${personality.channels.length}ch`;
			});
		}
	});

	function handleSubmit(event: Event): void {
		event.preventDefault();
		onSubmit(fixture);
	}
</script>

<form class="mx-auto w-full max-w-xl space-y-4" onsubmit={handleSubmit}>
	<h2 class="text-2xl">Lighting Fixture Definition</h2>

	<label class="label">
		<span class="label-text">Manufacturer</span>
		<input type="text" class="input" bind:value={fixture.manufacturer} />
	</label>

	<label class="label">
		<span class="label-text">Model</span>
		<input type="text" class="input" bind:value={fixture.model} />
	</label>

	<label class="label">
		<span class="label-text">Type</span>
		<select class="select" bind:value={fixture.type}>
			<option value="LED">LED</option>
			<option value="Incandescent">Incandescent</option>
			<option value="Effect">Effect</option>
			<option value="Other">Other</option>
		</select>
	</label>

	<label class="flex items-center space-x-2">
		<input class="checkbox" type="checkbox" bind:checked={autoNamePersonalities} />
		<p>Automatically name personalities based on the number of channels</p>
	</label>

	<PersonalityEditor
		personalities={fixture.personalities}
		{autoNamePersonalities}
		{activePersonalityIndex}
		onPersonalitiesChange={handlePersonalitiesChange}
		onActivePersonalityIndexChange={handleActivePersonalityIndexChange}
	/>

	<button class="btn bg-success-500 w-full text-black" type="submit">
		Save Fixture Definition
	</button>
</form>
