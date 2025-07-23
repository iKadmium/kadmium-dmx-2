// Define types for the lighting fixture definition
export type FixtureType = 'LED' | 'Incandescent' | 'Effect' | 'Other';
export type ChannelType =
    | 'red'
    | 'green'
    | 'blue'
    | 'uv'
    | 'white'
    | 'amber'
    | 'color-macro'
    | 'shutter'
    | 'other';
export type ShutterFunction = 'strobe' | 'dimmer';

export type ColorMacro = {
    color: string;
    min: number;
    max: number;
};

export type Channel = {
    number: number;
    name?: string;
    type: ChannelType;
    colorMacros: ColorMacro[];
    shutterFunctions: { name: ShutterFunction; min: number; max: number }[];
};

export type Personality = {
    name: string;
    channels: Channel[];
};

export type Fixture = {
    manufacturer: string;
    model: string;
    type: FixtureType;
    personalities: Personality[];
    movementAxis: string[];
};
