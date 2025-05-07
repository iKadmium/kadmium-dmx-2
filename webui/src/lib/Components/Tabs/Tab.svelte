<script lang="ts">
	import { getContext, onDestroy, onMount } from 'svelte';
	import type { TabInfo, TabContext } from './TabStrip.svelte';

	let { children, label } = $props();

	const tabs = getContext<TabContext>('tabs');
	const info = $state<TabInfo>({
		id: Symbol(),
		label: label
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
	});
</script>

{#if tabs.activeTab === tabs.tabs.indexOf(info)}
	<div class="tab-content">
		{@render children()}
	</div>
{/if}
