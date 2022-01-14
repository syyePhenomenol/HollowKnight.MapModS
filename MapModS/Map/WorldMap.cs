using GlobalEnums;
using MapModS.Data;
using MapModS.Settings;
using MapModS.Trackers;
using RandomizerCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapModS.Map
{
    public static class WorldMap
    {
        public static GameObject goCustomPins = null;
        public static PinsCustom CustomPins => goCustomPins?.GetComponent<PinsCustom>();

        public static void Hook()
        {
            On.GameManager.SetGameMap += GameManager_SetGameMap;
            On.GameMap.WorldMap += GameMap_WorldMap;
            On.GameMap.SetupMapMarkers += GameMap_SetupMapMarkers;
            On.GameMap.DisableMarkers += GameMap_DisableMarkers;
            On.GameManager.UpdateGameMap += GameManager_UpdateGameMap;
        }

        // The function that is called every time after a new GameMap is created (once per save load)
        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject go_gameMap)
        {
            orig(self, go_gameMap);

            GameMap gameMap = go_gameMap.GetComponent<GameMap>();

            DataLoader.FindPoolGroups();

            StoreOrigMapColors(gameMap);

            if (RandomizerMod.RandomizerMod.RS.GenerationSettings.TransitionSettings.Mode != RandomizerMod.Settings.TransitionSettings.TransitionMode.None)
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

            CustomPins.FindRandomizedGroups();

            MapModS.Instance.Log("Adding Custom Pins done.");
        }

        // Called every time we open the World Map
        private static void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);

            // Easiest way to force AdditionalMaps custom areas to show
            if (MapModS.LS.ModEnabled && (MapModS.LS.mapMode == MapMode.FullMap || MapModS.LS.mapMode == MapMode.TransitionRando))
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

            if (!MapModS.LS.ModEnabled) return;

            if (goCustomPins == null) return;

            CustomPins.gameObject.SetActive(true);
        }

        private static void GameMap_DisableMarkers(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            if (goCustomPins != null)
            {
                CustomPins.gameObject.SetActive(false);
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
                    && MapModS.LS.ModEnabled && MapModS.LS.mapMode == MapMode.TransitionRando)
                {
                    //gameMap.SetupMap();
                    transitionPinScenes = SetupMapTransitionRando(gameMap);
                }
                else
                {
                    ResetMapColors(gameMap);
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

        private static HashSet<string> SetupMapTransitionRando(GameMap gameMap)
        {
            HashSet<string> adjacentReachableScenes = new();
            HashSet<string> unreachedReachableScenes = new();

            foreach (TransitionPlacement tp in RandomizerMod.RandomizerMod.RS.Context.transitionPlacements)
            {
                string tpSourceClean = tp.source.Name.Split('[')[0];
                string tpTargetClean = tp.target.Name.Split('[')[0];

                if (RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableTransitions.Contains(tp.source.Name))
                {
                    if (tpSourceClean == GameManager.instance.sceneName)
                    {
                        adjacentReachableScenes.Add(tpTargetClean);
                        continue;
                    }

                    unreachedReachableScenes.Add(tpTargetClean);
                }

                if (PlayerData.instance.scenesVisited.Contains(tpTargetClean) && tpSourceClean == GameManager.instance.sceneName)
                {
                    adjacentReachableScenes.Add(tpTargetClean);
                }
            }

            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (roomObj.name == GameManager.instance.sceneName)
                    {
                        roomObj.gameObject.SetActive(true);
                        roomObj.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                    else if (adjacentReachableScenes.Contains(roomObj.name))
                    {
                        roomObj.gameObject.SetActive(true);
                        roomObj.GetComponent<SpriteRenderer>().color = Color.cyan;
                    }
                    else if (unreachedReachableScenes.Contains(roomObj.name))
                    {
                        roomObj.gameObject.SetActive(true);
                        roomObj.GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                    else if (PlayerData.instance.scenesVisited.Contains(roomObj.name))
                    {
                        roomObj.gameObject.SetActive(true);
                        roomObj.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                    else
                    {
                        roomObj.gameObject.SetActive(false);
                    }
                }
            }

            unreachedReachableScenes.Add(GameManager.instance.sceneName);

            return unreachedReachableScenes;
        }

        private class ColorCopy : MonoBehaviour
        {
            public Color origColor;
        }

        private static void StoreOrigMapColors(GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    ColorCopy colorCopy = roomObj.GetComponent<ColorCopy>();
                    SpriteRenderer SR = roomObj.GetComponent<SpriteRenderer>();

                    if (SR == null) continue;

                    if (colorCopy == null)
                    {
                        colorCopy = roomObj.gameObject.AddComponent<ColorCopy>();
                        colorCopy.origColor = SR.color;
                    }
                }
            }
        }

        private static void ResetMapColors(GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    ColorCopy colorCopy = roomObj.GetComponent<ColorCopy>();
                    SpriteRenderer SR = roomObj.GetComponent<SpriteRenderer>();

                    if (SR == null || colorCopy == null) continue;

                    SR.color = colorCopy.origColor;
                }
            }
        }
    }
}