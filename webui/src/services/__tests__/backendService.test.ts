import { describe, expect, it, vi } from 'vitest';
import { BackendService } from '../backendService';

interface ITestKey {
	key: string;
}

interface ITestValue {
	value: string;
}

class TestBackendService extends BackendService<ITestKey, ITestValue>
{
	async getBaseUri(): Promise<URL> {
		const url = new URL("/test", await this.apiRoot);
		return Promise.resolve(url);
	}

}

describe("Create", () => {
	it("should make a request to the root url (/service)", async () => {
		const expectedUrl = new URL("http://localhost/test");

		const fetchMock = vi.fn()
			.mockResolvedValue({
				json: vi.fn().mockResolvedValue({})
			});
		vi.stubGlobal("fetch", fetchMock);

		const instance = new TestBackendService(Promise.resolve(new URL("http://localhost")))
		await instance.create({
			value: "someone"
		});

		expect(fetchMock).toHaveBeenCalledWith(expectedUrl, expect.anything());
	})
})