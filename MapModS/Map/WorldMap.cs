using GlobalEnums;
using MapModS.Data;
using MapModS.Settings;
using MapModS.Trackers;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
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
            On.GameMap.Update += GameMap_Update;
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
            On.GameMap.Update -= GameMap_Update;
        }

        // Called every time when a new GameMap is created (once per save load)
        private static void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
        {
            orig(self);

            try
            {
                Dependencies.BenchwarpVersionCheck();
                MainData.SetUsedPinDefs();
                MainData.SetLogicLookup();
                TransitionData.SetTransitionLookup();
                PathfinderData.Load();
                Pathfinder.Initialize();
                Pathfinder.UpdateProgression();

                if (MapModS.LS.newSettings || MapModS.LS.poolGroupSettings.Count == 0)
                {
                    MapModS.LS.InitializePoolGroupSettings();
                    MapModS.LS.newSettings = true;
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

            MapRooms.AddExtraComponentsToMap(gameMap);

            if (GameObject.Find("MMS Custom Map Rooms") == null)
            {
                goExtraRooms = MapRooms.CreateExtraMapRooms(gameMap);
            }

            if (TransitionData.IsTransitionRando() && MapModS.LS.newSettings)
            {
                MapModS.LS.mapMode = MapMode.TransitionRando;
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

            if (MapModS.LS.newSettings)
            {
                CustomPins.ResetPoolSettings();
            }

            CustomPins.UpdatePins(MapZone.NONE, new());

            MapModS.Instance.Log("Adding Custom Pins done.");

            MapModS.LS.newSettings = false;
        }

        // Called every time we open the World Map
        private static void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);

            // Easiest way to force AdditionalMaps custom areas to show
            if (MapModS.LS.modEnabled
                && (MapModS.LS.mapMode == MapMode.FullMap
                    || MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt))
            {
                foreach (Transform roomObj in self.transform.Cast<Transform>().Where(t => t.name == "WHITE_PALACE" || t.name == "GODS_GLORY"))
                {
                    roomObj.gameObject.SetActive(true);

                    // Force enable sub area names
                    foreach (Transform roomObj2 in roomObj.transform.Cast<Transform>())
                    {
                        if (roomObj2.name.Contains("Area Name"))
                        {
                            roomObj2.gameObject.SetActive(true);
                        }

                        foreach (Transform roomObj3 in roomObj2.transform.Cast<Transform>()
                            .Where(r => r.name.Contains("Area Name")))
                        {
                            roomObj3.gameObject.SetActive(true);
                        }
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

            if (!MapModS.LS.modEnabled || goCustomPins == null) return;

            goCustomPins.SetActive(true);
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
            if (sheet == "MMS" && MainData.IsNonMappedScene(key))
            {
                return key;
            }

            return orig;
        }

        // Zoom faster on keyboard by holding down shift key, similarly to right analog stick on controller
        private static void GameMap_Update(On.GameMap.orig_Update orig, GameMap self)
        {
            if (MapModS.LS.modEnabled
                && self.canPan
                && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (InputHandler.Instance.inputActions.down.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y + self.panSpeed * Time.deltaTime, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.up.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y - self.panSpeed * Time.deltaTime, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.left.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x + self.panSpeed * Time.deltaTime, self.transform.position.y, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.right.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x - self.panSpeed * Time.deltaTime, self.transform.position.y, self.transform.position.z);
                }
            }

            orig(self);
        }

        // The main method for updating map objects and pins when opening either World Map or Quick Map
        public static void UpdateMap(GameMap gameMap, MapZone mapZone)
        {
            ItemTracker.UpdateObtainedItems();

            HashSet<string> transitionPinScenes = new();

            if (TransitionData.TransitionModeActive())
            {
                Pathfinder.UpdateProgression();
                transitionPinScenes = MapRooms.SetupMapTransitionMode(gameMap, mapZone);
            }
            else
            {
                FullMap.PurgeMap();
                gameMap.SetupMap();
                MapRooms.ResetMapColors(gameMap.gameObject);
            }

            if (goCustomPins == null || !MapModS.LS.modEnabled) return;

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