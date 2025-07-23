<script lang="ts">
	import FixtureForm from '$lib/Components/FixtureForm.svelte';
	import type { Fixture } from '$lib/types/fixture';

	const initialFixture: Fixture = {
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
						colorMacros: [],
						shutterFunctions: [
							{
								name: 'dimmer',
								min: 0,
								max: 255
							}
						]
					},
					{
						number: 2,
						type: 'color-macro',
						colorMacros: [
							{ color: '#FF0000', min: 0, max: 99 },
							{ color: '#00FF00', min: 100, max: 199 },
							{ color: '#0000FF', min: 200, max: 255 }
						],
						shutterFunctions: []
					}
				]
			}
		],
		movementAxis: []
	};
</script>

<FixtureForm fixture={initialFixture} onSubmit={(fixture) => console.log(fixture)} />
