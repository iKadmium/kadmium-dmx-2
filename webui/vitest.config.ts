import { defineConfig, UserConfigExport } from "vite"
import { extractFromSvelteConfig } from "vitest-svelte-kit"

const options: UserConfigExport = {
	...extractFromSvelteConfig(),
	test: {
		coverage: {
			reporter: ["lcovonly"]
		}
	}
}

export default defineConfig(options);