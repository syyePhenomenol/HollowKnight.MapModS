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

        public static string ToCleanGroup(PoolGroup poolGroup)
        {
            string[] splitGroup = Regex.Split(poolGroup.ToString(), @"(?<!^)(?=[A-Z])");

            if (splitGroup.Length == 0)
            {
                return "";
            }
            else if (splitGroup.Length == 1)
            {
                return splitGroup[0];
            }
            else
            {
                return splitGroup[0] + " " + splitGroup[1];
            }
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
    }
}
