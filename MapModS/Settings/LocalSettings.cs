using System;
using System.Linq;
using System.Collections.Generic;
using MapModS.Data;

namespace MapModS.Settings
{
	[Serializable]
	public class LocalSettings
	{
        public class GroupSettingPair
        {
            public GroupSettingPair()
            {
                Has = false;
                On = true;
            }

            public bool Has; // The corresponding pin has been bought
            public bool On; // The corresponding pin will be shown on the map (if Has)
        };

        public Dictionary<string, bool> ObtainedVanillaItems = new();

		public int GeoRockCounter = 0;

		public Dictionary<PoolGroup , GroupSettingPair> GroupSettings = Enum.GetValues(typeof(PoolGroup))
			.Cast<PoolGroup>().ToDictionary(t => t, t => new GroupSettingPair());

		public bool RevealFullMap = false;

        public void ToggleFullMap()
        {
			RevealFullMap = !RevealFullMap;

			// Force all pins to show again
			foreach (KeyValuePair<PoolGroup, GroupSettingPair> entry in GroupSettings)
            {
                entry.Value.On = true;
            }
        }

        public bool GetHasFromGroup(string groupName)
        {
			if (Enum.TryParse(groupName, out PoolGroup group))
            {
				return GroupSettings[group].Has;
			}

            return false;
        }

        public bool GetHasFromGroup(PoolGroup group)
		{
			return GroupSettings[group].Has;
		}

		//public void SetHasFromGroup(string groupName, bool value)
		//{
		//	if (Enum.TryParse(groupName, out PoolGroup group))
		//	{
		//		GroupSettings[group].Has = value;
		//	}
		//	else
		//	{
		//		// Set based on ORIGINAL PlayerData settings
		//		switch (groupName)
		//		{
		//			case "hasPinBench":
		//				GroupSettings[PoolGroup.Bench].Has = value;
		//				break;
		//			case "hasPinCocoon":
		//				GroupSettings[PoolGroup.Cocoon].Has = value;
		//				break;
		//			case "hasPinDreamPlant":
		//				GroupSettings[PoolGroup.Root].Has = value;
		//				break;
		//			case "hasPinGhost":
		//				GroupSettings[PoolGroup.Grave].Has = value;
		//				break;
		//			case "hasPinGrub":
		//				GroupSettings[PoolGroup.Grub].Has = value;
		//				break;
		//			case "hasPinShop":
		//				GroupSettings[PoolGroup.Vendor].Has = value;
		//				break;
		//			case "hasPinSpa":
		//				GroupSettings[PoolGroup.Spa].Has = value;
		//				break;
		//			case "hasPinStag":
		//				GroupSettings[PoolGroup.Stag].Has = value;
		//				break;
		//			case "hasPinTram":
		//				GroupSettings[PoolGroup.Tram].Has = value;
		//				break;

		//			default:
		//				break;
		//		};
		//	}
		//}

		public bool HasNoGroup()
		{
			if (RevealFullMap)
            {
				return false;
            }

			foreach (GroupSettingPair settingPair in GroupSettings.Values)
			{
				if (settingPair.Has)
				{
					return false;
				}
			}

			return true;
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

		public void ToggleGroups()
		{
			if (AllHasIsOff())
			{
				foreach (GroupSettingPair settingPair in GroupSettings.Values)
				{
					if (settingPair.Has || RevealFullMap)
					{
						settingPair.On = true;
					}
				}
			}
			else
			{
				foreach (GroupSettingPair settingPair in GroupSettings.Values)
				{
					if (settingPair.Has || RevealFullMap)
					{
						settingPair.On = false;
					}
				}
			}
		}

		public bool AllHasIsOn()
		{
			foreach (GroupSettingPair settingPair in GroupSettings.Values)
			{
				if (settingPair.Has || RevealFullMap)
				{
					if (!settingPair.On)
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool AllHasIsOff()
		{
			foreach (GroupSettingPair settingPair in GroupSettings.Values)
			{
				if (settingPair.Has || RevealFullMap)
				{
					if (settingPair.On)
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}