/// <reference types="vite/client" />

export const getServiceUri = (path: string) => {
	let apiRoot: URL;
	if (import.meta.env.VITE_API_ROOT) {
		apiRoot = new URL(import.meta.env.VITE_API_ROOT);
	}
	else if (import.meta.env.DEV) {
		apiRoot = new URL("/api/", 'http://localhost:5185');
	}
	else {
		apiRoot = new URL("/api/", window.location.toString());
	}
	return new URL(path, apiRoot);
}