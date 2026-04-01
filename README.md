# Ghostmode
Simple utility to make a player immortal, have infinite jetpack fuel and invisible to other players in Stationeers<br>
Made for Largely Unemployed's mod bounty

# Usage
When in-game as the host player, Press **F3** to open the in-game console and type `ghost <playerId>` where `playerId` is printed into the console on player join (Steam IDs will work 90% of the time). Run the command again to disable the ghost status

# Installation
Download and install [BepInEx](https://github.com/BepInEx/BepInEx/releases/) to the game directory, launch the game once and put the .dll file from [releases](https://github.com/wojtmic/stationeers-ghostmode/releases/tag/1.0.0) into `<gameDirectory>/BepInEx/plugins/`.

# Changelog

## v1.1.0
- **Noclip** — Ghost players phase through geometry while their jetpack is actively thrusting. Colliders are automatically restored the moment the jetpack stops. Implemented via a `NoclipController` MonoBehaviour that is attached to the ghost's `gameObject` and driven by signals from the existing `Jetpack.OnAtmosphericTick` patch.
- **Fullbright** — Ghost players see the world at maximum brightness. `RenderSettings.ambientLight` is forced to `Color.white` (intensity 8) every frame via a `FullbrightController` MonoBehaviour, overriding any per-frame overrides the game applies. Original lighting is restored when ghost mode is disabled. *Note: because the mod runs host-side, fullbright only affects the host player's view.*

## v1.0.0
- Initial release: invisibility, god mode, infinite jetpack fuel.
