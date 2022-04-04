using Modding;
using System;
using UnityEngine.SceneManagement;

namespace MapModS.UI
{
    public static class GUI
    {
        public static void Hook()
        {
            On.GameMap.Start += GameMap_Start;
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
            On.GameMap.WorldMap -= GameMap_WorldMap;
            On.GameMap.CloseQuickMap -= GameMap_CloseQuickMap;
            On.HeroController.Pause -= HeroController_Pause;
            On.HeroController.UnPause -= HeroController_UnPause;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= HandleSceneChanges;

            GUIController.Unload();
        }
        private static void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
        {
            orig(self);
                
            GUIController.Instance.BuildMenus();
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
        }

        private static void HandleSceneChanges(Scene from, Scene to)
        {
            //MapModS.Instance.Log($"{from.name} to {to.name}");

            if (GameManager.instance.sceneName != to.name) return;

            TransitionText.RemoveTraversedTransition(from.name, to.name);

            RouteCompass.CreateRouteCompass();
            RouteCompass.UpdateCompass();
        }
    }
}