/// <reference types="vite/client" />

export abstract class BackendService<TKey, TObject> {
	protected apiRoot: Promise<URL>;

	constructor(apiRoot: Promise<URL>) {
		this.apiRoot = apiRoot;
	}

	abstract getBaseUri(): Promise<URL>;

	public async create(obj: TObject): Promise<TObject> {
		const uri = await this.getBaseUri();
		const result = await fetch(
			uri,
			{
				headers: {
					'Content-Type': 'application/json'
				},
				method: 'POST',
				body: JSON.stringify(obj),
			}
		);
		const json = await result.json() as TObject;
		return json;
	}

	public async readKeys() {
		const uri = new URL(await this.getBaseUri());
		const result = await fetch(uri);
		const json = await result.json() as TKey[];
		return json;
	}

	public async read(id: string) {
		const uri = new URL(id, await this.getBaseUri());
		const result = await fetch(uri);
		const json = await result.json() as TObject;
		return json;
	}

	public async update(id: string, obj: TObject): Promise<TObject> {
		const uri = new URL(id, await this.getBaseUri());
		const result = await fetch(
			uri,
			{
				headers: {
					'Content-Type': 'application/json'
				},
				method: 'PUT',
				body: JSON.stringify(obj),
			}
		);
		const json = await result.json() as TObject;
		return json;
	}

	public async delete(id: string): Promise<void> {
		const uri = new URL(id, await this.getBaseUri());
		await fetch(
			uri,
			{
				method: 'DELETE'
			}
		);
	}
}