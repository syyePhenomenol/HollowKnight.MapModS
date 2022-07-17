using System;
using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector.Util;

namespace VanillaMapMod.Settings
{
    public class LocalSettings
    {
        public bool InitializedPreviously = false;

        //// Vanilla only
        //public int geoRockCounter = 0;

        public bool ModEnabled = false;

        public MapMode Mode = MapMode.VMM_FullMap;

        public Dictionary<PoolGroup, bool> PoolSettings = Enum.GetValues(typeof(PoolGroup))
               .Cast<PoolGroup>()
               .ToDictionary(t => t, t => true);

        internal void Initialize()
        {
            if (InitializedPreviously) return;

            InitializedPreviously = true;
        }

        internal void ToggleModEnabled()
        {
            ModEnabled = !ModEnabled;
        }

        internal void SetMode(MapMode mode)
        {
            Mode = mode;
        }

        internal void ToggleMode()
        {
            Mode = (MapMode)(((int)Mode + 1) % Enum.GetNames(typeof(MapMode)).Length);
        }

        internal bool GetPoolGroupSetting(PoolGroup poolGroup)
        {
            if (PoolSettings.ContainsKey(poolGroup))
            {
                return PoolSettings[poolGroup];
            }
            return false;
        }

        internal void SetPoolGroupSetting(PoolGroup poolGroup, bool value)
        {
            if (PoolSettings.ContainsKey(poolGroup))
            {
                PoolSettings[poolGroup] = value;
            }
        }

        internal void TogglePoolGroupSetting(PoolGroup poolGroup)
        {
            if (!PoolSettings.ContainsKey(poolGroup)) return;

            PoolSettings[poolGroup] = !PoolSettings[poolGroup];
        }
    }
}