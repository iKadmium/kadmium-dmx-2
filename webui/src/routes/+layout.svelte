<script lang="ts">
	import '../app.css';

	let { children } = $props();
</script>

<div class="main-layout">
	{@render children()}
</div>

<style>
	.main-layout {
		display: flex;
		flex-direction: column;
		align-items: center;
		width: 100%;
		max-width: 1200px;
	}
</style>
