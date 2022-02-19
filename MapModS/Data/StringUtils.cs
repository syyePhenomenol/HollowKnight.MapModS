using GlobalEnums;

namespace MapModS.Data
{
    public static class StringUtils
    {
        public static string DropSuffix(string scene)
        {
            if (scene == "") return "";

            string[] sceneSplit = scene.Split('_');

            string truncatedScene = sceneSplit[0];

            for (int i = 1; i < sceneSplit.Length - 1; i++)
            {
                truncatedScene += "_" + sceneSplit[i];
            }

            return truncatedScene;
        }

        public static string RemoveBossSuffix(string scene)
        {
            if (scene == null) return null;

            if (scene.EndsWith("_boss") || scene.EndsWith("_preload"))
            {
                return DropSuffix(scene);
            }

            if (scene.EndsWith("_boss_defeated"))
            {
                return DropSuffix(DropSuffix(scene));
            }

            return scene;
        }

        public static string CurrentNormalScene()
        {
            return RemoveBossSuffix(GameManager.instance.sceneName);
        }

        public static MapZone GetFixedMapZone()
        {
            switch (CurrentNormalScene())
            {
                case "Room_shop":
                case "Room_mapper":
                case "Room_Ouiji":
                case "Grimm_Main_Tent":
                case "Grimm_Divine":
                case "Room_Bretta":
                case "Room_Town_Stag_Station":
                    return MapZone.TOWN;
                case "Room_Charm_Shop":
                case "Room_temple":
                case "Crossroads_ShamanTemple":
                    return MapZone.CROSSROADS;
                case "RestingGrounds_07":
                case "Room_Mansion":
                    return MapZone.RESTING_GROUNDS;
                case "Fungus3_archive_02":
                case "Room_Fungus_Shaman":
                    return MapZone.FOG_CANYON;
                case "Deepnest_Spider_Town":
                case "Deepnest_45_v02":
                case "Room_spider_small":
                    return MapZone.DEEPNEST;
                case "Mines_35":
                    return MapZone.MINES;
                case "Room_nailmaster":
                case "Cliffs_03":
                    return MapZone.CLIFFS;
                case "Room_nailmaster_03":
                case "Room_Wyrm":
                case "Room_Colosseum_01":
                case "Deepnest_East_17":
                case "GG_Lurker":
                    return MapZone.OUTSKIRTS;
                case "Room_nailmaster_02":
                case "Fungus1_36":
                case "Fungus1_35":
                    return MapZone.GREEN_PATH;
                case "White_Palace_09":
                case "White_Palace_20":
                case "White_Palace_02":
                case "White_Palace_03_hub":
                case "White_Palace_04":
                case "White_Palace_15":
                case "White_Palace_17":
                case "White_Palace_18":
                case "White_Palace_19":
                case "White_Palace_08":
                case "White_Palace_12":
                case "White_Palace_14":
                    return MapZone.WHITE_PALACE;
                case "Abyss_15":
                    return MapZone.ABYSS;
                case "Ruins_House_01":
                case "Ruins_House_02":
                case "Ruins_House_03":
                    return MapZone.CITY;
                case "Room_GG_Shortcut":
                    return MapZone.WATERWAYS;
                default:
                    return MapZone.NONE;
            }
        }
    }
}
