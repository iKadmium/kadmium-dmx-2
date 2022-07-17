<button class="button" on:click>
	<slot />
</button>

<style>
	.button {
		padding: 0.5rem 1rem;
		border: none;
		border-radius: 0.5rem;
		cursor: pointer;
		background-color: var(--color-success);
		display: inline-flex;
		align-items: center;
		column-gap: 0.5rem;
	}

	.button:hover {
		filter: brightness(80%);
	}
</style>
