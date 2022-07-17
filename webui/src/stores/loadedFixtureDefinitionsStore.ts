import { writable } from "svelte/store";
import type { IFixtureDefinition } from "../models/fixtureDefinition";

export const loadedVenue = writable<IFixtureDefinition>(undefined)