using MapModS.Pins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapModS.Settings
{
    public class RandoLocalSettings : LocalSettings
    {
        public bool SpoilerOn = false;
        public bool RandomizedOn = true;
        public bool VanillaOn = false;
        public Dictionary<string, PoolState> PoolSettings;
        public GroupBySetting GroupBy = GroupBySetting.Location;

        public override void InitializeDerived()
        {
            PoolSettings = RandoPinData.PoolGroups.ToDictionary(poolGroup => poolGroup, poolGroup => PoolState.On);

            if (MapModS.GS.OverrideDefaultMode)
            {
                // Replace with condition that at least one randomized transition exists
                if (true)
                {
                    SetMode(MapModS.GS.TransitionRandoModeOverride);
                }
                else
                {
                    SetMode(MapModS.GS.ItemRandoModeOverride);
                }
            }
            else
            {
                // Replace with condition that at least one randomized transition exists
                if (true)
                {
                    SetMode(MapMode.FullMap);
                }
                else
                {
                    SetMode(MapMode.Transition);
                }
            }

            ResetPoolSettings();
        }
        internal void ToggleGroupBy()
        {
            GroupBy = (GroupBySetting)(((int)GroupBy + 1) % Enum.GetNames(typeof(GroupBySetting)).Length);
        }

        internal void ToggleBench()
        {
            ShowBenchPins = !ShowBenchPins;
        }

        internal void ToggleSpoilers()
        {
            SpoilerOn = !SpoilerOn;
        }

        internal void ToggleRandomizedOn()
        {
            RandomizedOn = !RandomizedOn;
            ResetPoolSettings();
        }

        internal void ToggleOthersOn()
        {
            VanillaOn = !VanillaOn;
            ResetPoolSettings();
        }

        internal PoolState GetPoolGroupSetting(string poolGroup)
        {
            if (PoolSettings.ContainsKey(poolGroup))
            {
                return PoolSettings[poolGroup];
            }
            return PoolState.Off;
        }

        internal void SetPoolGroupSetting(string poolGroup, PoolState state)
        {
            if (PoolSettings.ContainsKey(poolGroup))
            {
                PoolSettings[poolGroup] = state;
            }
        }

        internal void TogglePoolGroupSetting(string poolGroup)
        {
            if (!PoolSettings.ContainsKey(poolGroup)) return;

            PoolSettings[poolGroup] = PoolSettings[poolGroup] switch
            {
                PoolState.Off => PoolState.On,
                PoolState.On => PoolState.Off,
                PoolState.Mixed => PoolState.On,
                _ => PoolState.On
            };
        }

        private void ResetPoolSettings()
        {
            foreach (string poolGroup in RandoPinData.PoolGroups)
            {
                SetPoolGroupSetting(poolGroup, GetResetPoolState(poolGroup));
            }

            PoolState GetResetPoolState(string poolGroup)
            {
                bool IsRando = RandoPinData.RandoPoolGroups.Contains(poolGroup);
                bool IsVanilla = RandoPinData.VanillaPoolGroups.Contains(poolGroup);

                if (IsRando && IsVanilla && MapModS.LS.RandomizedOn != MapModS.LS.VanillaOn)
                {
                    return PoolState.Mixed;
                }
                if ((IsRando && RandomizedOn) || (IsVanilla && VanillaOn))
                {
                    return PoolState.On;
                }
                return PoolState.Off;
            }
        }
    }
}
