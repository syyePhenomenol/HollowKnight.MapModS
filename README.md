# Randomizer Map S
Randomizer Map S is a Hollow Knight mod used with Randomizer 4. It gives the option to show the player where items are on the World Map, and optionally what they are.

![Example Screenshot](./worldmap.jpg)
![Example Screenshot](./quickmap.jpg)
![Example Screenshot](./pause.jpg)

This fork of CaptainDapper's original mod has been expanded on with more features, bug fixes and a Pause Menu UI. It is currently compatible with:
- RandomizerMod v4.0.2
- AdditionalMaps v1.5.1.0
- ItemSync 2.1.1
- RandoPlus v1.1.0
- SkillUpgrades v0.9.3

https://github.com/homothetyhk/RandomizerMod

https://github.com/SFGrenade/AdditionalMaps

https://github.com/Shadudev/HollowKnight.MultiWorld/tree/itemsync

https://github.com/flibber-hk/HollowKnight.RandoPlus

https://github.com/flibber-hk/HollowKnight.SkillUpgrades

# Quick Start Guide
- Press `CTRL-M` during a game to enable the mod. Alternatively, enable it from the Pause Menu.

# Features
- The World Map will now show Pins for every item check.
    - Big Pins means the items are reachable according to RandomizerMod's logic
    - Small Pins means the items are not randomized or not reachable
    - Pins will disappear as you check their locations
    - MapMod S settings are displayed at the bottom

- New to MapModS for Rando 4:
    - Previewed items will appear as pins with a green border
    - Out-of-logic items will appear as pins with a red border

- The Pause Menu UI has the following buttons:
    - "Mod Enabled/Disabled": Toggle the mod on/off
    - "Spoilers" `CTRL-1`: Toggle Pins between vanilla (non-spoiler) and spoiler item pools
    - "Randomized" `Ctrl-2`: Toggle all Pins for randomized items on/off
    - "Others" `Ctrl-3`: Toggle all Pins for non-randomized items on/off
    - "Pin Style" `CTRL-4`: Toggle the style of the Pins
    - "Pin Size" `CTRL-5`: Toggle the style of the Pins
    - "Customize Pins": Open/close a panel with a toggle for each spoiler item pool

- The mod currently has four main modes:
   - "Transition": For transition rando runs only
   - "Full Map": Shows the full map regardless of map items obtained
   - "All Pins": Shows all pins, but only show map areas for obtained map items
   - "Pins Over Map": Only show pins over where the corresponding map item has been obtained

- "Transition" mode is a new mode in MapModS for Rando 4 that displays visited rooms with color-coding:
    - Current room is green
    - Adjacent visited rooms are cyan
    - Rooms containing unchecked reachable transitions are brighter
    - Out of logic/sequence break rooms are red
    - Other visited rooms are a standard grey/white
 
- In "Transition" mode, you also have a route searcher in the World Map that allows you to find a sequence of transitions to get to any other selected room on the map.
    - Hover over the desired room and press `CTRL-T` to attempt a route search.
    - If successful, the route will be displayed.
    - You can try again (press `CTRL-T` again) to change the starting transition if you cannot reach it at the moment.

# How To Install
Use Scarab: https://github.com/fifty-six/Scarab

(if the latest version of MapMod S is a pre-release, it will not be on Scarab yet)

Or, you can install manually:
1. Download the latest release of `MapModS.zip`.
2. Unzip and copy the folder 'MapModS' to `...\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods`.

# Acknowledgements
- The Hollow Knight/Hollow Knight Speedrun Discord Channels for always giving very sound advice and suggestions
- Special thanks to Homothety and Flib for significant coding help
- CaptainDapper for making the original mod
- PimpasPimpinela for helping to port the mod from Rando 3 to Rando 4
- Chaktis, KingKiller39 and Ender Onryo for helping with sprite art
- ColetteMSLP for testing out the mod during livestreams
