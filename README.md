# Seamoth Flare System

## Overview

The Seamoth Flare System mod adds a new upgrade module for the Seamoth, allowing you to launch flares from your vehicle. This mod is designed for robust integration, visual polish, and full configuration support.

## Features

- **Seamoth Flare Module**: Craft and install a new upgrade module to launch flares from your Seamoth.
- **Per-slot Visuals**: Each installed module adds a visible mesh to the Seamoth, with robust cleanup and no orphaned visuals.
- **Accurate Flare Launch**: Flares are launched from the correct mesh location, with a configurable Y offset for perfect alignment.
- **Configurable Options**: All key behaviors are adjustable in the mod options menu:
	- Require Crashfish for crafting
	- Flare launch energy cost
	- Flare launch cooldown (seconds)
	- Flare launch force
	- Flare light intensity (brightness)
	- Flare light range (illumination distance)
- **Localization**: All options and UI are localized in English, French, German, Italian, Russian, and Spanish.
- **Robust Asset Management**: Meshes, icons, and materials are loaded from an AssetBundle for easy updates and mod compatibility.
- **Logging and Error Handling**: Extensive logging for asset loading, prefab instantiation, and error conditions.

## Installation

1. Install [BepInEx](https://bepinex.github.io/) and [Nautilus](https://github.com/PrimeSonic/Nautilus) for Subnautica.
2. Place the Seamoth Flare System mod folder in your `BepInEx/plugins` directory.
3. Launch the game. The mod will initialize automatically.

## Configuration

Open the mod options menu in-game to adjust all available settings:

- **Require Crashfish**: If enabled, crafting the module requires a Crashfish.
- **Flare Launch Energy Cost**: How much energy is consumed per flare.
- **Flare Launch Cooldown**: Minimum time between launches.
- **Flare Launch Force**: How fast flares are launched.
- **Flare Light Intensity**: Brightness of launched flares.
- **Flare Light Range**: How far the flare's light reaches.

All options are fully localized.

## Credits

- Developed by visolar6
- Powered by BepInEx, Nautilus, and Harmony

## License

See LICENSE for details.