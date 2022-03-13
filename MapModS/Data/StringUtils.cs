using GlobalEnums;
using System.Text.RegularExpressions;

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

        public static string ToCleanPreviewText(string text)
        {
            return text.Replace("Pay ", "")
                .Replace("Once you own ", "")
                .Replace(", I'll gladly sell it to you.", "")
                .Replace("Requires ", "")
                .Replace("<br>", "");
        }

        public static string ToCleanName(string name)
        {
            return name.Replace("-", " ")
                .Replace("_", " ");
        }

        public static MapZone ToMapZone(string mapZone)
        {
            return mapZone switch
            {
                "Ancient Basin" => MapZone.ABYSS,
                "City of Tears" => MapZone.CITY,
                "Crystal Peak" => MapZone.MINES,
                "Deepnest" => MapZone.DEEPNEST,
                "Dirtmouth" => MapZone.TOWN,
                "Fog Canyon" => MapZone.FOG_CANYON,
                "Forgotten Crossroads" => MapZone.CROSSROADS,
                "Fungal Wastes" => MapZone.WASTES,
                "Greenpath" => MapZone.GREEN_PATH,
                "Howling Cliffs" => MapZone.CLIFFS,
                "Kingdom's Edge" => MapZone.OUTSKIRTS,
                "Queen's Gardens" => MapZone.ROYAL_GARDENS,
                "Resting Grounds" => MapZone.RESTING_GROUNDS,
                "Royal Waterways" => MapZone.WATERWAYS,
                "White Palace" => MapZone.WHITE_PALACE,
                _ => MapZone.NONE
            };
        }
    }
}
