<script lang="ts">
	import '../app.css';

	let { children } = $props();
</script>

<div class="grid h-screen grid-rows-[auto_1fr_auto]">
	<main class="space-y-4 p-4">
		{@render children()}
	</main>
</div>

<style>
</style>
