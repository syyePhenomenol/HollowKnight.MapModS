using System;
using System.Collections.Generic;
using System.Linq;
using RandomizerMod.RandomizerData;

namespace MapModS.Data
{
	public static class DataLoader
	{
		private static Dictionary<string, PinDef> _pins;
		//private static Dictionary<string, VanillaItemDef> _vanillaItems;
		//private static Dictionary<string, ShopDef> _shop;
		//private static Dictionary<string, Dictionary<string, string>> _languageStrings;

		//public static PinDef GetPinDef(string name)
		//{
		//    if (_pins.TryGetValue(name, out PinDef def)) return def;

		//    MapModS.Instance.LogWarn($"Unable to find PinDef for {name}.");

		//    return null;
		//}

		public static PinDef[] GetPinArray()
		{
			return _pins.Values.ToArray();
		}

		//public static VanillaItemDef GetVanillaItemDefFromLocation(string location)
		//{
		//    if (_vanillaItems.TryGetValue(location, out VanillaItemDef def)) return def;

		//    MapModS.Instance.LogWarn($"Unable to find VanillaItemDef for {location}.");

		//    return null;
		//}

		public static PoolGroup GetRandomizerPoolGroup(string location)
		{
			if (RandomizerMod.RandomizerMod.RS.Context.itemPlacements.Any(pair => pair.location.Name == location))
			{
				RandomizerCore.ItemPlacement ilp = RandomizerMod.RandomizerMod.RS.Context.itemPlacements.First(pair => pair.location.Name == location);

				string cleanItemName = ilp.item.Name.Replace("Placeholder-", "").Split('-')[0];

				MapModS.Instance.Log(cleanItemName);

				return GetPoolGroupFromName(cleanItemName);

			}

			MapModS.Instance.LogWarn($"Unable to find RandomizerItemDef for {location}.");

			return PoolGroup.Unknown;
		}

		public static PoolGroup GetPoolGroupFromName(string cleanItemName)
		{
			switch (cleanItemName)
            {
				case "Dreamer":
					return PoolGroup.Dreamers;
				case "Grimmchild1":
				case "Grimmchild2":
					return PoolGroup.Charms;
				default:
					break;
            }

			foreach (PoolDef poolDef in RandomizerMod.RandomizerData.Data.Pools)
			{
				foreach (string includeItem in poolDef.IncludeItems)
                {
					if (includeItem.StartsWith(cleanItemName))
					{
						PoolGroup group = (PoolGroup)Enum.Parse(typeof(PoolGroup), poolDef.Group);
						MapModS.Instance.Log(group);
						return group;
					}
				}
			}

			MapModS.Instance.LogWarn($"{cleanItemName} not found in PoolDefs");
			
			return PoolGroup.Unknown;
        }

