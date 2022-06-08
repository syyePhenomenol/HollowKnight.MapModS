using ItemChanger;
using UnityEngine.SceneManagement;

namespace MapModS.UI
{
    public static class GUI
    {
        public static bool worldMapOpen = false;
        public static bool quickMapOpen = false;
        public static bool lockToggleEnable = false;

        public static void Hook()
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
            Events.OnBeginSceneTransition += Events_OnBeginSceneTransition;

            GUIController.Setup();
        }

        public static void Unhook()
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
            Events.OnBeginSceneTransition -= Events_OnBeginSceneTransition;

            GUIController.Unload();

            PauseMenu.Destroy();
            MapText.Destroy();
            ControlPanel.Destroy();
            MapKey.Destroy();
            LookupText.Destroy();
            TransitionPersistent.Destroy();
            TransitionWorldMap.Destroy();
            TransitionQuickMap.Destroy();
        }

        private static void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
        {
            orig(self);
            
            GUIController.Instance.StartScripts();
        }

        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, UnityEngine.GameObject go_gameMap)
        {
            orig(self, go_gameMap);

            PauseMenu.Build();
            MapText.Build();
            ControlPanel.Build();
            MapKey.Build();
            LookupText.Build();
            TransitionPersistent.Build();
            TransitionWorldMap.Build();
            TransitionQuickMap.Build();
        }

        private static void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);

            worldMapOpen = true;
        }

        private static void GameMap_QuickMapAncientBasin(On.GameMap.orig_QuickMapAncientBasin orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapCity(On.GameMap.orig_QuickMapCity orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapCliffs(On.GameMap.orig_QuickMapCliffs orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapCrossroads(On.GameMap.orig_QuickMapCrossroads orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapCrystalPeak(On.GameMap.orig_QuickMapCrystalPeak orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapDeepnest(On.GameMap.orig_QuickMapDeepnest orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapDirtmouth(On.GameMap.orig_QuickMapDirtmouth orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapFogCanyon(On.GameMap.orig_QuickMapFogCanyon orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapFungalWastes(On.GameMap.orig_QuickMapFungalWastes orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapGreenpath(On.GameMap.orig_QuickMapGreenpath orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapKingdomsEdge(On.GameMap.orig_QuickMapKingdomsEdge orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapQueensGardens(On.GameMap.orig_QuickMapQueensGardens orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapRestingGrounds(On.GameMap.orig_QuickMapRestingGrounds orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_QuickMapWaterways(On.GameMap.orig_QuickMapWaterways orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = true;
            TransitionQuickMap.UpdateAll();
        }

        private static void GameMap_CloseQuickMap(On.GameMap.orig_CloseQuickMap orig, GameMap self)
        {
            orig(self);
            worldMapOpen = false;
            quickMapOpen = false;
            lockToggleEnable = false;

            MapText.UpdateAll();
            TransitionPersistent.UpdateAll();
        }


        private static void HeroController_UnPause(On.HeroController.orig_UnPause orig, HeroController self)
        {
            orig(self);
            PauseMenu.CollapsePanel();
        }

        private static void HandleSceneChanges(Scene from, Scene to)
        {
            if (GameManager.instance.sceneName != to.name) return;

            RouteCompass.CreateRouteCompass();
            RouteCompass.UpdateCompass();
        }

        private static void Events_OnBeginSceneTransition(Transition obj)
        {
            TransitionPersistent.UpdateRoute(obj);

            MapModS.Instance.Log(obj.ToString());
        }
    }
}