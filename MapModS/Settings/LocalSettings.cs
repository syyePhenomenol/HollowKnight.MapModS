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

    [Serializable]
    public class LocalSettings
    {
        public class GroupSetting
        {
            public GroupSetting()
            {
                On = true;
            }

            public bool On;
        };

        public Dictionary<PoolGroup, GroupSetting> GroupSettings = Enum.GetValues(typeof(PoolGroup))
            .Cast<PoolGroup>().ToDictionary(t => t, t => new GroupSetting());

        public Dictionary<string, bool> ObtainedVanillaItems = new();

        // Vanilla only
        public int GeoRockCounter = 0;

        public bool showBenchPins = false;

        public bool ModEnabled = false;

        public MapMode mapMode = MapMode.FullMap;

        public bool lookupOn = false;

        public GroupBy groupBy;

        public bool SpoilerOn = false;

        public bool NewSettings = true;

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

        public bool GetOnFromGroup(string groupName)
        {
            if (Enum.TryParse(groupName, out PoolGroup group))
            {
                return GroupSettings[group].On;
            }

            return false;
        }

        public bool GetOnFromGroup(PoolGroup group)
        {
            if (GroupSettings.ContainsKey(group))
            {
                return GroupSettings[group].On;
            }

            return false;
        }

        public void SetOnFromGroup(string groupName, bool value)
        {
            if (Enum.TryParse(groupName, out PoolGroup group))
            {
                GroupSettings[group].On = value;
            }
        }

        public void SetOnFromGroup(PoolGroup group, bool value)
        {
            if (GroupSettings.ContainsKey(group))
            {
                GroupSettings[group].On = value;
            }
        }
    }
}