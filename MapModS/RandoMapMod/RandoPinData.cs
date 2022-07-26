using System;
using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using MapChanger;
using MapChanger.Defs;
//using RandoMapMod.Data;
using RandoMapMod.Defs;
using RandomizerCore;
using RandomizerMod.IC;
using RandomizerMod.RC;
using RD = RandomizerMod.RandomizerData;
using RM = RandomizerMod.RandomizerMod;

namespace RandoMapMod
{
    /// <summary>
    /// Handles all data of item placements, and subsequently pin placements.
    /// </summary>
    internal static class RandoPinData
    {
        //private static readonly HashSet<string> shopNames = new()
        //{
        //    "Sly",
        //    "Sly_(Key)",
        //    "Iselda",
        //    "Salubra",
        //    "Salubra_(Requires_Charms)",
        //    "Leg_Eater",
        //    "Grubfather",
        //    "Seer",
        //    "Egg_Shop"
        //};

        //internal static Dictionary<string, RMMPinDef> PinDefs;

        internal static List<string> PoolGroups { get; private set; }
        internal static HashSet<string> RandoPoolGroups { get; private set; }
        internal static HashSet<string> VanillaPoolGroups { get; private set; }

        internal static void InjectRandoLocations()
        {
            MapChanger.Finder.InjectLocations(JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.RandoMapMod.Resources.randoPins.json"));

            //if (!Interop.HasAdditionalMaps()) return;
            //MapChanger.Finder.InjectLocations(JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.RandoMapMod.Resources.AdditionalMaps.randoPins.json"));
        }

        //internal static void SetPinDefs()
        //{
        //    PinDefs = new();
        //    foreach (AbstractPlacement placement in ItemChanger.Internal.Ref.Settings.Placements.Values.Where(placement => placement.HasTag<RandoPlacementTag>()))
        //    {
        //        AddRandoPinDef(placement);
        //    }
        //    foreach (GeneralizedPlacement placement in RM.RS.Context.Vanilla.Where(placement => RD.Data.IsLocation(placement.Location.Name) && !PinDefs.ContainsKey(placement.Location.Name)))
        //    {
        //        AddVanillaPinDef(placement);
        //    }

        //    SetPoolGroups();
        //}

        //private static void AddRandoPinDef(AbstractPlacement placement)
        //{
        //    placement.LogDebug();

        //    if (placement.Name == "Start")
        //    {
        //        RandoMapMod.Instance.LogDebug($"Start placement detected - not including as a pin");
        //        return;
        //    }

        //    RandoModLocation rml = placement.RandoModLocation();

        //    try
        //    {
        //        PinDefs.Add(rml.Name, new RandomizedPinDef(placement));
        //    }
        //    catch (Exception e)
        //    {
        //        RandoMapMod.Instance.LogError(e);
        //    }
        //}

        //private static void AddVanillaPinDef(GeneralizedPlacement placement)
        //{
        //    string name = placement.Location.Name;
        //    if (PinDefs.ContainsKey(name)) return;

        //    placement.LogDebug();

        //    if (name == "Start")
        //    {
        //        RandoMapMod.Instance.LogDebug($"Start vanilla placement detected - not including as a pin");
        //        return;
        //    }

        //    try
        //    {
        //        PinDefs.Add(name, new VanillaPinDef(name));
        //    }
        //    catch (Exception e)
        //    {
        //        RandoMapMod.Instance.LogError(e);
        //    }
        //}

        //private static void SetPoolGroups()
        //{
        //    PoolGroups = new();
        //    RandoPoolGroups = new();
        //    VanillaPoolGroups = new();

        //    foreach (RMMPinDef pinDef in PinDefs.Values)
        //    {
        //        if (pinDef is RandomizedPinDef rpd)
        //        {
        //            RandoPoolGroups.Add(rpd.LocationPoolGroup);
        //            RandoPoolGroups.UnionWith(rpd.ItemPoolGroups);

        //        }
        //        if (pinDef is VanillaPinDef vpd)
        //        {
        //            VanillaPoolGroups.Add(vpd.LocationPoolGroup);
        //            VanillaPoolGroups.UnionWith(vpd.ItemPoolGroups);
        //        }
        //    }
        //    foreach (string poolGroup in Enum.GetValues(typeof(PoolGroup)).Cast<PoolGroup>().Select(poolGroup => poolGroup.FriendlyName()).Where(poolGroup => RandoPoolGroups.Contains(poolGroup) || VanillaPoolGroups.Contains(poolGroup)))
        //    {
        //        PoolGroups.Add(poolGroup);
        //    }
        //    foreach (string poolGroup in RandoPoolGroups.Union(VanillaPoolGroups).Where(poolGroup => !PoolGroups.Contains(poolGroup)))
        //    {
        //        PoolGroups.Add(poolGroup);
        //    }

        //    foreach (string poolGroup in RandoPoolGroups)
        //    {
        //        RandoMapMod.Instance.LogDebug($"Randomized Pool Group: {poolGroup}");
        //    }
        //    foreach (string poolGroup in VanillaPoolGroups)
        //    {
        //        RandoMapMod.Instance.LogDebug($"Vanilla Pool Group: {poolGroup}");
        //    }
        //}
    }
}
