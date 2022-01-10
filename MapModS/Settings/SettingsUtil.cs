using GlobalEnums;

namespace MapModS.Settings
{
    public static class SettingsUtil
    {
        //public static bool GetMMSMapSetting(MapZone mapZone)
        //{
        //    return mapZone switch
        //    {
        //        MapZone.ABYSS => PlayerData.instance.GetBool("MMS_mapAbyss"),
        //        MapZone.CITY => PlayerData.instance.GetBool("MMS_mapCity"),
        //        MapZone.CLIFFS => PlayerData.instance.GetBool("MMS_mapCliffs"),
        //        MapZone.CROSSROADS => PlayerData.instance.GetBool("MMS_mapCrossroads"),
        //        MapZone.MINES => PlayerData.instance.GetBool("MMS_mapMines"),
        //        MapZone.DEEPNEST => PlayerData.instance.GetBool("MMS_mapDeepnest"),
        //        MapZone.TOWN => PlayerData.instance.GetBool("MMS_mapDirtmouth"),
        //        MapZone.FOG_CANYON => PlayerData.instance.GetBool("MMS_mapFogCanyon"),
        //        MapZone.WASTES => PlayerData.instance.GetBool("MMS_mapFungalWastes"),
        //        MapZone.GREEN_PATH => PlayerData.instance.GetBool("MMS_mapGreenpath"),
        //        MapZone.OUTSKIRTS => PlayerData.instance.GetBool("MMS_mapOutskirts"),
        //        MapZone.ROYAL_GARDENS => PlayerData.instance.GetBool("MMS_mapRoyalGardens"),
        //        MapZone.RESTING_GROUNDS => PlayerData.instance.GetBool("MMS_mapRestingGrounds"),
        //        MapZone.WATERWAYS => PlayerData.instance.GetBool("MMS_mapWaterways"),
        //        MapZone.WHITE_PALACE => PlayerData.instance.GetBool("MMS_AdditionalMapsGotWpMap"),
        //        MapZone.GODS_GLORY => PlayerData.instance.GetBool("MMS_AdditionalMapsGotGhMap"),
        //        _ => false,
        //    };
        //}

        public static bool GetMapSetting(MapZone mapZone)
        {
            return mapZone switch
            {
                MapZone.ABYSS => PlayerData.instance.GetBool("mapAbyss"),
                MapZone.CITY => PlayerData.instance.GetBool("mapCity"),
                MapZone.CLIFFS => PlayerData.instance.GetBool("mapCliffs"),
                MapZone.CROSSROADS => PlayerData.instance.GetBool("mapCrossroads"),
                MapZone.MINES => PlayerData.instance.GetBool("mapMines"),
                MapZone.DEEPNEST => PlayerData.instance.GetBool("mapDeepnest"),
                MapZone.TOWN => PlayerData.instance.GetBool("mapDirtmouth"),
                MapZone.FOG_CANYON => PlayerData.instance.GetBool("mapFogCanyon"),
                MapZone.WASTES => PlayerData.instance.GetBool("mapFungalWastes"),
                MapZone.GREEN_PATH => PlayerData.instance.GetBool("mapGreenpath"),
                MapZone.OUTSKIRTS => PlayerData.instance.GetBool("mapOutskirts"),
                MapZone.ROYAL_GARDENS => PlayerData.instance.GetBool("mapRoyalGardens"),
                MapZone.RESTING_GROUNDS => PlayerData.instance.GetBool("mapRestingGrounds"),
                MapZone.WATERWAYS => PlayerData.instance.GetBool("mapWaterways"),
                MapZone.WHITE_PALACE => PlayerData.instance.GetBool("AdditionalMapsGotWpMap"),
                MapZone.GODS_GLORY => PlayerData.instance.GetBool("AdditionalMapsGotGhMap"),
                _ => false,
            };
        }

        //public static void SyncPlayerDataSettings()
        //{
        //    // The Has settings should be equivalent to the ORIGINAL PlayerData settings
        //    MapModS.LS.GroupSettings[PoolGroup.Bench].Has = PlayerData.instance.GetBool("hasPinBench");
        //    MapModS.LS.GroupSettings[PoolGroup.Cocoon].Has = PlayerData.instance.GetBool("hasPinCocoon");
        //    MapModS.LS.GroupSettings[PoolGroup.Grave].Has = PlayerData.instance.GetBool("hasPinGhost");
        //    MapModS.LS.GroupSettings[PoolGroup.Grub].Has = PlayerData.instance.GetBool("hasPinGrub");
        //    MapModS.LS.GroupSettings[PoolGroup.Root].Has = PlayerData.instance.GetBool("hasPinDreamPlant");
        //    MapModS.LS.GroupSettings[PoolGroup.Spa].Has = PlayerData.instance.GetBool("hasPinSpa");
        //    MapModS.LS.GroupSettings[PoolGroup.Stag].Has = PlayerData.instance.GetBool("hasPinStag");
        //    MapModS.LS.GroupSettings[PoolGroup.Tram].Has = PlayerData.instance.GetBool("hasPinTram");
        //    MapModS.LS.GroupSettings[PoolGroup.Vendor].Has = PlayerData.instance.GetBool("hasPinShop");
        //}

        public static bool IsFSMMapState(string name)
        {
            return name switch
            {
                "Abyss"
                or "Ancient Basin"
                or "City"
                or "Cliffs"
                or "Crossroads"
                or "Deepnest"
                or "Fog Canyon"
                or "Fungal Wastes"
                or "Fungus"
                or "Greenpath"
                or "Hive"
                or "Mines"
                or "Outskirts"
                or "Resting Grounds"
                or "Royal Gardens"
                or "Waterways"
                or "WHITE_PALACE"
                or "GODS_GLORY" => true,
                _ => false,
            };
            ;
        }
    }
}