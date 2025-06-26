<script lang="ts">
	import Tab from './Tabs/Tab.svelte';
	import TabStrip from './Tabs/TabStrip.svelte';

	// Define types for the lighting fixture definition
	type FixtureType = 'LED' | 'Incandescent' | 'Effect' | 'Other';
	type ChannelType =
		| 'red'
		| 'green'
		| 'blue'
		| 'uv'
		| 'white'
		| 'amber'
		| 'color-macro'
		| 'shutter'
		| 'other';
	type ShutterFunction = 'strobe' | 'dimmer';

	type ColorMacro = {
		color: string;
		min: number;
		max: number;
	};

	type Channel = {
		number: number;
		name?: string;
		type: ChannelType;
		colorMacros: ColorMacro[];
		shutterFunctions: { name: ShutterFunction; min: number; max: number }[];
	};

	type Personality = {
		name: string;
		channels: Channel[];
	};

	type Fixture = {
		manufacturer: string;
		model: string;
		type: FixtureType;
		personalities: Personality[];
		movementAxis: string[];
	};

	// Replace createStore with $state for Svelte 5 runes
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

	// Add a new personality
	function addPersonality(): void {
		fixture.personalities.push({
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
		});
	}

	function removePersonality(index: number): void {
		if (fixture.personalities.length > 1) {
			fixture.personalities.splice(index, 1);
		} else {
			alert('At least one personality must remain.');
		}
	}

	// Add a new channel to a personality
	function addChannel(personalityIndex: number): void {
		const channels = fixture.personalities[personalityIndex].channels;
		const existingNumbers = channels.map((channel) => channel.number);
		const nextNumber = Math.min(
			...Array.from({ length: Math.max(0, ...existingNumbers) + 1 }, (_, i) => i + 1).filter(
				(n) => !existingNumbers.includes(n)
			)
		);

		const previousChannel = channels[channels.length - 1];
		let nextType: ChannelType = 'shutter';

		if (previousChannel) {
			switch (previousChannel.type) {
				case 'shutter':
					nextType = 'red';
					break;
				case 'red':
					nextType = 'green';
					break;
				case 'green':
					nextType = 'blue';
					break;
				case 'blue':
					nextType = 'shutter';
					break;
			}
		}

		channels.push({
			number: nextNumber,
			name: '',
			type: nextType,
			colorMacros: [],
			shutterFunctions: []
		});
	}

	// Remove a channel from a personality
	function removeChannel(personalityIndex: number, channelIndex: number): void {
		fixture.personalities[personalityIndex].channels.splice(channelIndex, 1);
	}

	// Add a new color macro to a channel
	function addColorMacro(personalityIndex: number, channelIndex: number): void {
		fixture.personalities[personalityIndex].channels[channelIndex].colorMacros.push({
			color: '#ffffff',
			min: 0,
			max: 255
		});
	}

	// Remove a color macro from a channel
	function removeColorMacro(
		personalityIndex: number,
		channelIndex: number,
		macroIndex: number
	): void {
		fixture.personalities[personalityIndex].channels[channelIndex].colorMacros.splice(
			macroIndex,
			1
		);
	}

	// Add a new shutter option to a channel
	function addShutterFunction(personalityIndex: number, channelIndex: number): void {
		fixture.personalities[personalityIndex].channels[channelIndex].shutterFunctions.push({
			name: 'strobe',
			min: 0,
			max: 255
		});
	}

	// Remove a shutter option from a channel
	function removeShutterFunction(
		personalityIndex: number,
		channelIndex: number,
		optionIndex: number
	): void {
		fixture.personalities[personalityIndex].channels[channelIndex].shutterFunctions.splice(
			optionIndex,
			1
		);
	}

	function handleChannelTypeChange(channel: Channel): void {
		if (channel.type === 'shutter' && channel.shutterFunctions.length === 0) {
			channel.shutterFunctions.push({
				name: 'strobe',
				min: 0,
				max: 255
			});
		}

		// Automatically add a color macro if the channel type is updated to 'color-macro'
		if (channel.type === 'color-macro' && channel.colorMacros.length === 0) {
			channel.colorMacros.push({
				color: '#ffffff',
				min: 0,
				max: 255
			});
		}
	}

	let autoNamePersonalities = $state(true);

	$effect(() => {
		if (autoNamePersonalities) {
			fixture.personalities.forEach((personality, index) => {
				personality.name = `${personality.channels.length}ch`;
			});
		}
	});
</script>

