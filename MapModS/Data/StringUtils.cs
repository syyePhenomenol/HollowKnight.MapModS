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
    }
}
