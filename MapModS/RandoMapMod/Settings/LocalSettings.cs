﻿using System;
using System.Collections.Generic;
using System.Linq;
using RandoMapMod.Pins;

namespace RandoMapMod.Settings
{
    public class LocalSettings
    {
        public bool InitializedPreviously = false;

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
            MapChanger.Settings.SetModEnabled(ModEnabled);
            MapChanger.Settings.SetMode("RandoMapMod", Mode.ToString().Replace('_', ' '));

            if (InitializedPreviously) return;

            PoolSettings = RmmPins.AllPoolGroups.ToDictionary(poolGroup => poolGroup, poolGroup => PoolState.On);

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

        //internal void ToggleModEnabled()
        //{
        //    ModEnabled = !ModEnabled;
        //}

        internal void SetMode(string mode)
        {
            if (Enum.TryParse(mode.Replace(' ', '_'), out RMMMode rmmMode))
            {
                Mode = rmmMode;
            }
        }

        //internal void ToggleMode()
        //{
        //    Mode = (RMMMode)(((int)Mode + 1) % Enum.GetNames(typeof(RMMMode)).Length);
        //}

        internal void ToggleGroupBy()
        {
            GroupBy = (GroupBySetting)(((int)GroupBy + 1) % Enum.GetNames(typeof(GroupBySetting)).Length);
            ResetPoolSettings();
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

        /// <summary>
        /// Reset the PoolGroups that are active based on the RandomizedOn, VanillaOn and Group By settings.
        /// When an individual pool that by default contains a mixed of randomized/vanilla placements gets toggled,
        /// It will either be forced to "On" or "Off" and the corresponding affected RandommizedOn/VanillaOn setting
        /// appears as "Custom" in the UI.
        /// </summary>
        private void ResetPoolSettings()
        {
            foreach (string poolGroup in RmmPins.AllPoolGroups)
            {
                SetPoolGroupSetting(poolGroup, GetResetPoolState(poolGroup));
            }

            PoolState GetResetPoolState(string poolGroup)
            {
                bool IsRando;
                bool IsVanilla;

                if (GroupBy == GroupBySetting.Item)
                {
                    IsRando = RmmPins.RandoItemPoolGroups.Contains(poolGroup);
                    IsVanilla = RmmPins.VanillaItemPoolGroups.Contains(poolGroup);
                }
                else
                {
                    IsRando = RmmPins.RandoLocationPoolGroups.Contains(poolGroup);
                    IsVanilla = RmmPins.VanillaLocationPoolGroups.Contains(poolGroup);
                }

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