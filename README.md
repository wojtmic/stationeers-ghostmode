# Ghostmode
Simple utility to make a player immortal, have infinite jetpack fuel and invisible to other players in Stationeers<br>
Made for Largely Unemployed's mod bounty

# Usage
When in-game as the host player, Press **F3** to open the in-game console and type `ghost <playerId>` where `playerId` is printed into the console on player join (Steam IDs will work 90% of the time). Run the command again to disable the ghost status

# Installation
Download and install [BepInEx](https://github.com/BepInEx/BepInEx/releases/) to the game directory, launch the game once and put the .dll file from [releases](https://github.com/wojtmic/stationeers-ghostmode/releases/tag/1.0.0) into `<gameDirectory>/BepInEx/plugins/`.

# Changelog

## v1.1.0
- **Fullbright** — Ghost players see the world at maximum brightness. `RenderSettings.ambientLight` is forced to `Color.white` (intensity 8) every frame, overriding any per-frame overrides the game applies. Toggleable with **L**. Works client-side when the mod is installed on the ghost player's machine.

## v1.0.0
- Initial release: invisibility, god mode, infinite jetpack fuel.
