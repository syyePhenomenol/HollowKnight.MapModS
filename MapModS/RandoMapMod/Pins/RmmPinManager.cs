using System;
using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using RandomizerCore;
using RandomizerMod.IC;
using UnityEngine;
using RD = RandomizerMod.RandomizerData;
using RM = RandomizerMod.RandomizerMod;

namespace RandoMapMod.Pins
{
    internal class RmmPinManager : HookModule
    {
        private const float OFFSETZ_BASE = -1.4f;
        private const float OFFSETZ_RANGE = 0.4f;

        internal static MapObject MoPins { get; private set; }
        internal static Dictionary<string, RmmPin> Pins { get; private set; } = new();

        internal static List<string> AllPoolGroups { get; private set; }
        internal static HashSet<string> RandoLocationPoolGroups { get; private set; }
        internal static HashSet<string> RandoItemPoolGroups { get; private set; }
        internal static HashSet<string> VanillaLocationPoolGroups { get; private set; }
        internal static HashSet<string> VanillaItemPoolGroups { get; private set; }

        public override void OnEnterGame()
        {
            TrackerUpdate.OnFinishedUpdate += UpdateRandoPins;
        }

        public override void OnQuitToMenu()
        {
            TrackerUpdate.OnFinishedUpdate -= UpdateRandoPins;
        }

        internal static void Make(GameObject goMap)
        {
            RmmPin.MiscPinsCount = 0;

            Pins = new();

            MoPins = Utils.MakeMonoBehaviour<MapObject>(goMap, "RandoMapMod Pins");
            MoPins.Initialize();

            MapObjectUpdater.Add(MoPins);

            foreach (AbstractPlacement placement in ItemChanger.Internal.Ref.Settings.Placements.Values.Where(placement => placement.HasTag<RandoPlacementTag>()))
            {
                if (SupplementalMetadata.OfPlacementAndLocations(placement).Get(InteropProperties.DoNotMakePin)) continue;
                MakeRandoPin(placement);
            }
            foreach (GeneralizedPlacement placement in RM.RS.Context.Vanilla.Where(placement => RD.Data.IsLocation(placement.Location.Name) && !Pins.ContainsKey(placement.Location.Name)))
            {
                MakeVanillaPin(placement);
            }
            if (Interop.HasBenchwarp())
            {
                foreach (string benchName in BenchwarpInterop.GetAllBenchNames())
                {
                    MakeBenchPin(benchName);
                }
            }

            StaggerPins();
            InitializePoolGroups();

            UpdateRandoPins();

            RmmPinSelector pinSelector = Utils.MakeMonoBehaviour<RmmPinSelector>(null, "RandoMapMod Pin Selector");
            pinSelector.Initialize(Pins.Values);
        }

        private static void MakeRandoPin(AbstractPlacement placement)
        {
            //placement.LogDebug();

            if (placement.Name is "Start" or "Remote")
            {
                RandoMapMod.Instance.LogDebug($"{placement.Name} detected - not including as a pin");
                return;
            }

            RandomizedRmmPin randoPin = Utils.MakeMonoBehaviour<RandomizedRmmPin>(MoPins.gameObject, placement.Name);
            randoPin.Initialize(placement);
            MoPins.AddChild(randoPin);
            Pins[placement.Name] = randoPin;
        }

        private static void MakeVanillaPin(GeneralizedPlacement placement)
        {
            //placement.LogDebug();

            string name = placement.Location.Name;
            if (Pins.ContainsKey(name))
            {
                RandoMapMod.Instance.LogDebug($"Vanilla placement with the same name as existing key in Pins detected: {name}");
                return;
            }

            if (name == "Start")
            {
                RandoMapMod.Instance.LogDebug($"Start vanilla placement detected - not including as a pin");
                return;
            }

            VanillaRmmPin vanillaPin = Utils.MakeMonoBehaviour<VanillaRmmPin>(MoPins.gameObject, placement.Location.Name);
            vanillaPin.Initialize(placement);
            MoPins.AddChild(vanillaPin);
            Pins[placement.Location.Name] = vanillaPin;
        }

