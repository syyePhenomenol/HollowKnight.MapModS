# Randomizer Map S
![GitHub release (latest by date)](https://img.shields.io/github/v/release/syyePhenomenol/HollowKnight.MapModS)
![Downloads GitHub all releases](https://img.shields.io/github/downloads/syyePhenomenol/HollowKnight.MapModS/total)

Randomizer Map S is a Hollow Knight mod used with Randomizer 4. It helps to find both item and transition checks.

This fork of CaptainDapper's original mod has been expanded on with many more features and bug fixes.

Dependencies:
- [RandomizerMod v4.0.3](https://github.com/homothetyhk/RandomizerMod) and all of its dependencies. Does NOT work with earlier versions
- [ConnectionMetadataInjector v1.1.8107.6258](https://github.com/BadMagic100/ConnectionMetadataInjector). Does NOT work with earlier versions
- [MagicUI v1.4.8178.42957](https://github.com/BadMagic100/HollowKnight.MagicUI). Does NOT work with earlier versions
- [Vasi](https://github.com/fifty-six/HollowKnight.Vasi)

Compatible optional mods:
- [AdditionalMaps](https://github.com/SFGrenade/AdditionalMaps). Highly recommended with this mod to view White Palace checks
- [RandomizableLevers](https://github.com/flibber-hk/HollowKnight.RandomizableLevers). Shows lever checks on map
- [RandoPlus](https://github.com/flibber-hk/HollowKnight.RandoPlus). Shows Mr Mushroom checks on map
- Most other mods

This mod has support for languages other than English. You will need to source a `language.json` file and copy it to the `Randomizer 4` directory.

# Screenshots
<details>
<summary>Click to expand</summary>
    <img src="./ReadmeAssets/worldmap.jpg" alt="World Map">
    <img src="./ReadmeAssets/quickmap.jpg" alt="Quick Map">
    <img src="./ReadmeAssets/transition.jpg" alt="Transition Mode">
    <img src="./ReadmeAssets/pause.jpg" alt="Pause Menu">
</details>

# Quick Start Guide
- Press `CTRL-M` during a game to enable the mod. Alternatively, enable it from the Pause Menu.
- [fireb0rn's quick MapModS guide (item randomizer)](https://www.youtube.com/watch?v=z35cFvU0McQ&t=1113s)
- [fireb0rn's quick MapModS guide (transition randomizer)](https://www.youtube.com/watch?v=z35cFvU0McQ&t=1195s)

# Features
## World Map / Quick Map
- Big Pins: Items are reachable according to RandomizerMod's logic
- Small Pins: Items are not randomized or not reachable
- Pins will disappear as you clear their locations. If item previews are enabled, it will show the corresponding previewed item.
- Pin settings are displayed at the bottom. See Pause Menu for more info on the toggles.
- ``CTRL-H``: Expand/collapse the hotkey panel
- ``SHIFT``: Pan faster (same as right thumbstick on controller).
- ``CTRL-K``: Toggle a panel for the map key for the pin and room colors.
    - Check out the [Map Legend](./MAPLEGEND.md) for more details on each pin style.
- ``CTRL-L``: Toggle a panel for pin lookup on/off.
    - Hover over any visible pin to display info about the name, room, status, logic requirements, previewed items (if any) and spoiler items (if Spoilers on).

## Pause Menu
- "Mod Enabled/Disabled" `CTRL-M`: Toggle the mod on/off
- "Spoilers" `CTRL-1`: Toggle pins between vanilla (non-spoiler) and spoiler item pools
- "Randomized" `CTRL-2`: Toggle all pins for randomized items on/off
- "Others" `CTRL-3`: Toggle all pins for non-randomized items on/off
- "Pin Style" `CTRL-4`: Toggle the style of the pins
- "Pin Size" `CTRL-5`: Toggle the size of the pins
- "Mode": Toggle the map mode
    - "Transition": See more info below.
    - "Transition 2": Instead of showing all in-logic + visited rooms, only visited rooms appear. Depending on your randomizer run, this might not change anything (and therefore may not be an option).
    - "Full Map": Shows all pins and the full map regardless of map items obtained
    - "All Pins": Shows all pins, but only show map areas for obtained map items
    - "Pins Over Map": Only show pins over where the corresponding map item has been obtained
- "Customize Pins":
    - Toggle each pool on/off.
    - Control whether the pool toggles are grouped by location, or by item (spoiler).
    - Control whether persistent items are always showing or not.

## Transition Mode
- Check out the in-game map key (`CTRL-K`) or the [Map Legend](./MAPLEGEND.md) for what the colors/brightness of each room indicate.
- Hover over a room and press the indicated key or button to find a path.
- If successful, the path route will be displayed. You can try again to change the start/final transition to what you want.
- A compass arrow will point to the next transition you need to go to.
- As you make the correct transitions, they will get cleared from the route.
- The Quick Map also shows the list of unchecked and visited transitions for the current room.
- `CTRL-B`: Toggle including Benchwarp in the pathfinder on/off.
- `CTRL-R`: Toggle the route to be displayed in-game full/next transition only/off.
- `CTRL-U`: Show/hide unchecked and visited transitions in the World Map.
- `CTRL-C`: Toggle the route compass on/off.

# How To Install
Use Scarab: https://github.com/fifty-six/Scarab

Or, you can install manually:
1. Download the latest release of `MapModS.zip`.
2. Unzip and copy the folder 'MapModS' to `...\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods`.

# Acknowledgements
- Special thanks to Homothety and Flib for significant coding help
- CaptainDapper for making the original mod
- PimpasPimpinela for helping to port the mod from Rando 3 to Rando 4
- Chaktis, KingKiller39 and Ender Onryo for helping with sprite art
- ColetteMSLP for testing out the mod during livestreams
- BadMagic for CMICore/MagicUI and help with the UI migration
