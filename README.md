# Randomizer Map S
Randomizer Map S is a Hollow Knight mod used with Randomizer 4. It helps to find both item and transition checks.

![Example Screenshot](./worldmap.jpg)
![Example Screenshot](./transition.jpg)
![Example Screenshot](./pause.jpg)

This fork of CaptainDapper's original mod has been expanded on with many more features and bug fixes. It is currently compatible with:
- RandomizerMod v4.0.3 (does NOT work with earlier versions)
- AdditionalMaps v1.5.1.0. Highly recommended with this mod to view White Palace checks
- RandomizableLevers v1.1.2.1
- ItemSync v2.2.1
- RandoPlus v1.2.0
- SkillUpgrades v0.9.4.2
- Most other mods that don't change map function

https://github.com/homothetyhk/RandomizerMod
https://github.com/SFGrenade/AdditionalMaps
https://github.com/flibber-hk/HollowKnight.RandomizableLevers
https://github.com/Shadudev/HollowKnight.MultiWorld/tree/itemsync
https://github.com/flibber-hk/HollowKnight.RandoPlus
https://github.com/flibber-hk/HollowKnight.SkillUpgrades

# Quick Start Guide
- Press `CTRL-M` during a game to enable the mod. Alternatively, enable it from the Pause Menu.

# Features
## World Map / Quick Map
- Big Pins: Items are reachable according to RandomizerMod's logic
- Small Pins: Items are not randomized or not reachable
- Pins will disappear as you clear their locations. If item previews are enabled, it will show the corresponding previewed item.
- Check out the [MapModS Legend](./MAPLEGEND.md) for more details on what each pin means, including the border colors.
- Pin settings are displayed at the bottom. See Pause Menu for more info on the toggles.
- ``CTRL-L``: In the World Map, toggle a panel for pin lookup on/off.
    - Hover over any visible pin to display info about the name, room, status, logic requirements, previewed items (if any) and spoiler items (if Spoilers on).
    - This feature is disabled in "Transition" mode.

## Pause Menu
- "Mod Enabled/Disabled": Toggle the mod on/off
- "Spoilers" `CTRL-1`: Toggle pins between vanilla (non-spoiler) and spoiler item pools
- "Randomized" `Ctrl-2`: Toggle all pins for randomized items on/off
- "Others" `Ctrl-3`: Toggle all pins for non-randomized items on/off
- "Pin Style" `CTRL-4`: Toggle the style of the pins
- "Pin Size" `CTRL-5`: Toggle the size of the pins
- "Mode": Toggle the map mode
    - "Transition": For transition rando runs only. See more info below.
    - "Transition 2": For area rando runs only. Instead of showing all in-logic + visited rooms, only visited rooms appear
    - "Full Map": Shows all pins and the full map regardless of map items obtained
    - "All Pins": Shows all pins, but only show map areas for obtained map items
    - "Pins Over Map": Only show pins over where the corresponding map item has been obtained
- "Customize Pins":
    - Toggle each pool on/off.
    - You can control whether the pool toggles are grouped by location, or by item (spoiler).
    - You can also control whether persistent items are always showing or not.

## Transition Mode
- Check out the [MapModS Legend](./MAPLEGEND.md) for more details on what the colors/brightness of each room indicate.
- Hover over the selected room and press your bound `Menu Select` button to find a path. If successful, the path route will be displayed. You can try again to change the start/final transition to what you want.
- The route will also be visible from the Quick Map, and in-game based on the below toggle. As you make the correct transitions, those transitions will get cleared from the route.
- The Quick Map also shows the list of unchecked and visited transitions for the current room.
- `CTRL-B`: Toggle including Benchwarp in the pathfinder on/off.
- `CTRL-R`: Toggle the route to be displayed in-game full/next transition only/off.
- `CTRL-U`: Toggle the panel for unchecked and visited transitions in the World Map. 

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
