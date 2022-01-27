using RandomizerMod.RandomizerData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapModS.Data
{
    public static class DataLoader
    {
        private static Dictionary<string, PinDef> _pins;
        private static Dictionary<string, PinDef> _pinsAM;

        public static PinDef[] GetPinArray()
        {
            return _pins.Values.ToArray();
        }

        public static PinDef[] GetPinAMArray()
        {
            return _pinsAM.Values.ToArray();
        }

        // Uses RandomizerData to get the PoolGroup from an item name
        public static PoolGroup GetPoolGroup(string cleanItemName)
        {
            switch (cleanItemName)
            {
                case "Dreamer":
                    return PoolGroup.Dreamers;

                case "Split_Mothwing_Cloak":
                case "Split_Crystal_Heart":
                    return PoolGroup.Skills;

                case "Double_Mask_Shard":
                case "Full_Mask":
                    return PoolGroup.MaskShards;

                case "Double_Vessel_Fragment":
                case "Full_Soul_Vessel":
                    return PoolGroup.VesselFragments;

                case "Grimmchild1":
                case "Grimmchild2":
                    return PoolGroup.Charms;

                case "Grub":
                    return PoolGroup.Grubs;

                case "One_Geo":
                    return PoolGroup.GeoChests;

                case "Mr_Mushroom_Level_Up":
                case "Mr_Mushroom":
                    return PoolGroup.LoreTablets;

                case "DirectionalDash":
                case "DownwardFireball":
                case "ExtraAirDash":
                case "HorizontalDive":
                case "SpiralScream":
                case "TripleJump":
                case "VerticalSuperdash":
                case "WallClimb":
                    return PoolGroup.Skills;

                default:
                    break;
            }

            foreach (PoolDef poolDef in RandomizerMod.RandomizerData.Data.Pools)
            {
                foreach (string includeItem in poolDef.IncludeItems)
                {
                    if (includeItem.StartsWith(cleanItemName))
                    {
                        PoolGroup group = (PoolGroup)Enum.Parse(typeof(PoolGroup), poolDef.Group);

                        return group;
                    }
                }
            }

            MapModS.Instance.LogWarn($"PoolGroup not found for an item");

            return PoolGroup.Unknown;
        }

        public static PoolGroup GetVanillaPoolGroup(string location)
        {
            string cleanItemName = location.Split('-')[0];

            return GetPoolGroup(cleanItemName);
        }

        public static PoolGroup GetRandomizerPoolGroup(string location, bool getLast)
        {
            if (RandomizerMod.RandomizerMod.RS.Context.itemPlacements.Any(pair => pair.location.Name == location))
            {
                RandomizerCore.ItemPlacement ilp;

                if (getLast)
                {
                    ilp = RandomizerMod.RandomizerMod.RS.Context.itemPlacements.Last(pair => pair.location.Name == location);
                }
                else
                {
                    ilp = RandomizerMod.RandomizerMod.RS.Context.itemPlacements.First(pair => pair.location.Name == location);
                }

                string cleanItemName = ilp.item.Name.Replace("Placeholder-", "").Split('-')[0];

                return GetPoolGroup(cleanItemName);
            }

            return GetVanillaPoolGroup(location);
        }

        // This method finds the vanilla and spoiler PoolGroups corresponding to each Pin, using RandomizerMod's ItemPlacements array
        // Called once per save load
        public static void FindPoolGroups()
        {
            foreach (KeyValuePair<string, PinDef> entry in _pins)
            {
                string vanillaItem = entry.Key;
                PinDef pinD = entry.Value;

                // First check if this is a shop pin
                if (pinD.isShop)
                {
                    pinD.vanillaPool = PoolGroup.Shop;
                    pinD.spoilerPool = PoolGroup.Shop;
                }
                // Then check if this item is randomized
                else
                {
                    pinD.vanillaPool = GetVanillaPoolGroup(vanillaItem);
                    pinD.spoilerPool = GetRandomizerPoolGroup(vanillaItem, false);

                    if (pinD.vanillaPool == PoolGroup.Unknown || pinD.spoilerPool == PoolGroup.Unknown)
                    {
                        MapModS.Instance.LogWarn($"A location doesn't seem to have a valid pool.");
                    }
                }

                if (pinD.name == "Focus")
                {
                    pinD.spoilerPool = GetRandomizerPoolGroup("Lore_Tablet-King's_Pass_Focus", true);
                }
            }

            // Disable pins based on Randomizer settings
            _pins["Egg_Shop"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.EggShop;

            _pins["Elevator_Pass"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.RandomizeElevatorPass;

            _pins["Focus"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.RandomizeFocus;

            _pins["Mantis_Claw"].disable = RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitClaw;

            _pins["Left_Mantis_Claw"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitClaw;
            _pins["Right_Mantis_Claw"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitClaw;

            _pins["Split_Mothwing_Cloak"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitCloak;

            _pins["Split_Crystal_Heart"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitSuperdash;

            _pins["World_Sense"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Dreamers
                || (RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Dreamers && RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.LoreTablets);

            bool disableMushroomLocations = !RandomizerMod.RandomizerMod.RS.Context.itemPlacements.Any(pair => pair.location.Name.Contains("Mr_Mushroom"));

            _pins["Mr_Mushroom-Fungal_Wastes"].disable = disableMushroomLocations;
            _pins["Mr_Mushroom-Kingdom's_Edge"].disable = disableMushroomLocations;
            _pins["Mr_Mushroom-Deepnest"].disable = disableMushroomLocations;
            _pins["Mr_Mushroom-Howling_Cliffs"].disable = disableMushroomLocations;
            _pins["Mr_Mushroom-Ancient_Basin"].disable = disableMushroomLocations;
            _pins["Mr_Mushroom-Fog_Canyon"].disable = disableMushroomLocations;
            _pins["Mr_Mushroom-King's_Pass"].disable = disableMushroomLocations;
        }

        public static void Load()
        {
            _pins = JsonUtil.Deserialize<Dictionary<string, PinDef>>("MapModS.Resources.pins.json");
            _pinsAM = JsonUtil.Deserialize<Dictionary<string, PinDef>>("MapModS.Resources.pinsAM.json");
        }
    }
}