import { writable } from "svelte/store";
import type { IVenue } from "../models/venue";

export const loadedVenue = writable<IVenue>(undefined)