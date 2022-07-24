<script lang="ts">
	import GoTrashcan from 'svelte-icons/go/GoTrashcan.svelte';

	import { AccordionItem, Button } from 'sveltestrap';
	import IconContainer from './IconContainer.svelte';
	export let title: string;
</script>

<AccordionItem>
	<div slot="header" class="accordion-header">
		<span class="accordion-title">{title}</span>
		<button class="btn btn-danger" on:click|preventDefault|stopPropagation>
			<IconContainer><GoTrashcan /></IconContainer>
		</button>
	</div>
	<slot />
</AccordionItem>

<style>
	.accordion-header {
		display: flex;
		align-content: space-between;
		width: 100%;
		margin-right: 1rem;
	}
	.accordion-title {
		flex-grow: 1;
	}
</style>
