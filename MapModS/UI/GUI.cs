using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MapModS.UI
{
    public static class GUI
    {
        public static void Hook()
        {
            On.GameMap.Start += GameMap_Start;
            On.GameMap.WorldMap += GameMap_WorldMap;
            On.GameMap.SetupMapMarkers += SetupMapMarkers;
            On.GameMap.DisableMarkers += GameMap_DisableMarkers;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += HandleSceneChanges;
        }

        public static void Unhook()
        {
            On.GameMap.Start -= GameMap_Start;
            On.GameMap.WorldMap -= GameMap_WorldMap;
            On.GameMap.SetupMapMarkers -= SetupMapMarkers;
            On.GameMap.DisableMarkers -= GameMap_DisableMarkers;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= HandleSceneChanges;

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

        private static void GameMap_WorldMap (On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);
            TransitionText.ShowInstructions();
        }

        private static void SetupMapMarkers(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            orig(self);
            MapText.Show();
            TransitionText.Show();
        }

        private static void GameMap_DisableMarkers(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            orig(self);
            MapText.Hide();
            TransitionText.Hide();
        }

        private static void HandleSceneChanges(Scene from, Scene to)
        {
            if (GameManager.instance.sceneName != to.name) return;

            TransitionText.RemoveTraversedTransition(from.name, to.name);
        }
    }
}