using System;
using System.Collections.Generic;
using System.Linq;
using RandoMapMod.Pins;

namespace RandoMapMod.Settings
{
    public class LocalSettings
    {
        public bool InitializedPreviously = false;
        public HashSet<string> ScenesVisited;

        //// Vanilla only
        //public int geoRockCounter = 0;

        public bool ModEnabled = false;
        public RMMMode Mode = RMMMode.Full_Map;
        public bool ShowBenchPins = false;
        public bool SpoilerOn = false;
        public bool RandomizedOn = true;
        public bool VanillaOn = false;
        public Dictionary<string, PoolState> PoolSettings;
        public GroupBySetting GroupBy = GroupBySetting.Location;

        public void Initialize()
        {
            ScenesVisited = new(PlayerData.instance.scenesVisited);

            if (InitializedPreviously) return;

            PoolSettings = RmmPinMaster.PoolGroups.ToDictionary(poolGroup => poolGroup, poolGroup => PoolState.On);

            //if (RandoMapMod.GS.OverrideDefaultMode)
            //{
            //    // Replace with condition that at least one randomized transition exists
            //    if (true)
            //    {
            //        SetMode(RandoMapMod.GS.TransitionRandoModeOverride);
            //    }
            //    else
            //    {
            //        SetMode(RandoMapMod.GS.ItemRandoModeOverride);
            //    }
            //}
            //else
            //{
            //    // Replace with condition that at least one randomized transition exists
            //    if (true)
            //    {
            //        SetMode(RMMMode.Full_Map);
            //    }
            //    else
            //    {
            //        SetMode(RMMMode.Transition_1);
            //    }
            //}

            ResetPoolSettings();

            InitializedPreviously = true;
        }

        internal void ToggleModEnabled()
        {
            ModEnabled = !ModEnabled;
        }

        internal void SetMode(RMMMode mode)
        {
            Mode = mode;
        }

        internal void ToggleMode()
        {
            Mode = (RMMMode)(((int)Mode + 1) % Enum.GetNames(typeof(RMMMode)).Length);
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

        internal void ToggleRandomized()
        {
            RandomizedOn = !RandomizedOn;
            ResetPoolSettings();
        }

        internal void ToggleVanilla()
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
            foreach (string poolGroup in RmmPinMaster.PoolGroups)
            {
                SetPoolGroupSetting(poolGroup, GetResetPoolState(poolGroup));
            }

            PoolState GetResetPoolState(string poolGroup)
            {
                bool IsRando = RmmPinMaster.RandoPoolGroups.Contains(poolGroup);
                bool IsVanilla = RmmPinMaster.VanillaPoolGroups.Contains(poolGroup);

                if (IsRando && IsVanilla && RandoMapMod.LS.RandomizedOn != RandoMapMod.LS.VanillaOn)
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