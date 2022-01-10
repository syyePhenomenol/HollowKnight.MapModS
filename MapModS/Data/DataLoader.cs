using RandomizerMod.RandomizerData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapModS.Data
{
    public static class DataLoader
    {
        private static Dictionary<string, PinDef> _pins;

        public static PinDef[] GetPinArray()
        {
            return _pins.Values.ToArray();
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

                case "Grimmchild1":
                case "Grimmchild2":
                    return PoolGroup.Charms;

                case "Grub":
                    return PoolGroup.Grubs;

                case "One_Geo":
                    return PoolGroup.GeoChests;

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

                        // MapModS.Instance.Log(group);

                        return group;
                    }
                }
            }

            MapModS.Instance.LogWarn($"{cleanItemName} not found in PoolDefs");

            return PoolGroup.Unknown;
        }

        public static PoolGroup GetVanillaPoolGroup(string location)
        {
            string cleanItemName = location.Split('-')[0];

            //MapModS.Instance.Log(cleanItemName);

            return GetPoolGroup(cleanItemName);
        }

        public static PoolGroup GetRandomizerPoolGroup(string location)
        {
            if (RandomizerMod.RandomizerMod.RS.Context.itemPlacements.Any(pair => pair.location.Name == location))
            {
                RandomizerCore.ItemPlacement ilp = RandomizerMod.RandomizerMod.RS.Context.itemPlacements.First(pair => pair.location.Name == location);

                string cleanItemName = ilp.item.Name.Replace("Placeholder-", "").Split('-')[0];

                //MapModS.Instance.Log(cleanItemName);

                return GetPoolGroup(cleanItemName);
            }

            //MapModS.Instance.LogWarn($"Unable to find RandomizerItemDef for {location}. Either it's not randomized or there is a bug");

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
                    pinD.spoilerPool = GetRandomizerPoolGroup(vanillaItem);

                    if (pinD.vanillaPool == PoolGroup.Unknown || pinD.spoilerPool == PoolGroup.Unknown)
                    {
                        MapModS.Instance.LogWarn($"Location {pinD.name} doesn't seem to have a valid pool.");
                    }
                }
            }

            // Handle special cases
            _pins["Egg_Shop"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.EggShop;

            _pins["Elevator_Pass"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.RandomizeElevatorPass;

            _pins["Focus"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.RandomizeFocus;

            _pins["Mantis_Claw"].disable = RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitClaw;

            _pins["Left_Mantis_Claw"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitClaw;
            _pins["Right_Mantis_Claw"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitClaw;

            _pins["Split_Mothwing_Cloak"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitCloak;

            _pins["Split_Crystal_Heart"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.SplitSuperdash;

            _pins["World_Sense"].disable = !RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Dreamers;
        }

        public static void Load()
        {
            _pins = JsonUtil.Deserialize<Dictionary<string, PinDef>>("MapModS.Resources.pins.json");

            foreach (KeyValuePair<string, PinDef> pin in _pins)
            {
                if (pin.Value.mapZone == GlobalEnums.MapZone.NONE)
                {
                    MapModS.Instance.Log($"{pin.Value.name} is missing mapZone");
                }
            }
        }
    }
}