<script module lang="ts">
	export interface TabInfo {
		id: Symbol;
		label: string;
		closable?: boolean;
		onTabClosed?: () => void;
	}

	export interface TabContext {
		tabs: TabInfo[];
		activeTab: number;
	}
</script>

<script lang="ts">
	import { setContext, type Snippet } from 'svelte';

	export interface TabStripProps {
		showAddButton?: boolean;
		onAddTab?: () => void;
		children: Snippet;
	}

	let { showAddButton = false, onAddTab, children }: TabStripProps = $props();

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

	function handleCloseTab(index: number, event: Event): void {
		event.stopPropagation(); // Prevent tab selection when closing

		const tabToClose = tabs.tabs[index];
		tabToClose.onTabClosed?.();
	}

	function handleAddTab(): void {
		onAddTab?.();
	}
</script>

<div class="tab-strip">
	{#each tabs.tabs as tab, index (tab.id)}
		<div class="tab {index === tabs.activeTab ? 'active' : ''}">
			<button class="tab-button" onclick={() => handleTabClick(index)}>
				<span class="tab-label">{tab.label}</span>
			</button>
			{#if tab.closable}
				<button class="close-button" onclick={(e) => handleCloseTab(index, e)} title="Close tab">
					×
				</button>
			{/if}
		</div>
	{/each}

	{#if showAddButton}
		<button class="add-button" onclick={handleAddTab} title="Add new tab"> + </button>
	{/if}
</div>

<div class="tab-content">
	{@render children?.()}
</div>

<style>
	.tab-strip {
		display: flex;
		border-bottom: 2px solid var(--foreground, black);
	}

	.tab {
		flex: 1;
		display: flex;
		align-items: center;
		background: var(--background, white);
		transition: background 0.3s;
		position: relative;
	}

	.tab:hover {
		background: var(--current-line, lightgray);
	}

	.tab.active {
		font-weight: bold;
		border-bottom: 2px solid var(--cyan, blue);
	}

	.tab-button {
		flex: 1;
		padding: 10px;
		text-align: center;
		cursor: pointer;
		background: none;
		border: none;
		outline: none;
		font: inherit;
		color: inherit;
	}

	.tab.active .tab-button {
		font-weight: bold;
	}

	.tab-label {
		display: block;
	}

	.close-button {
		background: none;
		border: none;
		cursor: pointer;
		font-size: 16px;
		font-weight: bold;
		color: var(--foreground, black);
		opacity: 0.6;
		transition:
			opacity 0.2s,
			background 0.2s;
		border-radius: 50%;
		width: 20px;
		height: 20px;
		display: flex;
		align-items: center;
		justify-content: center;
		line-height: 1;
		margin-right: 8px;
		flex-shrink: 0;
	}

	.close-button:hover {
		opacity: 1;
		background: var(--red, red);
		color: white;
	}

	.add-button {
		background: var(--background, white);
		border: none;
		cursor: pointer;
		font-size: 18px;
		font-weight: bold;
		color: var(--foreground, black);
		padding: 10px 15px;
		transition: background 0.3s;
		border-radius: 0;
		min-width: 45px;
	}

	.add-button:hover {
		background: var(--current-line, lightgray);
	}

	.tab-content {
		padding: 10px;
	}
</style>
