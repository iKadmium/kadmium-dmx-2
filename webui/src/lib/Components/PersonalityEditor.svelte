<script lang="ts">
	import { X } from '@lucide/svelte';
	import { Tabs } from '@skeletonlabs/skeleton-svelte';
	import type { ChannelType, Personality } from '../types/fixture.js';
	import ChannelEditor from './ChannelEditor.svelte';

	// Props
	interface Props {
		personalities: Personality[];
		autoNamePersonalities: boolean;
		activePersonalityIndex: number;
		onPersonalitiesChange: (personalities: Personality[]) => void;
		onActivePersonalityIndexChange: (index: number) => void;
	}

	let {
		personalities,
		autoNamePersonalities,
		activePersonalityIndex,
		onPersonalitiesChange,
		onActivePersonalityIndexChange
	}: Props = $props();

	// Add a new personality
	function addPersonality(): void {
		const newPersonalities = [...personalities];
		newPersonalities.push({
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
		onPersonalitiesChange(newPersonalities);
	}

	function removePersonality(index: number): void {
		if (personalities.length > 1) {
			const newPersonalities = [...personalities];
			newPersonalities.splice(index, 1);
			onPersonalitiesChange(newPersonalities);

			// Adjust active personality index based on which personality was removed
			if (index < activePersonalityIndex) {
				// Removing a personality before the active one, shift index down
				onActivePersonalityIndexChange(activePersonalityIndex - 1);
			} else if (index === activePersonalityIndex) {
				// Removing the currently active personality
				if (index >= newPersonalities.length) {
					// If we removed the last personality, select the new last one
					onActivePersonalityIndexChange(newPersonalities.length - 1);
				} else {
					// Stay on the same index (which now contains the next personality)
					onActivePersonalityIndexChange(index);
				}
			}
			// If index > activePersonalityIndex, no change needed to active index
		} else {
			alert('At least one personality must remain.');
		}
	}

	// Add a new channel to a personality
	function addChannel(personalityIndex: number): void {
		const newPersonalities = [...personalities];
		const channels = newPersonalities[personalityIndex].channels;
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
		onPersonalitiesChange(newPersonalities);
	}

	// Remove a channel from a personality
	function removeChannel(personalityIndex: number, channelIndex: number): void {
		const newPersonalities = [...personalities];
		newPersonalities[personalityIndex].channels.splice(channelIndex, 1);
		onPersonalitiesChange(newPersonalities);
	}

	// Add a new color macro to a channel
	function addColorMacro(personalityIndex: number, channelIndex: number): void {
		const newPersonalities = [...personalities];
		newPersonalities[personalityIndex].channels[channelIndex].colorMacros.push({
			color: '#ffffff',
			min: 0,
			max: 255
		});
		onPersonalitiesChange(newPersonalities);
	}

	// Remove a color macro from a channel
	function removeColorMacro(
		personalityIndex: number,
		channelIndex: number,
		macroIndex: number
	): void {
		const newPersonalities = [...personalities];
		newPersonalities[personalityIndex].channels[channelIndex].colorMacros.splice(macroIndex, 1);
		onPersonalitiesChange(newPersonalities);
	}

	// Add a new shutter option to a channel
	function addShutterFunction(personalityIndex: number, channelIndex: number): void {
		const newPersonalities = [...personalities];
		newPersonalities[personalityIndex].channels[channelIndex].shutterFunctions.push({
			name: 'strobe',
			min: 0,
			max: 255
		});
		onPersonalitiesChange(newPersonalities);
	}

	// Remove a shutter option from a channel
	function removeShutterFunction(
		personalityIndex: number,
		channelIndex: number,
		optionIndex: number
	): void {
		const newPersonalities = [...personalities];
		newPersonalities[personalityIndex].channels[channelIndex].shutterFunctions.splice(
			optionIndex,
			1
		);
		onPersonalitiesChange(newPersonalities);
	}

	function handleChannelTypeChange(personalityIndex: number, channelIndex: number): void {
		const newPersonalities = [...personalities];
		const channel = newPersonalities[personalityIndex].channels[channelIndex];

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

		onPersonalitiesChange(newPersonalities);
	}

	function handlePersonalityNameChange(personalityIndex: number, name: string): void {
		const newPersonalities = [...personalities];
		newPersonalities[personalityIndex].name = name;
		onPersonalitiesChange(newPersonalities);
	}

	function handleChannelChange(
		personalityIndex: number,
		channelIndex: number,
		field: string,
		value: any
	): void {
		const newPersonalities = [...personalities];
		(newPersonalities[personalityIndex].channels[channelIndex] as any)[field] = value;
		onPersonalitiesChange(newPersonalities);
	}

	function handleColorMacroChange(
		personalityIndex: number,
		channelIndex: number,
		macroIndex: number,
		field: string,
		value: any
	): void {
		const newPersonalities = [...personalities];
		(newPersonalities[personalityIndex].channels[channelIndex].colorMacros[macroIndex] as any)[
			field
		] = value;
		onPersonalitiesChange(newPersonalities);
	}

	function handleShutterFunctionChange(
		personalityIndex: number,
		channelIndex: number,
		functionIndex: number,
		field: string,
		value: any
	): void {
		const newPersonalities = [...personalities];
		(
			newPersonalities[personalityIndex].channels[channelIndex].shutterFunctions[
				functionIndex
			] as any
		)[field] = value;
		onPersonalitiesChange(newPersonalities);
	}

	function getInputValue(event: Event): string {
		return (event.target as HTMLInputElement).value;
	}

	function getSelectValue(event: Event): string {
		return (event.target as HTMLSelectElement).value;
	}
</script>

<div class="personality-editor">
	<div class="mb-4 flex items-center justify-between">
		<h3 class="text-xl">Personalities</h3>
		<button type="button" class="btn preset-filled-primary-500" onclick={addPersonality}>
			Add Personality
		</button>
	</div>

	<Tabs
		value={activePersonalityIndex.toString()}
		onValueChange={(e) => onActivePersonalityIndexChange(parseInt(e.value))}
	>
		{#snippet list()}
			{#each personalities as personality, pIndex}
				<Tabs.Control value={pIndex.toString()}>
					<div class="flex items-center gap-2">
						<span>{personality.name}</span>
						{#if personalities.length > 1}
							<button
								type="button"
								class="btn btn-sm preset-outlined-error-500"
								onclick={(e) => {
									e.stopPropagation();
									removePersonality(pIndex);
								}}
								title="Remove personality"
							>
								<X size={20} />
							</button>
						{/if}
					</div>
				</Tabs.Control>
			{/each}
		{/snippet}
		{#snippet content()}
			{#each personalities as personality, pIndex}
				<Tabs.Panel value={pIndex.toString()}>
					<div class="space-y-4">
						{#if !autoNamePersonalities}
							<label class="label">
								<span class="label-text">Name</span>
								<input
									type="text"
									class="input"
									value={personality.name}
									oninput={(e) => handlePersonalityNameChange(pIndex, getInputValue(e))}
								/>
							</label>
						{/if}

						<div class="flex items-center justify-between">
							<h4 class="text-secondary-200 text-lg">Channels</h4>
							<button
								class="btn preset-filled-primary-500"
								type="button"
								onclick={() => addChannel(pIndex)}
							>
								Add Channel
							</button>
						</div>

						<Tabs listClasses="overflow-x-scroll overflow-y-hidden">
							{#snippet list()}
								{#each personality.channels as channel, cIndex}
									<Tabs.Control value={cIndex.toString()}>
										<div class="flex items-center gap-2">
											<span>{channel.number}. {channel.name || channel.type}</span>
											{#if personality.channels.length > 1}
												<button
													type="button"
													class="btn btn-sm preset-outlined-error-500"
													onclick={(e) => {
														e.stopPropagation();
														removeChannel(pIndex, cIndex);
													}}
													title="Remove channel"
												>
													<X size={20} />
												</button>
											{/if}
										</div>
									</Tabs.Control>
								{/each}
							{/snippet}
							{#snippet content()}
								{#each personality.channels as channel, cIndex}
									<Tabs.Panel value={cIndex.toString()}>
										<ChannelEditor
											{channel}
											channelIndex={cIndex}
											onChannelChange={(field, value) =>
												handleChannelChange(pIndex, cIndex, field, value)}
											onChannelTypeChange={() => handleChannelTypeChange(pIndex, cIndex)}
											onRemoveChannel={() => removeChannel(pIndex, cIndex)}
											onAddColorMacro={() => addColorMacro(pIndex, cIndex)}
											onRemoveColorMacro={(macroIndex) =>
												removeColorMacro(pIndex, cIndex, macroIndex)}
											onColorMacroChange={(macroIndex, field, value) =>
												handleColorMacroChange(pIndex, cIndex, macroIndex, field, value)}
											onAddShutterFunction={() => addShutterFunction(pIndex, cIndex)}
											onRemoveShutterFunction={(functionIndex) =>
												removeShutterFunction(pIndex, cIndex, functionIndex)}
											onShutterFunctionChange={(functionIndex, field, value) =>
												handleShutterFunctionChange(pIndex, cIndex, functionIndex, field, value)}
										/>
									</Tabs.Panel>
								{/each}
							{/snippet}
						</Tabs>
					</div>
				</Tabs.Panel>
			{/each}
		{/snippet}
	</Tabs>
</div>
