using MapModS.Data;
using MapModS.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapModS.Settings
{
    public enum MapMode
    {
        FullMap,
        AllPins,
        PinsOverMap
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

        public bool RevealFullMap = false;

        public bool ModEnabled = false;

        public MapMode mapMode = MapMode.FullMap;

        public PinStyle pinStyle = PinStyle.Normal;

        public bool SpoilerOn = false;

        public bool RandomizedOn = true;

        public bool OthersOn = true;

        public void ToggleModEnabled()
        {
            ModEnabled = !ModEnabled;

            if (!GameManager.instance.IsGamePaused() && !HeroController.instance.controlReqlinquished)
            {
                UIManager.instance.checkpointSprite.Show();
                UIManager.instance.checkpointSprite.Hide();
            }

            if (!ModEnabled)
            {
                FullMap.PurgeMap();
            }
        }

        public void ToggleFullMap()
        {
            switch (mapMode)
            {
                case MapMode.FullMap:
                    FullMap.PurgeMap();
                    mapMode = MapMode.AllPins;
                    break;

                case MapMode.AllPins:
                    mapMode = MapMode.PinsOverMap;
                    break;

                case MapMode.PinsOverMap:
                    mapMode = MapMode.FullMap;
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