export const getServiceUri = (path: string) => {
	const port = 5185;
	const hostname = window.location.hostname;
	const scheme = window.location.protocol;
	return new URL(path, `${scheme}//${hostname}:${port}`);
}