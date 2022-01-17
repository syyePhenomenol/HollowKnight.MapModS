using System;
using System.Collections;

namespace MapModS.UI
{
    public static class GUI
    {
        public static void Hook()
        {
            On.GameMap.Start += GameMap_Start;
            On.GameMap.SetupMapMarkers += SetupMapMarkers;
            On.GameMap.DisableMarkers += GameMap_DisableMarkers;
            //On.QuitToMenu.Start += OnQuitToMenu;
        }

        public static void Unhook()
        {
            On.GameMap.Start -= GameMap_Start;
            On.GameMap.SetupMapMarkers -= SetupMapMarkers;
            On.GameMap.DisableMarkers -= GameMap_DisableMarkers;
            //On.QuitToMenu.Start -= OnQuitToMenu;

            GUIController.Unload();
        }

        private static void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
        {
            orig(self);

            try
            {
                GUIController.Setup();
                GUIController.Instance.BuildMenus();
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(e);
            }
        }

        private static void SetupMapMarkers(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            orig(self);

            MapText.Show();
            //TransitionText.Show();
        }

        private static void GameMap_DisableMarkers(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            orig(self);

            MapText.Hide();
            //TransitionText.Hide();
        }

        //private static IEnumerator OnQuitToMenu(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        //{
        //    return orig(self);
        //}
    }
}