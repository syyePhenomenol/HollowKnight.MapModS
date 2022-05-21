using MapModS.Data;
using MapModS.Settings;
using UnityEngine.SceneManagement;

namespace MapModS.UI
{
    public static class GUI
    {
        public static void Hook()
        {
            On.GameMap.Start += GameMap_Start;
            On.GameManager.SetGameMap += GameManager_SetGameMap;
            On.GameMap.WorldMap += GameMap_WorldMap;
            On.GameMap.CloseQuickMap += GameMap_CloseQuickMap;
            On.HeroController.Pause += HeroController_Pause;
            On.HeroController.UnPause += HeroController_UnPause;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += HandleSceneChanges;

            GUIController.Setup();
        }

        public static void Unhook()
        {
            On.GameMap.Start -= GameMap_Start;
            On.GameManager.SetGameMap -= GameManager_SetGameMap;
            On.GameMap.WorldMap -= GameMap_WorldMap;
            On.GameMap.CloseQuickMap -= GameMap_CloseQuickMap;
            On.HeroController.Pause -= HeroController_Pause;
            On.HeroController.UnPause -= HeroController_UnPause;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= HandleSceneChanges;

            GUIController.Unload();
            TransitionText.ClearData();

            PauseMenu.DestroyMenu();
        }

        private static void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
        {
            orig(self);
            
            GUIController.Instance.BuildMenus();
        }

        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, UnityEngine.GameObject go_gameMap)
        {
            orig(self, go_gameMap);

            PauseMenu.BuildMenu();
        }

        private static void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);

            MapText.Show();
            TransitionText.ShowWorldMap();
            LookupText.ShowWorldMap();
        }

        private static void GameMap_CloseQuickMap(On.GameMap.orig_CloseQuickMap orig, GameMap self)
        {
            orig(self);

            MapText.Hide();
            TransitionText.Hide();
            LookupText.Hide();
        }

        private static void HeroController_Pause(On.HeroController.orig_Pause orig, HeroController self)
        {
            orig(self);
            TransitionText.SetRouteActive();
        }

        private static void HeroController_UnPause(On.HeroController.orig_UnPause orig, HeroController self)
        {
            orig(self);
            TransitionText.SetRouteActive();

            PauseMenu.CollapsePanel();
        }

        private static void HandleSceneChanges(Scene from, Scene to)
        {
            if (GameManager.instance.sceneName != to.name) return;

            TransitionText.RemoveTraversedTransition(from.name, to.name);

            RouteCompass.CreateRouteCompass();
            RouteCompass.UpdateCompass();
        }
    }
}