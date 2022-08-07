using MapChanger;
using UnityEngine.SceneManagement;

namespace RandoMapMod.UI
{
    internal class GUI : IMainHooks
    {
        public static bool worldMapOpen = false;
        public static bool quickMapOpen = false;
        public static bool lockToggleEnable = false;

        public void OnEnterGame()
        {
            On.GameMap.Start += GameMap_Start;
            On.GameManager.SetGameMap += GameManager_SetGameMap;
            On.GameMap.WorldMap += GameMap_WorldMap;
            On.GameMap.QuickMapAncientBasin += GameMap_QuickMapAncientBasin;
            On.GameMap.QuickMapCity += GameMap_QuickMapCity;
            On.GameMap.QuickMapCliffs += GameMap_QuickMapCliffs;
            On.GameMap.QuickMapCrossroads += GameMap_QuickMapCrossroads;
            On.GameMap.QuickMapCrystalPeak += GameMap_QuickMapCrystalPeak;
            On.GameMap.QuickMapDeepnest += GameMap_QuickMapDeepnest;
            On.GameMap.QuickMapDirtmouth += GameMap_QuickMapDirtmouth;
            On.GameMap.QuickMapFogCanyon += GameMap_QuickMapFogCanyon;
            On.GameMap.QuickMapFungalWastes += GameMap_QuickMapFungalWastes;
            On.GameMap.QuickMapGreenpath += GameMap_QuickMapGreenpath;
            On.GameMap.QuickMapKingdomsEdge += GameMap_QuickMapKingdomsEdge;
            On.GameMap.QuickMapQueensGardens += GameMap_QuickMapQueensGardens;
            On.GameMap.QuickMapRestingGrounds += GameMap_QuickMapRestingGrounds;
            On.GameMap.QuickMapWaterways += GameMap_QuickMapWaterways;
            On.GameMap.CloseQuickMap += GameMap_CloseQuickMap;
            On.HeroController.UnPause += HeroController_UnPause;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += HandleSceneChanges;
            ItemChanger.Events.OnBeginSceneTransition += Events_OnBeginSceneTransition;

            GUIController.Setup();
        }

        public void OnQuitToMenu()
        {
            On.GameMap.Start -= GameMap_Start;
            On.GameManager.SetGameMap -= GameManager_SetGameMap;
            On.GameMap.WorldMap -= GameMap_WorldMap;
            On.GameMap.QuickMapAncientBasin -= GameMap_QuickMapAncientBasin;
            On.GameMap.QuickMapCity -= GameMap_QuickMapCity;
            On.GameMap.QuickMapCliffs -= GameMap_QuickMapCliffs;
            On.GameMap.QuickMapCrossroads -= GameMap_QuickMapCrossroads;
            On.GameMap.QuickMapCrystalPeak -= GameMap_QuickMapCrystalPeak;
            On.GameMap.QuickMapDeepnest -= GameMap_QuickMapDeepnest;
            On.GameMap.QuickMapDirtmouth -= GameMap_QuickMapDirtmouth;
            On.GameMap.QuickMapFogCanyon -= GameMap_QuickMapFogCanyon;
            On.GameMap.QuickMapFungalWastes -= GameMap_QuickMapFungalWastes;
            On.GameMap.QuickMapGreenpath -= GameMap_QuickMapGreenpath;
            On.GameMap.QuickMapKingdomsEdge -= GameMap_QuickMapKingdomsEdge;
            On.GameMap.QuickMapQueensGardens -= GameMap_QuickMapQueensGardens;
            On.GameMap.QuickMapRestingGrounds -= GameMap_QuickMapRestingGrounds;
            On.GameMap.QuickMapWaterways -= GameMap_QuickMapWaterways;
            On.GameMap.CloseQuickMap -= GameMap_CloseQuickMap;
            On.HeroController.UnPause -= HeroController_UnPause;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= HandleSceneChanges;
            ItemChanger.Events.OnBeginSceneTransition -= Events_OnBeginSceneTransition;

            //GUIController.Unload();

            //PauseMenu.Destroy();
            //MapText.Destroy();
            //ControlPanel.Destroy();
            //MapKey.Destroy();
            //InfoPanels.Destroy();
            //TransitionPersistent.Destroy();
            //TransitionWorldMap.Destroy();
            //TransitionQuickMap.Destroy();
            //Benchwarp.Destroy();
        }

        private static void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
        {
            orig(self);
            
            GUIController.Instance.StartScripts();
        }

        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, UnityEngine.GameObject go_gameMap)
        {
            orig(self, go_gameMap);

            //PauseMenu.Build();
            //MapText.Build();
            ControlPanel.Build();
            //MapKey.Build();
            //InfoPanels.Build();
            //TransitionPersistent.Build();
            //TransitionWorldMap.Build();
            //TransitionQuickMap.Build();
            //Benchwarp.Build();
        }

        private static void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);

            worldMapOpen = true;
            //TransitionPersistent.UpdateRoute();
            //InfoPanels.UpdateAll();
            //Benchwarp.UpdateAll();
        }

        public static void OnOpenQuickMap()
        {
            worldMapOpen = false;
            quickMapOpen = true;
            //TransitionPersistent.UpdateRoute();
            //TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_CloseQuickMap(On.GameMap.orig_CloseQuickMap orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = false;
            lockToggleEnable = false;

            //MapText.UpdateAll();
            //TransitionPersistent.UpdateAll();
            //Benchwarp.attackHoldTimer.Reset();
        }
    }
}