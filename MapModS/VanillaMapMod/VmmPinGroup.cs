using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector.Util;
using MapChanger.MonoBehaviours;

namespace VanillaMapMod
{
    internal class VmmPinGroup : MapObjectGroup
    {
        internal Dictionary<PoolGroup, List<VmmPin>> GroupedPins { get; private set; } = new();

        public void InitializeGroups()
        {
            foreach (MapObject mapObject in MapObjects)
            {
                if (mapObject is VmmPin pin)
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
            foreach ((PoolGroup poolGroup, List<VmmPin> pins) in GroupedPins.Select(kvp => (kvp.Key, kvp.Value)))
            {
                foreach (VmmPin pin in pins)
                {
                    pin.Active = VanillaMapMod.LS.GetPoolGroupSetting(poolGroup);
                }
            }
        }
    }
}