		// This method finds the vanilla and spoiler PoolGroups corresponding to each Pin, using RandomizerMod's ItemPlacements array
		// Called once per save load
		public static void FindPoolGroups()
		{
			foreach (KeyValuePair<string, PinDef> entry in _pins)
			{
				string vanillaItem = entry.Key;
				PinDef pinD = entry.Value;

				//if (pinD.NotPin)
				//{
				//	continue;
				//}

				// First check if this is a shop pin
				if (pinD.isShop)
				{
					continue;

					// Then check if this item is randomized
				}
				else
                {
					pinD.isRandomized = true;
					pinD.spoilerPool = GetRandomizerPoolGroup(vanillaItem);
				}
				//else if (RandomizerMod.RandomizerMod.RS.Context.itemPlacements.Any(pair => pair.location.Name == vanillaItem))
				//{
					//RandomizerCore.ItemPlacement ilp = RandomizerMod.RandomizerMod.RS.Context.itemPlacements.First(pair => pair.location.Name == vanillaItem);
					//spoilerItem = ilp.item.Name.Replace("Placeholder-", "");

					//// If spoilerItem's in the PinDataDictionary, use that Value
					//if (PinDataDictionary.ContainsKey(spoilerItem))
					//{
					//	pinD.SpoilerPool = PinDataDictionary[spoilerItem].VanillaPool;

					//	// Items that are not in RandomizerMod's xml files but are created during randomization
					//}
					//else if (Dictionaries.IsClonedItem(spoilerItem))
					//{
					//	pinD.SpoilerPool = Dictionaries.GetPoolFromClonedItem(spoilerItem);

					//}
					//// For cursed mode
					//else if (spoilerItem.StartsWith("1_Geo") || spoilerItem.StartsWith("Lumafly_Escape"))
					//{
					//	pinD.SpoilerPool = "CursedGeo";

					//	// Nothing should end up here!
					//}
					//else if (spoilerItem == "Grub" || spoilerItem == "Mask_Shard" || spoilerItem == "Vessel_Fragment" || spoilerItem == "Rancid_Egg" || spoilerItem == "Charm_Notch")
					//{
					//	pinD.SpoilerPool = spoilerItem;
					//}
					//else if (spoilerItem == "Wanderer's_Journal" || spoilerItem == "Hallownest_Seal" || spoilerItem == "King's_Idol" || spoilerItem == "Arcane_Egg")
					//{
					//	pinD.SpoilerPool = "Relic";
					//}
					//else if (spoilerItem == "Simple_Key")
					//{
					//	pinD.SpoilerPool = "Key";
					//}
					//else if (spoilerItem == "Pale_Ore")
					//{
					//	pinD.SpoilerPool = "Ore";
					//}
					//else if (spoilerItem.StartsWith("Grimmchild"))
					//{
					//	pinD.SpoilerPool = "Charm";
					//}
					//else if (spoilerItem.StartsWith("Lifeblood_Cocoon_"))
					//{
					//	pinD.SpoilerPool = "Cocoon";
					//}
					//else if (spoilerItem == "Grimmkin_Flame")
					//{
					//	pinD.SpoilerPool = "Flame";
					//}
					//else if (spoilerItem.StartsWith("Soul_Totem-"))
					//{
					//	pinD.SpoilerPool = "Soul";
					//}
					//else if (spoilerItem.StartsWith("Geo_Rock-"))
					//{
					//	pinD.SpoilerPool = "Rock";
					//}
					//else if (spoilerItem == "Dreamer")
					//{
					//	pinD.SpoilerPool = "Dreamer";
					//}
					//else if (spoilerItem == "One_Geo")
					//{
					//	pinD.SpoilerPool = "CursedGeo";
					//}
					//else if (spoilerItem == "Quill" || spoilerItem == "Deepnest_Map")
					//{
					//	pinD.SpoilerPool = "Map";
					//}
					//else
					//{
					//	pinD.SpoilerPool = pinD.VanillaPool;
					//	MapModS.Instance.LogWarn($"Item in RandomizerMod not recognized: {vanillaItem} -> {spoilerItem}, {pinD.VanillaPool}");
					//}

					// Don't create the Pin if it is not recognized by RandomizerMod
					// ElevatorPass Pin should not be created if ElevatorPass is false
				//}
				//else if (pinD.VanillaPool == "" || pinD.VanillaPool == "ElevatorPass")
				//{
				//	spoilerItem = vanillaItem;
				//	pinD.SpoilerPool = pinD.VanillaPool;
				//	pinD.NotPin = true;

				//	// These items are recognized by RandomizerMod, but not randomized
				//}
				//else
				//{
				//	spoilerItem = vanillaItem;
				//	pinD.SpoilerPool = pinD.VanillaPool;
				//}

				//MapModS.Instance.Log($"{vanillaItem} -> {spoilerItem}, {pinD.VanillaPool} -> {pinD.SpoilerPool}");
			}

			return;
		}

		//public static ItemDef GetItemDef(string name)
		//{
		//    if (_items.TryGetValue(name, out ItemDef def)) return def;

		//    MapModS.Instance.LogWarn($"Unable to find ItemDef for {name}.");

		//    return null;
		//}

		//public static bool IsPin(string item)
		//{
		//    return _pins.ContainsKey(item);
		//}

		//public static ShopDef[] GetShopArray()
		//{
		//    return _shop.Values.ToArray();
		//}

		//public static bool IsCustomLanguage(string sheet, string key)
		//{
		//    if (!_languageStrings.ContainsKey(sheet)) return false;

		//    if (!_languageStrings[sheet].ContainsKey(key)) return false;

		//    return true;
		//}

		//public static string GetCustomLanguage(string sheet, string key)
		//{
		//    return _languageStrings[sheet][key];
		//}

		public static void Load()
        {
            _pins = JsonUtil.Deserialize<Dictionary<string, PinDef>>("MapModS.Resources.pins.json");
            //_vanillaItems = JsonUtil.Deserialize<Dictionary<string, VanillaItemDef>>("MapModS.Resources.items.json");
            //_shop = JsonUtil.Deserialize<Dictionary<string, ShopDef>>("MapModS.Resources.shop.json");
            //_languageStrings = JsonUtil.Deserialize<Dictionary<string, Dictionary<string, string>>>("MapModS.Resources.language.json");

            //foreach (KeyValuePair<string, PinDef> entry in _pins)
            //{
            //    if (entry.Value.vanillaObjectName == null
            //        && entry.Value.vanillaPool != Pool.Cocoon
            //        && entry.Value.vanillaPool != Pool.Totem
            //        && entry.Value.vanillaPool != Pool.Lore)
            //    {
            //        MapModS.Instance.LogWarn($"There is a pin with no objectName that should have one: {entry.Key}");
            //    }
            //}
        }
    }
}