        private static void MakeBenchPin(string benchName)
        {
            string objectName = $"{benchName}{BenchwarpInterop.BENCH_EXTRA_SUFFIX}";
            BenchPin benchPin = Utils.MakeMonoBehaviour<BenchPin>(MoPins.gameObject, objectName);
            benchPin.Initialize(benchName);
            MoPins.AddChild(benchPin);
            Pins[objectName] = benchPin;
        }

        internal static void Update()
        {
            foreach (RmmPin pin in Pins.Values)
            {
                pin.MainUpdate();
            }
        }

        internal static void UpdateRandoPins()
        {
            foreach (RmmPin pin in Pins.Values)
            {
                if (pin is RandomizedRmmPin randoPin)
                {
                    randoPin.UpdatePlacementState();
                }
            }
        }

        /// <summary>
        /// Makes sure all the Z offsets for the pins aren't the same.
        /// </summary>
        private static void StaggerPins()
        {
            IEnumerable<MapObject> PinsSorted = Pins.Values.OrderBy(mapObj => mapObj.transform.position.x).ThenBy(mapObj => mapObj.transform.position.y);

            for (int i = 0; i < PinsSorted.Count(); i++)
            {
                Transform transform = PinsSorted.ElementAt(i).transform;
                transform.localPosition = new(transform.localPosition.x, transform.localPosition.y, OFFSETZ_BASE + (float)i / Pins.Count() * OFFSETZ_RANGE);
            }
        }

        /// <summary>
        /// Sets all the sorted, randomized/vanilla location/item PoolGroups.
        /// </summary>
        private static void InitializePoolGroups()
        {
            AllPoolGroups = new();
            RandoLocationPoolGroups = new();
            RandoItemPoolGroups = new();
            VanillaLocationPoolGroups = new();
            VanillaItemPoolGroups = new();

            foreach (RmmPin pin in Pins.Values)
            {
                if (pin is RandomizedRmmPin)
                {
                    RandoLocationPoolGroups.Add(pin.LocationPoolGroup);
                    RandoItemPoolGroups.UnionWith(pin.ItemPoolGroups);
                }
                if (pin is VanillaRmmPin)
                {
                    VanillaLocationPoolGroups.Add(pin.LocationPoolGroup);
                    VanillaItemPoolGroups.UnionWith(pin.ItemPoolGroups);
                }
            }

            foreach (string poolGroup in Enum.GetValues(typeof(PoolGroup))
                .Cast<PoolGroup>()
                .Select(poolGroup => poolGroup.FriendlyName())
                .Where(poolGroup => RandoLocationPoolGroups.Contains(poolGroup)
                    || RandoItemPoolGroups.Contains(poolGroup)
                    || VanillaLocationPoolGroups.Contains(poolGroup)
                    || VanillaItemPoolGroups.Contains(poolGroup)))
            {
                AllPoolGroups.Add(poolGroup);
            }
            foreach (string poolGroup in RandoLocationPoolGroups
                .Union(RandoItemPoolGroups)
                .Union(VanillaLocationPoolGroups)
                .Union(VanillaItemPoolGroups)
                .Where(poolGroup => !AllPoolGroups.Contains(poolGroup)))
            {
                AllPoolGroups.Add(poolGroup);
            }

            //foreach (string poolGroup in ActiveRandoPoolGroups)
            //{
            //    RandoMapMod.Instance.LogDebug($"Randomized Pool Group: {poolGroup}");
            //}
            //foreach (string poolGroup in ActiveVanillaPoolGroups)
            //{
            //    RandoMapMod.Instance.LogDebug($"Vanilla Pool Group: {poolGroup}");
            //}
        }

        //public static void ImportDefs()
        //{
        //    Dictionary<string, MapLocationDef> newDefs = JsonUtil.DeserializeFromExternalFile<Dictionary<string, MapLocationDef>>("locations.json");

        //    foreach (MapLocationDef def in newDefs.Values)
        //    {
        //        if (Pins.TryGetValue(def.Name, out RmmPin pin))
        //        {
        //            pin.UpdatePosition(def.MapLocations);
        //        }
        //    }
        //}
    }
}
