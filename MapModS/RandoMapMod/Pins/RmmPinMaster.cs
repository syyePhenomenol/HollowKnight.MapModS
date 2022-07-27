using System;
using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using MapChanger;
using MapChanger.MonoBehaviours;
using RandoMapMod.Settings;
using RandomizerCore;
using RandomizerMod.IC;
using UnityEngine;
using RD = RandomizerMod.RandomizerData;
using RM = RandomizerMod.RandomizerMod;

namespace RandoMapMod.Pins
{
    internal static class RmmPinMaster
    {
        private const float OFFSETZ_BASE = -1.4f;
        private const float OFFSETZ_RANGE = 0.4f;

        internal static MapObject MoPins;
        internal static Dictionary<string, RmmPin> Pins { get; private set; } = new();

        internal static List<string> PoolGroups { get; private set; }
        internal static HashSet<string> RandoPoolGroups { get; private set; }
        internal static HashSet<string> VanillaPoolGroups { get; private set; }

        internal static void MakePins(GameObject goMap)
        {
            Pins = new();

            MoPins = Utils.MakeMonoBehaviour<MapObject>(goMap, "RandoMapMod Pins");
            MoPins.Initialize();

            MapObjectUpdater.Add(MoPins);

            foreach (AbstractPlacement placement in ItemChanger.Internal.Ref.Settings.Placements.Values.Where(placement => placement.HasTag<RandoPlacementTag>()))
            {
                MakeRandoPin(placement);
            }
            foreach (GeneralizedPlacement placement in RM.RS.Context.Vanilla.Where(placement => RD.Data.IsLocation(placement.Location.Name) && !Pins.ContainsKey(placement.Location.Name)))
            {
                MakeVanillaPin(placement);
            }

            StaggerPins();
            SetPoolGroups();

            //VmmPinSelector pinSelector = Utils.MakeMonoBehaviour<VmmPinSelector>(null, "Pin Selector");
            //pinSelector.Initialize(Pins);
        }

        private static void MakeRandoPin(AbstractPlacement placement)
        {
            placement.LogDebug();

            if (placement.Name == "Start")
            {
                RandoMapMod.Instance.LogDebug($"Start placement detected - not including as a pin");
                return;
            }

            RandomizedRmmPin randoPin = Utils.MakeMonoBehaviour<RandomizedRmmPin>(MoPins.gameObject, placement.Name);
            randoPin.Initialize(placement);
            randoPin.Parent = MoPins;
            Pins[placement.Name] = randoPin;
        }

        private static void MakeVanillaPin(GeneralizedPlacement placement)
        {
            placement.LogDebug();

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
            vanillaPin.Parent = MoPins;
            Pins[placement.Location.Name] = vanillaPin;
        }

        internal static void UpdateActive()
        {
            foreach (RmmPin pin in Pins.Values)
            {
                pin.UpdateActive();
            }
        }

        //internal static string GetPoolGroupCounter(PoolGroup poolGroup)
        //{
        //    string text;

        //    IReadOnlyCollection<MapObject> pins = PinGroups[poolGroup].GetChildren();

        //    if (IsPersistent(poolGroup))
        //    {
        //        text = "";
        //    }
        //    else
        //    {
        //        text = pins.Where(pin => Tracker.HasClearedLocation(pin.name)).Count().ToString() + " / ";
        //    }

        //    return text + pins.Count().ToString();
        //}

        //private static bool IsPersistent(PoolGroup poolGroup)
        //{
        //    return poolGroup is PoolGroup.LifebloodCocoons or PoolGroup.SoulTotems or PoolGroup.LoreTablets;
        //}

        private static void StaggerPins()
        {
            IEnumerable<MapObject> PinsSorted = Pins.Values.OrderBy(mapObj => mapObj.transform.position.x).ThenBy(mapObj => mapObj.transform.position.y);

            for (int i = 0; i < PinsSorted.Count(); i++)
            {
                Transform transform = PinsSorted.ElementAt(i).transform;
                transform.localPosition = new(transform.localPosition.x, transform.localPosition.y, OFFSETZ_BASE + (float)i / Pins.Count() * OFFSETZ_RANGE);
            }
        }

        private static void SetPoolGroups()
        {
            PoolGroups = new();
            RandoPoolGroups = new();
            VanillaPoolGroups = new();

            foreach (RmmPin pin in Pins.Values)
            {
                if (pin is RandomizedRmmPin randoPin)
                {
                    RandoPoolGroups.Add(randoPin.LocationPoolGroup);
                    RandoPoolGroups.UnionWith(randoPin.ItemPoolGroups);

                }
                if (pin is VanillaRmmPin vanillaPin)
                {
                    VanillaPoolGroups.Add(vanillaPin.LocationPoolGroup);
                    VanillaPoolGroups.UnionWith(vanillaPin.ItemPoolGroups);
                }
            }
            foreach (string poolGroup in Enum.GetValues(typeof(PoolGroup)).Cast<PoolGroup>().Select(poolGroup => poolGroup.FriendlyName()).Where(poolGroup => RandoPoolGroups.Contains(poolGroup) || VanillaPoolGroups.Contains(poolGroup)))
            {
                PoolGroups.Add(poolGroup);
            }
            foreach (string poolGroup in RandoPoolGroups.Union(VanillaPoolGroups).Where(poolGroup => !PoolGroups.Contains(poolGroup)))
            {
                PoolGroups.Add(poolGroup);
            }

            foreach (string poolGroup in RandoPoolGroups)
            {
                RandoMapMod.Instance.LogDebug($"Randomized Pool Group: {poolGroup}");
            }
            foreach (string poolGroup in VanillaPoolGroups)
            {
                RandoMapMod.Instance.LogDebug($"Vanilla Pool Group: {poolGroup}");
            }
        }
    }
}
