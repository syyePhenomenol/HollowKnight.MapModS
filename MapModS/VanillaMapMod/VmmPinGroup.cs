using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector.Util;
using MapChanger.MonoBehaviours;

namespace VanillaMapMod
{
    internal class VMMPinGroup : MapObjectGroup
    {
        internal static VMMPinGroup Instance { get; private set; }

        internal Dictionary<PoolGroup, List<VMMPin>> GroupedPins { get; private set; } = new();

        public void InitializeGroups()
        {
            Instance = this;

            foreach (MapObject mapObject in MapObjects)
            {
                if (mapObject is VMMPin pin)
                {
                    if (GroupedPins.ContainsKey(pin.PoolGroup))
                    {
                        GroupedPins[pin.PoolGroup].Add(pin);
                    }
                    else
                    {
                        GroupedPins[pin.PoolGroup] = new() { pin };
                    }
                }
            }
        }

        public override void Set()
        {
            SetActive();
            base.Set();
        }

        private void SetActive()
        {
            foreach ((PoolGroup poolGroup, List<VMMPin> pins) in GroupedPins.Select(kvp => (kvp.Key, kvp.Value)))
            {
                foreach (VMMPin pin in pins)
                {
                    pin.Active = VanillaMapMod.LS.GetPoolGroupSetting(poolGroup);
                }
            }
        }
    }
}
