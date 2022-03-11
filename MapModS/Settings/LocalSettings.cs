using MapModS.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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

    [Serializable]
    public class LocalSettings
    {
        public Dictionary<string, bool> ObtainedVanillaItems = new();

        // Vanilla only
        public int GeoRockCounter = 0;

        public bool showBenchPins = false;

        public bool ModEnabled = false;

        public MapMode mapMode = MapMode.FullMap;

        public bool lookupOn = false;

        public GroupBy groupBy;

        public bool SpoilerOn = false;

        public bool randomizedOn = true;

        public bool othersOn = false;

        public bool NewSettings = true;

        //public class GroupSetting
        //{
        //    public GroupSetting()
        //    {
        //        On = true;
        //    }

        //    public bool On;
        //};

        public Dictionary<PoolGroup, PoolGroupState> PoolGroupStates = Enum.GetValues(typeof(PoolGroup))
            .Cast<PoolGroup>().ToDictionary(t => t, t => PoolGroupState.On);

        public void ToggleModEnabled()
        {
            ModEnabled = !ModEnabled;
        }

        public void ToggleFullMap()
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
                    if (SettingsUtil.IsTransitionRando())
                    {
                        mapMode = MapMode.TransitionRando;
                    }
                    else
                    {
                        mapMode = MapMode.FullMap;
                    }
                    break;

                case MapMode.TransitionRando:
                    if (SettingsUtil.IsAreaRando())
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

        public void ToggleLookup()
        {
            lookupOn = !lookupOn;
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
            SpoilerOn = !SpoilerOn;
        }

        public void ToggleRandomizedOn()
        {
            randomizedOn = !randomizedOn;
        }

        public void ToggleOthersOn()
        {
            othersOn = !othersOn;
        }

        public PoolGroupState GetPoolGroupState(string groupName)
        {
            if (Enum.TryParse(groupName, out PoolGroup group))
            {
                return PoolGroupStates[group];
            }

            return PoolGroupState.Off;
        }

        public PoolGroupState GetPoolGroupState(PoolGroup group)
        {
            if (PoolGroupStates.ContainsKey(group))
            {
                return PoolGroupStates[group];
            }

            return PoolGroupState.Off;
        }

        public void SetPoolGroupState(string groupName, PoolGroupState state)
        {
            if (Enum.TryParse(groupName, out PoolGroup group))
            {
                PoolGroupStates[group] = state;
            }
        }

        public void SetPoolGroupState(PoolGroup group, PoolGroupState state)
        {
            if (PoolGroupStates.ContainsKey(group))
            {
                PoolGroupStates[group] = state;
            }
        }

        public void TogglePoolGroupState(string groupName)
        {
            if (Enum.TryParse(groupName, out PoolGroup group))
            {
                PoolGroupStates[group] = PoolGroupStates[group] switch
                {
                    PoolGroupState.Off => PoolGroupState.On,
                    PoolGroupState.On => PoolGroupState.Off,
                    PoolGroupState.Mixed => PoolGroupState.On,
                    _ => throw new NotImplementedException()
                };
            }
        }

        //public void SetAllGroupsOn()
        //{
        //    foreach (GroupSetting groupSetting in GroupSettings.Values)
        //    {
        //        groupSetting.On = true;
        //    }
        //}

        //public void SetAllGroupsOff()
        //{
        //    foreach (GroupSetting groupSetting in GroupSettings.Values)
        //    {
        //        groupSetting.On = false;
        //    }
        //}
    }
}