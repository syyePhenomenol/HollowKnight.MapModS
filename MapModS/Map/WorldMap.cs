using GlobalEnums;
using MapModS.Data;
using MapModS.Settings;
using MapModS.Trackers;
using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapModS.Map
{
    public static class WorldMap
    {
        public static GameObject goCustomPins = null;
        public static PinsCustom CustomPins => goCustomPins.GetComponent<PinsCustom>();

        public static GameObject goExtraRooms = null;

        public static void Hook()
        {
            On.GameMap.Start += GameMap_Start;
            On.GameManager.SetGameMap += GameManager_SetGameMap;
            On.GameMap.WorldMap += GameMap_WorldMap;
            On.GameMap.SetupMapMarkers += GameMap_SetupMapMarkers;
            On.GameMap.DisableMarkers += GameMap_DisableMarkers;
            On.GameManager.UpdateGameMap += GameManager_UpdateGameMap;
            ModHooks.LanguageGetHook += OnLanguageGetHook;
        }

        public static void Unhook()
        {
            On.GameMap.Start -= GameMap_Start;
            On.GameManager.SetGameMap -= GameManager_SetGameMap;
            On.GameMap.WorldMap -= GameMap_WorldMap;
            On.GameMap.SetupMapMarkers -= GameMap_SetupMapMarkers;
            On.GameMap.DisableMarkers -= GameMap_DisableMarkers;
            On.GameManager.UpdateGameMap -= GameManager_UpdateGameMap;
            ModHooks.LanguageGetHook -= OnLanguageGetHook;
        }

        // Called every time when a new GameMap is created (once per save load)
        private static void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
        {
            orig(self);

            try
            {
                Dependencies.BenchwarpInterop();
                DataLoader.SetUsedPinDefs();
                DataLoader.SetLogicLookup();

                if (SettingsUtil.IsTransitionRando())
                {
                    DataLoader.SetTransitionLookup();
                }

                if (MapModS.LS.NewSettings || MapModS.LS.PoolGroupSettings.Count == 0)
                {
                    MapModS.LS.InitializePoolGroupSettings();
                    MapModS.LS.NewSettings = true;
                }
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(e);
            }
        }

        // Called every time after a new GameMap is created (once per save load)
        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject go_gameMap)
        {
            orig(self, go_gameMap);
            
            GameMap gameMap = go_gameMap.GetComponent<GameMap>();

            Transition.AddExtraComponentsToMap(gameMap);

            if (SettingsUtil.IsTransitionRando())
            {
                if (GameObject.Find("MMS Custom Map Rooms") == null)
                {
                    goExtraRooms = Transition.CreateExtraMapRooms(gameMap);
                }

                if (MapModS.LS.NewSettings)
                {
                    MapModS.LS.mapMode = MapMode.TransitionRando;
                }
            }

            if (goCustomPins != null)
            {
                goCustomPins.GetComponent<PinsCustom>().DestroyPins();
                UnityEngine.Object.Destroy(goCustomPins);
            }

            MapModS.Instance.Log("Adding Custom Pins...");

            goCustomPins = new GameObject($"MMS Custom Pin Group");
            goCustomPins.AddComponent<PinsCustom>();

            // Setting parent here is only for controlling local position,
            // not active/not active (need separate mechanism)
            goCustomPins.transform.SetParent(go_gameMap.transform);

            CustomPins.MakePins(gameMap);

            CustomPins.GetRandomizedOthersGroups();

            if (MapModS.LS.NewSettings)
            {
                CustomPins.ResetPoolSettings();
            }

            CustomPins.UpdatePins(MapZone.NONE, new());

            MapModS.Instance.Log("Adding Custom Pins done.");

            MapModS.LS.NewSettings = false;
        }

        // Called every time we open the World Map
        private static void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);

            // Easiest way to force AdditionalMaps custom areas to show
            if (MapModS.LS.ModEnabled
                && (MapModS.LS.mapMode == MapMode.FullMap
                    || MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt))
            {
                foreach (Transform child in self.transform)
                {
                    if (child.name == "WHITE_PALACE"
                        || child.name == "GODS_GLORY")
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }

            try
            {
                UpdateMap(self, MapZone.NONE);
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(e);
            }
        }

        // Following two behaviours necessary since GameMap is actually persistently active
        private static void GameMap_SetupMapMarkers(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            orig(self);

            if (!MapModS.LS.ModEnabled || goCustomPins == null) return;

            goCustomPins.SetActive(true);

            // For debugging purposes
            //if (goExtraRooms != null)
            //{
            //    goExtraRooms.SetActive(true);
            //}
        }

        private static void GameMap_DisableMarkers(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            if (goCustomPins != null)
            {
                goCustomPins.SetActive(false);
            }

            if (goExtraRooms != null)
            {
                goExtraRooms.SetActive(false);
            }

            orig(self);
        }

        // Remove the "Map Updated" idle animation, since it occurs when the return value is true
        public static bool GameManager_UpdateGameMap(On.GameManager.orig_UpdateGameMap orig, GameManager self)
        {
            orig(self);

            return false;
        }

        private static string OnLanguageGetHook(string key, string sheet, string orig)
        {
            if (sheet == "MMS" && (Transition.nonMapScenes.Contains(key) || Transition.whitePalaceScenes.Contains(key)))
            {
                return key;
            }

            return orig;
        }

        // The main method for updating map objects and pins when opening either World Map or Quick Map
        public static void UpdateMap(GameMap gameMap, MapZone mapZone)
        {
            ItemTracker.UpdateObtainedItems();

            HashSet<string> transitionPinScenes = new();

            FullMap.PurgeMap();

            if (SettingsUtil.IsTransitionRando()
                && MapModS.LS.ModEnabled
                && (MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt))
            {
                    transitionPinScenes = Transition.SetupMapTransitionMode(gameMap);
            }
            else
            {
                gameMap.SetupMap();
            }

            if (goCustomPins == null || !MapModS.LS.ModEnabled) return;

            gameMap.panMinX = -29f;
            gameMap.panMaxX = 26f;
            gameMap.panMinY = -25f;
            gameMap.panMaxY = 20f;

            PinsVanilla.ForceDisablePins(gameMap.gameObject);

            CustomPins.UpdatePins(mapZone, transitionPinScenes);
            CustomPins.ResizePins("None selected");
            CustomPins.SetPinsActive();
            CustomPins.SetSprites();
        }
    }
}