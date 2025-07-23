<script lang="ts">
	import type { Channel, ChannelType } from '../types/fixture.js';

	// Props
	interface Props {
		channel: Channel;
		channelIndex: number;
		onChannelChange: (field: string, value: any) => void;
		onChannelTypeChange: () => void;
		onRemoveChannel: () => void;
		onAddColorMacro: () => void;
		onRemoveColorMacro: (macroIndex: number) => void;
		onColorMacroChange: (macroIndex: number, field: string, value: any) => void;
		onAddShutterFunction: () => void;
		onRemoveShutterFunction: (functionIndex: number) => void;
		onShutterFunctionChange: (functionIndex: number, field: string, value: any) => void;
	}

	let {
		channel,
		channelIndex,
		onChannelChange,
		onChannelTypeChange,
		onRemoveChannel,
		onAddColorMacro,
		onRemoveColorMacro,
		onColorMacroChange,
		onAddShutterFunction,
		onRemoveShutterFunction,
		onShutterFunctionChange
	}: Props = $props();

	function getInputValue(event: Event): string {
		return (event.target as HTMLInputElement).value;
	}

	function getSelectValue(event: Event): string {
		return (event.target as HTMLSelectElement).value;
	}

	function handleTypeChange(event: Event): void {
		const newType = getSelectValue(event);
		onChannelChange('type', newType);
		onChannelTypeChange();
	}
</script>

<div class="card space-y-3 p-4">
	<div class="flex items-center justify-between">
		<h5 class="text-base font-semibold">
			Channel {channel.number}: {channel.type !== 'other'
				? channel.type
				: channel.name || 'Unnamed'}
		</h5>
	</div>

	<div class="grid grid-cols-1 gap-3 md:grid-cols-2">
		<label class="label">
			<span class="label-text">Type</span>
			<select class="select" value={channel.type} onchange={handleTypeChange}>
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

		<label class="label">
			<span class="label-text">Channel Number</span>
			<input
				class="input"
				type="number"
				min="1"
				value={channel.number}
				oninput={(e) => onChannelChange('number', parseInt(getInputValue(e)))}
			/>
		</label>

		{#if channel.type === 'other'}
			<label class="label md:col-span-2">
				<span class="label-text">Custom Name</span>
				<input
					type="text"
					class="input"
					value={channel.name || ''}
					oninput={(e) => onChannelChange('name', getInputValue(e))}
				/>
			</label>
		{/if}
	</div>

	{#if channel.type === 'color-macro'}
		<div class="space-y-3">
			<div class="flex items-center justify-between">
				<h6 class="text-sm font-medium">Color Macros</h6>
				<button class="btn btn-sm variant-filled-secondary" type="button" onclick={onAddColorMacro}>
					Add Macro
				</button>
			</div>

			<div class="space-y-2">
				{#each channel.colorMacros as macro, mIndex}
					<div class="card variant-soft p-3">
						<div class="mb-2 flex items-center justify-between">
							<span class="text-sm font-medium">Macro {mIndex + 1}</span>
							{#if channel.colorMacros.length > 1}
								<button
									class="btn btn-sm preset-outlined-error-500"
									type="button"
									onclick={() => onRemoveColorMacro(mIndex)}
								>
									Remove
								</button>
							{/if}
						</div>

						<div class="grid grid-cols-3 gap-2">
							<label class="label">
								<span class="label-text text-xs">Color</span>
								<input
									type="color"
									class="input h-8"
									value={macro.color}
									oninput={(e) => onColorMacroChange(mIndex, 'color', getInputValue(e))}
								/>
							</label>

							<label class="label">
								<span class="label-text text-xs">Min</span>
								<input
									type="number"
									class="input"
									min="0"
									max="255"
									value={macro.min}
									oninput={(e) => onColorMacroChange(mIndex, 'min', parseInt(getInputValue(e)))}
								/>
							</label>

							<label class="label">
								<span class="label-text text-xs">Max</span>
								<input
									type="number"
									class="input"
									min="0"
									max="255"
									value={macro.max}
									oninput={(e) => onColorMacroChange(mIndex, 'max', parseInt(getInputValue(e)))}
								/>
							</label>
						</div>
					</div>
				{/each}
			</div>
		</div>
	{/if}

	{#if channel.type === 'shutter'}
		<div class="space-y-3">
			<div class="flex items-center justify-between">
				<h6 class="text-sm font-medium">Shutter Functions</h6>
				<button
					class="btn btn-sm preset-filled-secondary-500"
					type="button"
					onclick={onAddShutterFunction}
				>
					Add Function
				</button>
			</div>

			<div class="space-y-2">
				{#each channel.shutterFunctions as shutterFunction, oIndex}
					<div class="card variant-soft p-3">
						<div class="mb-2 flex items-center justify-between">
							<span class="text-sm font-medium">Function {oIndex + 1}</span>
							{#if channel.shutterFunctions.length > 1}
								<button
									class="btn btn-sm preset-outlined-error-500"
									type="button"
									onclick={() => onRemoveShutterFunction(oIndex)}
								>
									Remove
								</button>
							{/if}
						</div>

						<div class="grid grid-cols-3 gap-2">
							<label class="label">
								<span class="label-text text-xs">Type</span>
								<select
									class="select"
									value={shutterFunction.name}
									onchange={(e) => onShutterFunctionChange(oIndex, 'name', getSelectValue(e))}
								>
									<option value="strobe">Strobe</option>
									<option value="dimmer">Dimmer</option>
								</select>
							</label>

							<label class="label">
								<span class="label-text text-xs">Min</span>
								<input
									class="input"
									type="number"
									min="0"
									max="255"
									value={shutterFunction.min}
									oninput={(e) =>
										onShutterFunctionChange(oIndex, 'min', parseInt(getInputValue(e)))}
								/>
							</label>

							<label class="label">
								<span class="label-text text-xs">Max</span>
								<input
									class="input"
									type="number"
									min="0"
									max="255"
									value={shutterFunction.max}
									oninput={(e) =>
										onShutterFunctionChange(oIndex, 'max', parseInt(getInputValue(e)))}
								/>
							</label>
						</div>
					</div>
				{/each}
			</div>
		</div>
	{/if}
</div>

<style>
	/* Ensure responsive behavior for grids */
	@media (max-width: 768px) {
		:global(.md\\:grid-cols-2) {
			grid-template-columns: 1fr !important;
		}

		:global(.grid-cols-3) {
			grid-template-columns: 1fr !important;
		}

		:global(.md\\:col-span-2) {
			grid-column: span 1 !important;
		}
	}
</style>
