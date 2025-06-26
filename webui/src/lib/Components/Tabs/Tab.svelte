<script module lang="ts">
	export interface TabProps {
		label: string;
		closable?: boolean;
		onClosed?: () => void;
		children: Snippet;
	}
</script>

<script lang="ts">
	import { getContext, onDestroy, onMount, type Snippet } from 'svelte';
	import type { TabInfo, TabContext } from './TabStrip.svelte';

	let { children, label, closable }: TabProps = $props();

	const tabs = getContext<TabContext>('tabs');
	const info = $state<TabInfo>({
		id: Symbol(),
		label: label,
		closable: closable ?? false
	});

	onMount(() => {
		if (tabs?.tabs === undefined) return;
		tabs.tabs.push(info);
	});

	onDestroy(() => {
		if (tabs?.tabs === undefined) return;
		tabs.tabs = tabs.tabs.filter((tab) => tab.id !== info.id);
	});

	$effect(() => {
		info.label = label;
		info.closable = closable;
	});
</script>

{#if tabs.activeTab === tabs.tabs.indexOf(info)}
	<div class="tab-content">
		{@render children()}
	</div>
{/if}
