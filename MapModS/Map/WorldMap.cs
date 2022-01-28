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
        public static PinsCustom CustomPins => goCustomPins?.GetComponent<PinsCustom>();

        public static GameObject goExtraRooms = null;

        public static void Hook()
        {
            On.GameManager.SetGameMap += GameManager_SetGameMap;
            On.GameMap.WorldMap += GameMap_WorldMap;
            On.GameMap.SetupMapMarkers += GameMap_SetupMapMarkers;
            On.GameMap.DisableMarkers += GameMap_DisableMarkers;
            On.GameManager.UpdateGameMap += GameManager_UpdateGameMap;
            ModHooks.LanguageGetHook += OnLanguageGetHook;
        }

        public static void Unhook()
        {
            On.GameManager.SetGameMap -= GameManager_SetGameMap;
            On.GameMap.WorldMap -= GameMap_WorldMap;
            On.GameMap.SetupMapMarkers -= GameMap_SetupMapMarkers;
            On.GameMap.DisableMarkers -= GameMap_DisableMarkers;
            On.GameManager.UpdateGameMap -= GameManager_UpdateGameMap;
            ModHooks.LanguageGetHook -= OnLanguageGetHook;
        }

        private static string OnLanguageGetHook(string key, string sheet, string orig)
        {
            if (sheet == "MMS" && Transition.nonMapScenes.Contains(key))
            {
                return key;
            }

            return orig;
        }

        // The function that is called every time after a new GameMap is created (once per save load)
        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject go_gameMap)
        {
            orig(self, go_gameMap);

            GameMap gameMap = go_gameMap.GetComponent<GameMap>();

            DataLoader.FindPoolGroups();

            Transition.AddExtraComponentsToMap(gameMap);

            if (RandomizerMod.RandomizerMod.RS.GenerationSettings.TransitionSettings.Mode != RandomizerMod.Settings.TransitionSettings.TransitionMode.None)
            {
                goExtraRooms = Transition.CreateExtraMapRooms(gameMap);

                if (MapModS.LS.NewSettings)
                {
                    MapModS.LS.mapMode = MapMode.TransitionRando;
                    MapModS.LS.NewSettings = false;
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

            CustomPins.FindRandomizedGroups();

            MapModS.Instance.Log("Adding Custom Pins done.");
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

            UpdateMap(self, MapZone.NONE);
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

        // The main method for updating map objects and pins when opening either World Map or Quick Map
        public static void UpdateMap(GameMap gameMap, MapZone mapZone)
        {
            try
            {
                ItemTracker.UpdateObtainedItems();

                HashSet<string> transitionPinScenes = new();

                FullMap.PurgeMap();

                if (RandomizerMod.RandomizerMod.RS.GenerationSettings.TransitionSettings.Mode != RandomizerMod.Settings.TransitionSettings.TransitionMode.None
                    && MapModS.LS.ModEnabled)
                {
                    if (MapModS.LS.mapMode == MapMode.TransitionRando)
                    {
                        transitionPinScenes = Transition.SetupMapTransitionMode(gameMap, false);
                    }
                    else if (MapModS.LS.mapMode == MapMode.TransitionRandoAlt)
                    {
                        transitionPinScenes = Transition.SetupMapTransitionMode(gameMap, true);
                    }
                    else
                    {
                        Transition.ResetMapColors(gameMap);
                        gameMap.SetupMap();
                    }
                }
                else
                {
                    Transition.ResetMapColors(gameMap);
                    gameMap.SetupMap();
                }

                PinsVanilla.ForceDisablePins(gameMap.gameObject);

                if (goCustomPins == null || !MapModS.LS.ModEnabled) return;

                CustomPins.ResizePins();
                CustomPins.UpdatePins(mapZone, transitionPinScenes);
                CustomPins.RefreshGroups();
                CustomPins.RefreshSprites();
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(e);
            }
        }

        
    }
}