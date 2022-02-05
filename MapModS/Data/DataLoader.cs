using RandomizerMod.RandomizerData;
using System;
using System.Collections.Generic;
using System.Linq;
using ItemChanger;
using RandomizerCore;
using RandomizerMod.IC;

namespace MapModS.Data
{
    public static class DataLoader
    {
        private static Dictionary<string, PinDef> _allPins;
        private static Dictionary<string, PinDef> _allPinsAM;
        private static Dictionary<string, PinDef> _usedPins = new();

        //public static Dictionary<string, PinDef> newPins = new();

        private static readonly HashSet<string> shopLocations = new()
        {
            "Sly",
            "Sly_(Key)",
            "Iselda",
            "Salubra",
            "Leg_Eater",
            "Grubfather",
            "Seer",
            "Egg_Shop"
        };

        public static PinDef[] GetPinArray()
        {
            return _allPins.Values.ToArray();
        }

        public static PinDef[] GetPinAMArray()
        {
            return _allPinsAM.Values.ToArray();
        }

        public static PinDef[] GetUsedPinArray()
        {
            return _usedPins.Values.ToArray();
        }

        public static PinDef GetUsedPinDef(string locationName)
        {
            if (_usedPins.TryGetValue(locationName, out PinDef pinDef))
            {
                return pinDef;
            }

            return default;
        }

        // Uses RandomizerData to get the PoolGroup from an item name
        public static PoolGroup GetPoolGroup(string cleanItemName)
        {
            if (shopLocations.Contains(cleanItemName)) return PoolGroup.Shop;

            switch (cleanItemName)
            {
                case "Dreamer":
                    return PoolGroup.Dreamers;

                case "Split_Mothwing_Cloak":
                case "Split_Crystal_Heart":
                case "Downslash":
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

                case "Lever":
                case "Switch":
                    return PoolGroup.Levers;

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

            MapModS.Instance.LogWarn($"PoolGroup not found for an item: " + cleanItemName);

            return PoolGroup.Unknown;
        }

        public static PoolGroup GetLocationPoolGroup(string location)
        {
            string cleanItemName = location.Split('-')[0];

            return GetPoolGroup(cleanItemName);
        }

        public static PoolGroup GetItemPoolGroup(string item)
        {
            string cleanItemName = item.Replace("Placeholder-", "").Split('-')[0];

            return GetPoolGroup(cleanItemName);
        }

        // Next five helper functions are based on BadMagic100's Rando4Stats RandoExtensions
        // MIT License

        // Copyright(c) 2022 BadMagic100

        // Permission is hereby granted, free of charge, to any person obtaining a copy
        // of this software and associated documentation files(the "Software"), to deal
        // in the Software without restriction, including without limitation the rights
        // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        // copies of the Software, and to permit persons to whom the Software is
        // furnished to do so, subject to the following conditions:

        // The above copyright notice and this permission notice shall be included in all
        // copies or substantial portions of the Software.
        public static ItemPlacement RandoPlacement(this AbstractItem item)
        {
            if (item.GetTag(out RandoItemTag tag))
            {
                return RandomizerMod.RandomizerMod.RS.Context.itemPlacements[tag.id];
            }
            return default;
        }

        public static string RandoItemName(this AbstractItem item)
        {
            return item.RandoPlacement().item.Name ?? "";
        }

        public static string RandoLocationName(this AbstractItem item)
        {
            return item.RandoPlacement().location.Name ?? "";
        }

        public static int RandoItemId(this AbstractItem item)
        {
            if (item.GetTag(out RandoItemTag tag))
            {
                return tag.id;
            }
            return default;
        }

        public static bool CanPreview(this AbstractPlacement placement)
        {
            return !placement.HasTag<ItemChanger.Tags.DisableItemPreviewTag>();
        }

        public static bool IsPersistent(this AbstractItem item)
        {
            return item.HasTag<ItemChanger.Tags.PersistentItemTag>();
        }

        public static void SetUsedPinDefs()
        {
            _usedPins.Clear();

            foreach (KeyValuePair<string, AbstractPlacement> placement in ItemChanger.Internal.Ref.Settings.Placements)
            {
                IEnumerable<ItemDef> items = placement.Value.Items
                    .Where(x => !x.IsObtained())
                    .Select(x => new ItemDef(x));

                if (!items.Any()) continue;

                string locationName = placement.Value.Items.Where(x => !x.IsObtained()).First().RandoLocationName();

                if (locationName == "Start") continue;

                if (MapModS.RandomizableLeversInstalled
                    && locationName == "Dirtmouth_Stag" || locationName == "Resting_Grounds_Stag")
                {
                    continue;
                }

                if (_allPins.TryGetValue(locationName, out PinDef pinDef))
                {
                    pinDef.randoItems = items;
                    pinDef.canPreview = placement.Value.CanPreview();
                    pinDef.pinLocationState = PinLocationState.UncheckedUnreachable;
                    pinDef.locationPoolGroup = GetLocationPoolGroup(pinDef.name);
                        
                    _usedPins.Add(locationName, pinDef);
                }
                else
                {
                    MapModS.Instance.Log("No corresponding pin location for a placement");
                }
            }

            foreach (KeyValuePair<string, PinDef> pinDef in _allPins)
            {
                if (!_usedPins.ContainsKey(pinDef.Key) && !pinDef.Value.randoOnly)
                {
                    pinDef.Value.pinLocationState = PinLocationState.NonRandomizedUnchecked;
                    pinDef.Value.locationPoolGroup = GetLocationPoolGroup(pinDef.Value.name);
                    _usedPins.Add(pinDef.Key, pinDef.Value);
                }
            }

            if (MapModS.AdditionalMapsInstalled)
            {
                foreach (PinDef pinDefAM in GetPinAMArray())
                {
                    if (_usedPins.TryGetValue(pinDefAM.name, out PinDef pinDef))
                    {
                        pinDef.pinScene = pinDefAM.pinScene;
                        pinDef.mapZone = pinDefAM.mapZone;
                        pinDef.offsetX = pinDefAM.offsetX;
                        pinDef.offsetY = pinDefAM.offsetY;
                    }
                }
            }
        }

        public static void Load()
        {
            _allPins = JsonUtil.Deserialize<Dictionary<string, PinDef>>("MapModS.Resources.pins.json");
            _allPinsAM = JsonUtil.Deserialize<Dictionary<string, PinDef>>("MapModS.Resources.pinsAM.json");
        }

        // For debugging pins
        //public static void LoadNewPinDef()
        //{
        //    newPins.Clear();
        //    newPins = JsonUtil.DeserializeFromExternalFile<Dictionary<string, PinDef>>("newPins.json");
        //}
    }
}