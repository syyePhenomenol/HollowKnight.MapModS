using System;
using System.Linq;
using System.Collections.Generic;
using MapModS.Data;
using MapModS.Map;

namespace MapModS.Settings
{
	public enum Mode
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
                //Has = false;
                On = true;
            }

            //public bool Has; // The corresponding pin has been bought
            public bool On; // The corresponding pin will be shown on the map (if Has)
        };

		public Dictionary<PoolGroup, GroupSetting> GroupSettings = Enum.GetValues(typeof(PoolGroup))
			.Cast<PoolGroup>().ToDictionary(t => t, t => new GroupSetting());

		public Dictionary<string, bool> ObtainedVanillaItems = new();

		//public int GeoRockCounter = 0;

		public bool RevealFullMap = false;

		public bool ModEnabled = false;

		public Mode mapState = Mode.FullMap;

		public PinStyle pinStyle = PinStyle.Normal;

		public bool SpoilerOn = false;

		public bool RandomizedOn = true;

		public bool OthersOn = true;

		public void ToggleModEnabled()
        {
			ModEnabled = !ModEnabled;
			//GameManager.instance.SaveGame();
		}

		public void ToggleFullMap()
        {
			switch (mapState)
            {
				case Mode.FullMap:
					FullMap.PurgeMap();
					mapState = Mode.AllPins;
					break;
				case Mode.AllPins:
					mapState = Mode.PinsOverMap;
					break;
				case Mode.PinsOverMap:
					mapState = Mode.FullMap;
					break;
            }

			//// Force all pins to show again
			//foreach (KeyValuePair<PoolGroup, GroupSetting> entry in GroupSettings)
   //         {
   //             entry.Value.On = true;
   //         }
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

		public void ToggleGroups()
		{
			if (AllGroupsOff())
			{
				foreach (GroupSetting setting in GroupSettings.Values)
				{
					setting.On = true;
				}
			}
			else
			{
				foreach (GroupSetting setting in GroupSettings.Values)
				{
					setting.On = false;
				}
			}
		}

		public bool AllGroupsOn()
		{
			foreach (GroupSetting setting in GroupSettings.Values)
			{
				if (!setting.On)
				{
					return false;
				}
			}

			return true;
		}

		public bool AllGroupsOff()
		{
			foreach (GroupSetting setting in GroupSettings.Values)
			{
				if (setting.On)
				{
					return false;
				}
			}
			return true;
		}
	}
}