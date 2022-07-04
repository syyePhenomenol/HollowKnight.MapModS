using ConnectionMetadataInjector;
using ConnectionMetadataInjector.Util;
using GlobalEnums;
using ItemChanger;
using RandomizerCore;
using RandomizerMod.IC;
using RandomizerMod.RC;
using System.Collections.Generic;
using System.Linq;
using CMI = ConnectionMetadataInjector.ConnectionMetadataInjector;
using RD = RandomizerMod.RandomizerData.Data;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Data
{
    public static class MainData
    {
        //private static Dictionary<string, OldPin> allPins;
        //private static Dictionary<string, OldPin> allPinsAM;
        private static List<string> sortedGroups;
        private static HashSet<string> minimalMapRooms;
        internal static Dictionary<string, MapRoomDef> nonMappedRooms;

        //private static readonly Dictionary<string, OldPin> usedPins = new();
        //private static Dictionary<string, string> logicLookup = new();

        public static List<string> usedPoolGroups = new();

        //public static OldPin[] GetUsedPinArray()
        //{
        //    return usedPins.Values.OrderBy(p => p.offsetX).ThenBy(p => p.offsetY).ToArray();
        //}

        //public static OldPin GetUsedPinDef(string locationName)
        //{
        //    if (usedPins.TryGetValue(locationName, out OldPin pinDef))
        //    {
        //        return pinDef;
        //    }

        //    return default;
        //}

        public static bool IsMinimalMapRoom(string scene)
        {
            return minimalMapRooms.Contains(scene);
        }

        public static bool IsNonMappedScene(string scene)
        {
            return nonMappedRooms.ContainsKey(scene);
        }

        public static IEnumerable<string> GetNonMappedScenes()
        {
            if (Dependencies.HasAdditionalMaps())
            {
                return nonMappedRooms.Keys.Where(s => nonMappedRooms[s].includeWithAdditionalMaps);
            }

            return nonMappedRooms.Keys;
        }

        public static MapRoomDef GetNonMappedRoomDef(string scene)
        {
            if (nonMappedRooms.TryGetValue(scene, out MapRoomDef mrd))
            {
                return mrd;
            }

            return default;
        }

        public static MapZone GetFixedMapZone()
        {
            if (nonMappedRooms.TryGetValue(Utils.CurrentScene(), out MapRoomDef mrd))
            {
                return mrd.mapZone;
            }

            // This seems to be the one mapped room that doesn't have its MapZone set
            if (Utils.CurrentScene() == "Ruins_Elevator")
            {
                return MapZone.CITY;
            }

            return default;
        }

        //public static bool IsInLogicLookup(string locationName)
        //{
        //    return logicLookup.ContainsKey(locationName);
        //}

        //public static string GetRawLogic(string locationName)
        //{
        //    if (logicLookup.TryGetValue(locationName, out string logic))
        //    {
        //        return logic;
        //    }

        //    return default;
        //}

        //public static void SetUsedPinDefs()
        //{
        //    usedPins.Clear();
        //    usedPoolGroups.Clear();
        //    HashSet<string> unsortedGroups = new();

        //    // Randomized placements
        //    foreach (KeyValuePair<string, AbstractPlacement> placement in ItemChanger.Internal.Ref.Settings.Placements)
        //    {
        //        if (placement.Value.Items.Any(i => !i.HasTag<RandoItemTag>())) continue;

        //        IEnumerable<ItemDef> items = placement.Value.Items
        //            .Where(x => !x.IsObtained() || x.IsPersistent())
        //            .Select(x => new ItemDef(x));

        //        if (!items.Any()) continue;

        //        RandoModLocation rml = placement.Value.RandoLocation();

        //        if (rml == null || rml.Name == "Start" || rml.LocationDef.SceneName == null) continue;

        //        if (!allPins.TryGetValue(rml.Name, out OldPin pd))
        //        {
        //            pd = new();

        //            MapModS.Instance.Log("Unknown placement. Making a 'best guess' for the placement");
        //        }

        //        pd.name = rml.Name;
        //        pd.sceneName = rml.LocationDef.SceneName;
                
        //        if (pd.sceneName == "Room_Colosseum_Bronze" || pd.sceneName == "Room_Colosseum_Silver")
        //        {
        //            pd.sceneName = "Room_Colosseum_01";
        //        }

        //        if (nonMappedRooms.ContainsKey(pd.sceneName))
        //        {
        //            pd.pinScene = nonMappedRooms[pd.sceneName].mappedScene;
        //            pd.mapZone = nonMappedRooms[pd.sceneName].mapZone;
        //        }

        //        if (pd.pinScene == null)
        //        {
        //            pd.mapZone = Utils.ToMapZone(RD.GetRoomDef(pd.sceneName).MapArea);
        //        }

        //        pd.randomized = true;
        //        pd.randoItems = items;
        //        pd.canPreviewItem = placement.Value.CanPreview();

        //        // UpdatePins will set it to the correct state
        //        pd.pinLocationState = PinState.UncheckedUnreachable;
        //        pd.locationPoolGroup = SupplementalMetadata.OfPlacementAndLocations(placement.Value).Get(CMI.LocationPoolGroup);

        //        usedPins.Add(rml.Name, pd);

        //        unsortedGroups.Add(pd.locationPoolGroup);

        //        foreach(ItemDef i in pd.randoItems)
        //        {
        //            unsortedGroups.Add(i.poolGroup);
        //        }

        //        //MapModS.Instance.Log(locationName);
        //        //MapModS.Instance.Log(pinDef.locationPoolGroup);
        //    }
            
        //    // Vanilla placements
        //    foreach (GeneralizedPlacement placement in RM.RS.Context.Vanilla)
        //    {
        //        if (RD.IsLocation(placement.Location.Name)
        //            && !RM.RS.TrackerData.clearedLocations.Contains(placement.Location.Name)
        //            && placement.Location.Name != "Start"
        //            && placement.Location.Name != "Iselda"
        //            && allPins.ContainsKey(placement.Location.Name)
        //            && !usedPins.ContainsKey(placement.Location.Name))
        //        {
        //            OldPin pd = allPins[placement.Location.Name];

        //            pd.name = placement.Location.Name;
        //            pd.sceneName = RD.GetLocationDef(placement.Location.Name).SceneName;

        //            if (pd.sceneName == "Room_Colosseum_Bronze" || pd.sceneName == "Room_Colosseum_Silver")
        //            {
        //                pd.sceneName = "Room_Colosseum_01";
        //            }

        //            if (nonMappedRooms.ContainsKey(pd.sceneName))
        //            {
        //                pd.pinScene = nonMappedRooms[pd.sceneName].mappedScene;
        //                pd.mapZone = nonMappedRooms[pd.sceneName].mapZone;
        //            }

        //            if (pd.pinScene == null)
        //            {
        //                pd.mapZone = Utils.ToMapZone(RD.GetRoomDef(pd.sceneName).MapArea);
        //            }

        //            //if (!HasObtainedVanillaItem(pd))
        //            //{
        //            //    pd.randomized = false;

        //            //    pd.pinLocationState = PinLocationState.NonRandomizedUnchecked;
        //            //    pd.locationPoolGroup = SubcategoryFinder.GetLocationPoolGroup(placement.Location.Name).FriendlyName();

        //            //    usedPins.Add(placement.Location.Name, pd);

        //            //    unsortedGroups.Add(pd.locationPoolGroup);

        //            //    //MapModS.Instance.Log(placement.Location.Name);
        //            //}
        //        }
        //    }

        //    // Sort all the PoolGroups that have been used
        //    foreach (string poolGroup in sortedGroups)
        //    {
        //        if (unsortedGroups.Contains(poolGroup))
        //        {
        //            usedPoolGroups.Add(poolGroup);
        //            unsortedGroups.Remove(poolGroup);
        //        }
        //    }

        //    usedPoolGroups.AddRange(unsortedGroups);

        //    if (Dependencies.HasAdditionalMaps())
        //    {
        //        ApplyAdditionalMapsChanges();
        //    }
        //}

        //public static void ApplyAdditionalMapsChanges()
        //{
        //    foreach (KeyValuePair<string, OldPin> kvp in allPinsAM)
        //    {
        //        if (usedPins.TryGetValue(kvp.Key, out OldPin pinDef))
        //        {
        //            pinDef.pinScene = kvp.Value.pinScene;
        //            pinDef.mapZone = kvp.Value.mapZone;
        //            pinDef.offsetX = kvp.Value.offsetX;
        //            pinDef.offsetY = kvp.Value.offsetY;
        //        }
        //    }
        //}

        //public static void SetLogicLookup()
        //{
        //    logicLookup = RM.RS.TrackerData.lm.LogicLookup.Values.ToDictionary(l => l.Name, l => l.ToInfix());
        //}

        public static void Load()
        {
            //allPins = JsonUtil.Deserialize<Dictionary<string, OldPin>>("MapModS.Resources.pins.json");
            //allPinsAM = JsonUtil.Deserialize<Dictionary<string, OldPin>>("MapModS.Resources.pinsAM.json");
            sortedGroups = JsonUtil.Deserialize<List<string>>("MapModS.Resources.sortedGroups.json");
            minimalMapRooms = JsonUtil.Deserialize<HashSet<string>>("MapModS.Resources.minimalMapRooms.json");
            nonMappedRooms = JsonUtil.Deserialize<Dictionary<string, MapRoomDef>>("MapModS.Resources.nonMappedRooms.json");
        }

#if DEBUG
        //public static Dictionary<string, OldPin> newPins;
        public static Dictionary<string, MapRoomDef> newRooms;

        public static void LoadDebugResources()
        {
            //newPins = JsonUtil.DeserializeFromExternalFile<Dictionary<string, PinDef>> ("newPins.json");
            newRooms = JsonUtil.DeserializeFromExternalFile<Dictionary<string, MapRoomDef>>("newRooms.json");
        }
#endif
    }
}