using System.Collections;

namespace MapModS.UI
{
    public static class GUI
    {
        public static void Hook()
        {
            On.GameManager.SetGameMap += GameManager_SetGameMap;
            On.GameMap.SetupMapMarkers += SetupMapMarkers;
            On.GameMap.DisableMarkers += GameMap_DisableMarkers;
            On.QuitToMenu.Start += OnQuitToMenu;
        }

        private static void SetupMapMarkers(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            orig(self);

            MapText.Show();
        }

        private static void GameMap_DisableMarkers(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            orig(self);

            MapText.Hide();
        }

        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, UnityEngine.GameObject go_gameMap)
        {
            orig(self, go_gameMap);

            GUIController.Setup();
            GUIController.Instance.BuildMenus();
        }

        private static IEnumerator OnQuitToMenu(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            GUIController.Unload();

            return orig(self);
        }
    }
}