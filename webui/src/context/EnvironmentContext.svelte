<script context="module" lang="ts">
	export const EnvironmentContextKey = Symbol();
	export interface IEnvironmentContext {
		getApiRoot: () => Promise<URL>;
	}
</script>

<script lang="ts">
	import { onMount, setContext } from 'svelte';

	const contextValue: IEnvironmentContext = {
		getApiRoot: () =>
			new Promise<URL>((resolve) => {
				onMount(() => {
					if (import.meta.env.VITE_API_ROOT) {
						resolve(new URL(import.meta.env.VITE_API_ROOT));
					} else if (import.meta.env.DEV) {
						resolve(new URL('/api/', 'http://localhost:5185'));
					} else {
						resolve(new URL('/api/', window.location.toString()));
					}
				});
			})
	};

	setContext<IEnvironmentContext>(EnvironmentContextKey, contextValue);
</script>

{#if contextValue}
	<slot />
{/if}
