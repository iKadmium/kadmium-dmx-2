<script lang="ts">
	import type { Fixture, Personality } from '../types/fixture.js';
	import PersonalityEditor from './PersonalityEditor.svelte';

	let fixture = $state<Fixture>({
		manufacturer: '',
		model: '',
		type: 'LED',
		personalities: [
			{
				name: '1ch',
				channels: [
					{
						number: 1,
						type: 'shutter',
						colorMacros: [
							{
								color: '#ffffff',
								min: 0,
								max: 255
							}
						],
						shutterFunctions: [
							{
								name: 'dimmer',
								min: 0,
								max: 255
							}
						]
					}
				]
			}
		],
		movementAxis: []
	});

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
</script>

<form class="mx-auto w-full max-w-xl space-y-4" onsubmit={(e) => e.preventDefault()}>
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
