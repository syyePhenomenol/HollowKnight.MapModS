using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector.Util;
using MapChanger;
using MapChanger.MonoBehaviours;
using RandoMapMod.Defs;
using UnityEngine;

namespace RandoMapMod.Pins
{
    internal class RMMPinGroup : MapObjectGroup
    {
        internal static RMMPinGroup Instance { get; private set; }

        //internal Dictionary<string, List<RMMPin>> GroupedPins { get; private set; } = new();
        internal Selector PinSelector { get; private set; }

        internal static void Make(GameObject goMap)
        {
            Instance = Utils.MakeMonoBehaviour<RMMPinGroup>(goMap, nameof(RMMPinGroup));

            Instance.PinSelector = Utils.MakeMonoBehaviour<Selector>(null, nameof(PinSelector));

            Instance.MakePins();

            Instance.Set();
        }

        //internal static string GetPoolGroupCounter(PoolGroup poolGroup)
        //{
        //    return text = Instance.GroupedPins[poolGroup].Where(pin => pin.Obtained).Count().ToString() + " / ";

        //     text + Instance.GroupedPins[poolGroup].Count().ToString();
        //}

        private void MakePins()
        {
            foreach (RMMPinDef mld in RandoPinData.PinDefs.Values)
            {
                RMMPin pin = Utils.MakeMonoBehaviour<RMMPin>(gameObject, $"{nameof(RMMPin)} {mld.Name}");
                pin.Initialize(mld);

                //PinSelector.Objects.Add(pin.Location.Name, new() { pin });
                Children.Add(pin);
            }

            Instance.StaggerZ();
        }

        //private void GroupPins()
        //{
        //    foreach (MapObject mapObject in MapObjects)
        //    {
        //        if (mapObject is RMMPin pin)
        //        {
        //            if (GroupedPins.ContainsKey(pin.PoolGroup))
        //            {
        //                GroupedPins[pin.PoolGroup].Add(pin);
        //            }
        //            else
        //            {
        //                GroupedPins[pin.PoolGroup] = new() { pin };
        //            }
        //        }
        //    }
        //}
    }
}
