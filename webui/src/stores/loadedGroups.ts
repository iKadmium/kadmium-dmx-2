import { writable } from "svelte/store";

export const loadedGroups = writable<string[]>(undefined)