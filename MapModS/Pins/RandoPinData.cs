using ConnectionMetadataInjector.Util;
using ItemChanger;
using MapModS.Data;
using RandomizerCore;
using RandomizerMod.IC;
using RandomizerMod.RC;
using System.Collections.Generic;
using System.Linq;
using RD = RandomizerMod.RandomizerData;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Pins
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

        private static readonly string[] sortedDefaultPoolGroups =
        {
            PoolGroupNames.DREAMERS,
            PoolGroupNames.SKILLS,
            PoolGroupNames.CHARMS,
            PoolGroupNames.KEYS,
            PoolGroupNames.MASKSHARDS,
            PoolGroupNames.VESSELFRAGMENTS,
            PoolGroupNames.CHARMNOTCHES,
            PoolGroupNames.PALEORE,
            PoolGroupNames.GEOCHESTS,
            PoolGroupNames.RANCIDEGGS,
            PoolGroupNames.RELICS,
            PoolGroupNames.WHISPERINGROOTS,
            PoolGroupNames.BOSSESSENCE,
            PoolGroupNames.GRUBS,
            PoolGroupNames.MIMICS,
            PoolGroupNames.MAPS,
            PoolGroupNames.STAGS,
            PoolGroupNames.LIFEBLOODCOCOONS,
            PoolGroupNames.GRIMMKINFLAMES,
            PoolGroupNames.JOURNALENTRIES,
            PoolGroupNames.GEOROCKS,
            PoolGroupNames.BOSSGEO,
            PoolGroupNames.SOULTOTEMS,
            PoolGroupNames.LORETABLETS,
            PoolGroupNames.SHOPS,
        };

        public static Dictionary<string, MapPositionDef> LocationLookup;

        public static Dictionary<string, VanillaPinDef> globalPinDefs;

        internal static Dictionary<string, RandomizerModPinDef> PinDefs;

        internal static List<string> PoolGroups { get; private set; }
        internal static HashSet<string> RandoPoolGroups { get; private set; }
        internal static HashSet<string> VanillaPoolGroups { get; private set; }

        internal static void LoadGlobalPinDefs()
        {
            LocationLookup = JsonUtil.Deserialize<Dictionary<string, MapPositionDef>>("MapModS.Resources.pins.json");

            globalPinDefs = JsonUtil.Deserialize<Dictionary<string, VanillaPinDef>>("MapModS.Resources.pins.json");

            if (!Dependencies.HasAdditionalMaps()) return;

            Dictionary<string, MapPositionDef> pinDefsAM = JsonUtil.Deserialize<Dictionary<string, MapPositionDef>>("MapModS.Resources.pinsAM.json");
            foreach((string name, MapPositionDef mpd) in pinDefsAM.Select(kvp => (kvp.Key, kvp.Value)))
            {
                globalPinDefs[name].SetPosition(mpd);
            }
        }

        internal static void SetPinDefs()
        {
            PinDefs = new();
            foreach (AbstractPlacement placement in ItemChanger.Internal.Ref.Settings.Placements.Values.Where(placement => placement.HasTag<RandoPlacementTag>()))
            {
                AddRandoPinDef(placement);
            }
            foreach (GeneralizedPlacement placement in RM.RS.Context.Vanilla.Where(placement => RD.Data.IsLocation(placement.Location.Name) && !PinDefs.ContainsKey(placement.Location.Name)))
            {
                AddVanillaPinDef(placement);
            }

            SetPoolGroups();
        }

        private static void AddRandoPinDef(AbstractPlacement placement)
        {
            placement.LogDebug();

            if (placement.Name == "Start")
            {
                MapModS.Instance.LogDebug($"Start placement detected - not including as a pin");
                return;
            }

            RandoModLocation rml = placement.RandoLocation();
            RandoPinDef rpd;
            if (globalPinDefs.ContainsKey(rml.Name))
            {
                rpd = new(placement, rml, globalPinDefs[rml.Name]);
            }
            else
            {
                MapModS.Instance.Log("Unknown placement. Making a 'best guess' for the placement");
                rpd = new(placement, rml);
            }

            PinDefs.Add(rml.Name, rpd);
        }

        private static void AddVanillaPinDef(GeneralizedPlacement placement)
        {
            string name = placement.Location.Name;
            // TODO: support vanilla multi-item placements
            if (PinDefs.ContainsKey(name)) return;

            placement.LogDebug();

            if (name == "Start")
            {
                MapModS.Instance.LogDebug($"Start vanilla placement detected - not including as a pin");
                return;
            }

            PinDefs.Add(name, globalPinDefs[name]);
        }

        private static void SetPoolGroups()
        {
            PoolGroups = new();
            RandoPoolGroups = new();
            VanillaPoolGroups = new();

            HashSet<string> unsortedPoolGroups = new();
            foreach (RandomizerModPinDef pinDef in PinDefs.Values)
            {
                unsortedPoolGroups.Add(pinDef.LocationPoolGroup);
                unsortedPoolGroups.UnionWith(pinDef.ItemPoolGroups);

                if (pinDef.GetType() == typeof(RandoPinDef))
                {
                    RandoPoolGroups.Add(pinDef.LocationPoolGroup);
                    RandoPoolGroups.UnionWith(pinDef.ItemPoolGroups);
                }
                if (pinDef.GetType() == typeof(VanillaPinDef))
                {
                    VanillaPoolGroups.Add(pinDef.LocationPoolGroup);
                    VanillaPoolGroups.UnionWith(pinDef.ItemPoolGroups);
                }
            }
            foreach (string poolGroup in sortedDefaultPoolGroups.Where(poolGroup => unsortedPoolGroups.Contains(poolGroup)))
            {
                PoolGroups.Add(poolGroup);
            }
            foreach (string poolGroup in unsortedPoolGroups.Where(poolGroup => !PoolGroups.Contains(poolGroup)))
            {
                PoolGroups.Add(poolGroup);
            }

            foreach (string poolGroup in RandoPoolGroups)
            {
                MapModS.Instance.LogDebug($"Randomized Pool Group: {poolGroup}");
            }
            foreach (string poolGroup in VanillaPoolGroups)
            {
                MapModS.Instance.LogDebug($"Vanilla Pool Group: {poolGroup}");
            }
        }
    }
}
