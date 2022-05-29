using MapModS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using RM = RandomizerMod.RandomizerMod;
using TM = RandomizerMod.Settings.TransitionSettings.TransitionMode;

namespace MapModS.Settings
{
    public enum MapMode
    {
        FullMap,
        AllPins,
        PinsOverMap,
        TransitionRando,
        TransitionRandoAlt,
    }

    public enum GroupBy
    {
        Location,
        Item
    }

    public enum PoolGroupState
    {
        Off,
        On,
        Mixed
    }

    public class SettingPair
    {
        public SettingPair(string poolGroup, PoolGroupState state)
        {
            this.poolGroup = poolGroup;
            this.state = state;
        }

        public string poolGroup;
        public PoolGroupState state;
    }

    [Serializable]
    public class LocalSettings
    {
        public Dictionary<string, bool> obtainedVanillaItems = new();

        // Vanilla only
        public int geoRockCounter = 0;

        public bool showBenchPins = false;

        public bool modEnabled = false;

        public MapMode mapMode = MapMode.FullMap;

        public GroupBy groupBy;

        public bool spoilerOn = false;

        public bool randomizedOn = true;

        public bool othersOn = false;

        public bool newSettings = true;

        public List<SettingPair> poolGroupSettings = new();

        public void ToggleModEnabled()
        {
            modEnabled = !modEnabled;
        }

        public void ToggleMapMode()
        {
            switch (mapMode)
            {
                case MapMode.FullMap:
                    mapMode = MapMode.AllPins;
                    break;

                case MapMode.AllPins:
                    mapMode = MapMode.PinsOverMap;
                    break;

                case MapMode.PinsOverMap:
                    mapMode = MapMode.TransitionRando;
                    break;

                case MapMode.TransitionRando:
                    if (RM.RS.GenerationSettings.TransitionSettings.Mode != TM.RoomRandomizer)
                    {
                        mapMode = MapMode.TransitionRandoAlt;
                    }
                    else
                    {
                        mapMode = MapMode.FullMap;
                    }
                    break;

                case MapMode.TransitionRandoAlt:
                    mapMode = MapMode.FullMap;
                    break;
            }
        }

        public void ToggleBench()
        {
            showBenchPins = !showBenchPins;
        }

        public void ToggleGroupBy()
        {
            switch (groupBy)
            {
                case GroupBy.Location:
                    groupBy += 1;
                    break;
                default:
                    groupBy = GroupBy.Location;
                    break;
            }
        }

        public void ToggleSpoilers()
        {
            spoilerOn = !spoilerOn;
        }

        public void ToggleRandomizedOn()
        {
            randomizedOn = !randomizedOn;
        }

        public void ToggleOthersOn()
        {
            othersOn = !othersOn;
        }

        public void InitializePoolGroupSettings()
        {
            poolGroupSettings = MainData.usedPoolGroups.Select(p => new SettingPair(p, PoolGroupState.On)).ToList();
        }

        public PoolGroupState GetPoolGroupSetting(string poolGroup)
        {
            var item = poolGroupSettings.FirstOrDefault(s => s.poolGroup == poolGroup);

            if (item != null)
            {
                return item.state;
            }

            return PoolGroupState.Off;
        }

        public void SetPoolGroupSetting(string poolGroup, PoolGroupState state)
        {
            var item = poolGroupSettings.FirstOrDefault(s => s.poolGroup == poolGroup);

            if (item != null)
            {
                item.state = state;
            }
        }

        public void TogglePoolGroupSetting(string poolGroup)
        {
            var item = poolGroupSettings.FirstOrDefault(s => s.poolGroup == poolGroup);

            if (item != null)
            {
                item.state = item.state switch
                {
                    PoolGroupState.Off => PoolGroupState.On,
                    PoolGroupState.On => PoolGroupState.Off,
                    PoolGroupState.Mixed => PoolGroupState.On,
                    _ => throw new NotImplementedException()
                };
            }
        }
    }
}