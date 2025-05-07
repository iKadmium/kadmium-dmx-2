<script module lang="ts">
	export interface TabInfo {
		id: Symbol;
		label: string;
	}

	export interface TabContext {
		tabs: TabInfo[];
		activeTab: number;
	}
</script>

<script lang="ts">
	import { setContext } from 'svelte';

	let { children } = $props();

	const tabs = $state<TabContext>({
		tabs: [],
		activeTab: 0
	});

	setContext('tabs', tabs);

	function handleTabClick(index: number): void {
		if (index !== tabs.activeTab) {
			tabs.activeTab = index;
		}
	}
</script>

<div class="tab-strip">
	{#each tabs.tabs as tab, index (index)}
		<button
			class="tab {index === tabs.activeTab ? 'active' : ''}"
			onclick={() => handleTabClick(index)}
		>
			{tab.label}
		</button>
	{/each}
</div>

<div class="tab-content">
	{@render children()}
</div>

<style>
	.tab-strip {
		display: flex;
		border-bottom: 2px solid var(--foreground, black);
	}

	.tab {
		flex: 1;
		padding: 10px;
		text-align: center;
		cursor: pointer;
		background: var(--background, white);
		border: none;
		outline: none;
		transition: background 0.3s;
	}

	.tab:hover {
		background: var(--current-line, lightgray);
	}

	.tab.active {
		font-weight: bold;
		border-bottom: 2px solid var(--cyan, blue);
	}

	.tab-content {
		padding: 10px;
	}
</style>
