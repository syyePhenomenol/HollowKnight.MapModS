using System;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.SceneManagement;
using Vasi;

namespace MapModS.UI
{
    public static class GUI
    {
        public static void Hook()
        {
            On.GameMap.Start += GameMap_Start;
            //On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
            On.GameMap.WorldMap += GameMap_WorldMap;
            On.GameMap.CloseQuickMap += GameMap_CloseQuickMap;
            On.HeroController.Pause += HeroController_Pause;
            On.HeroController.UnPause += HeroController_UnPause;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += HandleSceneChanges;
        }
        public static void Unhook()
        {
            On.GameMap.Start -= GameMap_Start;
            //On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
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

        private static void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);

            MapText.Show();
            TransitionText.ShowWorldMap();
        }

        private static void GameMap_CloseQuickMap(On.GameMap.orig_CloseQuickMap orig, GameMap self)
        {
            orig(self);

            MapText.Hide();
            TransitionText.Hide();
        }

        //private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        //{
        //    orig(self);

        //    if (self.gameObject.name == "Knight" && self.FsmName == "Map Control"
        //        || self.FsmName == "Bench Control")
        //    {
        //        FsmUtil.AddAction(FsmUtil.GetState(self, "Map Idle"), new OpenQuickMap());
        //    }
        //}

        //public class OpenQuickMap : FsmStateAction
        //{
        //    public override void OnEnter()
        //    {
        //        MapText.Show();
        //        TransitionText.ShowQuickMap();

        //        Finish();
        //    }
        //}

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
        }
    }
}