<form>
	<h2>Lighting Fixture Definition</h2>

	<label>
		Manufacturer:
		<input type="text" bind:value={fixture.manufacturer} />
	</label>

	<label>
		Model:
		<input type="text" bind:value={fixture.model} />
	</label>

	<label>
		Type:
		<select bind:value={fixture.type}>
			<option value="LED">LED</option>
			<option value="Incandescent">Incandescent</option>
			<option value="Effect">Effect</option>
			<option value="Other">Other</option>
		</select>
	</label>

	<label>
		<input type="checkbox" bind:checked={autoNamePersonalities} />
		Automatically name personalities based on the number of channels
	</label>

	<!-- Update the personalities section to use Tabs correctly -->
	<h3>Personalities</h3>
	<TabStrip showAddButton={true} onAddTab={addPersonality}>
		{#each fixture.personalities as personality, pIndex}
			<Tab
				label={personality.name}
				closable={fixture.personalities.length > 1}
				onClosed={() => removePersonality(pIndex)}
			>
				{#if !autoNamePersonalities}
					<label>
						Name:
						<input type="text" bind:value={personality.name} />
					</label>
				{/if}

				<h4>Channels</h4>
				{#each personality.channels as channel, cIndex}
					<fieldset>
						<legend>
							{channel.number}: {channel.type !== 'other' ? channel.type : channel.name}
						</legend>

						<label>
							Type:
							<select bind:value={channel.type} onchange={() => handleChannelTypeChange(channel)}>
								<option value="red">Red</option>
								<option value="green">Green</option>
								<option value="blue">Blue</option>
								<option value="uv">UV</option>
								<option value="white">White</option>
								<option value="amber">Amber</option>
								<option value="color-macro">Color Macro</option>
								<option value="shutter">Shutter</option>
								<option value="other">Other</option>
							</select>
						</label>

						{#if channel.type === 'other'}
							<label>
								Name:
								<input type="text" bind:value={channel.name} />
							</label>
						{/if}

						<label>
							Channel Number:
							<input type="number" min="1" bind:value={channel.number} />
						</label>

						{#if channel.type === 'color-macro'}
							<h5>Color Macros</h5>
							{#each channel.colorMacros as macro, mIndex}
								<fieldset>
									<legend>Color Macro {mIndex + 1}</legend>

									<label>
										Color:
										<input type="color" bind:value={macro.color} />
									</label>

									<label>
										Min Value:
										<input type="number" min="0" max="255" bind:value={macro.min} />
									</label>

									<label>
										Max Value:
										<input type="number" min="0" max="255" bind:value={macro.max} />
									</label>

									{#if channel.colorMacros.length > 1}
										<button type="button" onclick={() => removeColorMacro(pIndex, cIndex, mIndex)}>
											Remove Color Macro
										</button>
									{/if}
								</fieldset>
							{/each}

							<button type="button" onclick={() => addColorMacro(pIndex, cIndex)}
								>Add Color Macro</button
							>
						{/if}

						{#if channel.type === 'shutter'}
							<h5>Shutter Functions</h5>
							{#each channel.shutterFunctions as shutterFunction, oIndex}
								<fieldset>
									<legend>Shutter Function {oIndex + 1}</legend>

									<label>
										Name:
										<select bind:value={shutterFunction.name}>
											<option value="strobe">Strobe</option>
											<option value="dimmer">Dimmer</option>
										</select>
									</label>

									<label>
										Min Value:
										<input type="number" min="0" max="255" bind:value={shutterFunction.min} />
									</label>

									<label>
										Max Value:
										<input type="number" min="0" max="255" bind:value={shutterFunction.max} />
									</label>

									{#if channel.shutterFunctions.length > 1}
										<button
											type="button"
											onclick={() => removeShutterFunction(pIndex, cIndex, oIndex)}
										>
											Remove Shutter Option
										</button>
									{/if}
								</fieldset>
							{/each}

							<button type="button" onclick={() => addShutterFunction(pIndex, cIndex)}
								>Add Shutter Option</button
							>
						{/if}

						<button type="button" onclick={() => removeChannel(pIndex, cIndex)}>
							Remove Channel
						</button>
					</fieldset>
				{/each}

				<button type="button" onclick={() => addChannel(pIndex)}>Add Channel</button>
			</Tab>
		{/each}
	</TabStrip>

	<button type="button" onclick={addPersonality}>Add Personality</button>
</form>

<style>
	form {
		display: flex;
		flex-direction: column;
		gap: 1rem;
	}

	label {
		display: flex;
		flex-direction: row;
		justify-content: space-between;
		align-items: center;
		gap: 1rem;
	}

	@media (max-width: 600px) {
		label {
			flex-direction: column;
			align-items: flex-start;
		}
	}
</style>
