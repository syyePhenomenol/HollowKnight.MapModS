using RandomizerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapModS.Map
{
    internal static class Transition
    {
        enum RoomState
        {
            Default,
            Current,
            Adjacent,
            OutOfLogic
        }

        readonly static Dictionary<RoomState, Vector4> roomColor = new()
        {
            { RoomState.Default, new(255, 255, 255, 0.3f) }, // white
            { RoomState.Current, new(0, 255, 0, 0.4f) }, // green
            { RoomState.Adjacent, new(0, 255, 255, 0.4f) }, // cyan
            { RoomState.OutOfLogic, new(255, 0, 0, 0.3f) } // red
        };

        private static string GetAdjacentScene(RandomizerMod.RandomizerData.TransitionDef transitionDef)
        {
            foreach (TransitionPlacement tp in RandomizerMod.RandomizerMod.RS.Context.transitionPlacements)
            {
                if (tp.target == null) return null;

                if (tp.source.Name == transitionDef.Name)
                {
                    return RandomizerMod.RandomizerData.Data.GetTransitionDef(tp.target.Name).SceneName;
                }
            }

            if (transitionDef.VanillaTarget == null) return null;

            // If it's not in TransitionPlacements, it's the vanilla target
            return RandomizerMod.RandomizerData.Data.GetTransitionDef(transitionDef.VanillaTarget).SceneName;
        }

        private static string DropSuffix(string scene)
        {
            if (scene == "") return "";

            string[] sceneSplit = scene.Split('_');

            string truncatedScene = sceneSplit[0];

            for (int i = 1; i < sceneSplit.Length - 1; i++)
            {
                truncatedScene += "_" + sceneSplit[i];
            }

            return truncatedScene;
        }

        private static void SetActiveColor(Transform transform, bool active, SpriteRenderer sr, Vector4 color)
        {
            if (sr == null)
            {
                transform.gameObject.SetActive(false);
                return;
            }

            transform.gameObject.SetActive(active);
            sr.color = color;
        }

        public static HashSet<string> SetupMapTransitionMode(GameMap gameMap)
        {
            HashSet<string> inLogicScenes = new();

            HashSet<string> outOfLogicScenes = new();

            HashSet<string> visitedAdjacentScenes = new();

            HashSet<string> uncheckedReachableScenes = new();
            

            RandomizerCore.Logic.ProgressionManager pm = RandomizerMod.RandomizerMod.RS.TrackerData.pm;

            // Get the scenes that are visited or connected in vanilla fashion to other visited scenes
            foreach (KeyValuePair<string, RandomizerCore.Logic.LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
            {
                RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(transitionEntry.Key);

                if (pm.Has(transitionEntry.Value.term.Id))
                {
                    inLogicScenes.Add(transitionDef.SceneName);
                    continue;
                }

                if (PlayerData.instance.scenesVisited.Contains(transitionDef.SceneName))
                {
                    outOfLogicScenes.Add(transitionDef.SceneName);
                }
            }

            // Get the scenes adjacent to the current scene that are in logic
            foreach (KeyValuePair<string, RandomizerCore.Logic.LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
            {
                if (pm.Has(transitionEntry.Value.term.Id))
                {
                    RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(transitionEntry.Key);

                    if (transitionDef.SceneName == GameManager.instance.sceneName)
                    {
                        string adjacentScene = GetAdjacentScene(transitionDef);

                        if (adjacentScene == null || !inLogicScenes.Contains(adjacentScene)) continue;

                        visitedAdjacentScenes.Add(adjacentScene);
                    }
                }
            }

            // Get scenes where there are unchecked reachable transitions
            foreach (string sourceTransition in RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableTransitions)
            {
                uncheckedReachableScenes.Add(RandomizerMod.RandomizerData.Data.GetTransitionDef(sourceTransition).SceneName);
            }

            // Show rooms with custom colors
            foreach (Transform areaObj in gameMap.transform)
            {
                if (areaObj.name == "Grub Pins"
                        || areaObj.name == "Dream_Gate_Pin"
                        || areaObj.name == "Compass Icon"
                        || areaObj.name == "Shade Pos"
                        || areaObj.name == "Flame Pins"
                        || areaObj.name == "Dreamer Pins"
                        || areaObj.name == "Map Markers") continue;

                foreach (Transform roomObj in areaObj.transform)
                {
                    ExtraMapData emd = roomObj.GetComponent<ExtraMapData>();

                    if (emd == null)
                    {
                        roomObj.gameObject.SetActive(false);
                        continue;
                    }

                    SpriteRenderer sr = roomObj.GetComponent<SpriteRenderer>();

                    // For AdditionalMaps room objects, the child has the SR
                    if (emd.sceneName.Contains("White_Palace"))
                    {
                        foreach (Transform roomObj2 in roomObj.transform)
                        {
                            if (!roomObj2.name.Contains("RWP")) continue;
                            sr = roomObj2.GetComponent<SpriteRenderer>();
                            break;
                        }
                    }

                    bool active = false;
                    Vector4 color = roomColor[RoomState.Default];

                    if (outOfLogicScenes.Contains(emd.sceneName))
                    {
                        color = roomColor[RoomState.OutOfLogic];
                        active = true;
                    }

                    if (inLogicScenes.Contains(emd.sceneName))
                    {
                        color = roomColor[RoomState.Default];
                        active = true;
                    }

                    if (emd.sceneName == GameManager.instance.sceneName)
                    {
                        color = roomColor[RoomState.Current];
                    }

                    if (visitedAdjacentScenes.Contains(emd.sceneName))
                    {
                        color = roomColor[RoomState.Adjacent];
                    }
                    
                    if (uncheckedReachableScenes.Contains(emd.sceneName))
                    {
                        color.w = 1f;
                    }

                    SetActiveColor(roomObj, active, sr, color);

                    if (!active) continue;

                    // Force disable sub area names
                    foreach (Transform roomObj2 in roomObj.transform)
                    {
                        if (roomObj2.name.Contains("Area Name"))
                        {
                            roomObj2.gameObject.SetActive(false);
                        }
                    }
                }
            }

            return new(inLogicScenes.Union(outOfLogicScenes));
        }

        private static string GetActualSceneName(string objName)
        {
            // Some room objects have non-standard scene names, so we truncate the name
            // in these situations
            if (RandomizerMod.RandomizerData.Data.IsRoom(objName))
            {
                return objName;
            }

            if (RandomizerMod.RandomizerData.Data.IsRoom(DropSuffix(objName)))
            {
                return DropSuffix(objName);
            }

            return null;
        }

        public class ExtraMapData : MonoBehaviour
        {
            public Color origColor;
            public Color origTransitionColor;
            public string sceneName;
        }

        // Store original color, also store the sceneName for the room object for convenience
        public static void AddExtraComponentsToMap(GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    string sceneName = GetActualSceneName(roomObj.name);

                    if (sceneName == null) continue;

                    ExtraMapData extraData = roomObj.GetComponent<ExtraMapData>();
                    SpriteRenderer SR = roomObj.GetComponent<SpriteRenderer>();

                    if (SR == null) continue;

                    if (extraData == null)
                    {
                        extraData = roomObj.gameObject.AddComponent<ExtraMapData>();
                        extraData.origColor = SR.color;
                        extraData.sceneName = sceneName;
                    }
                }
            }
        }

        public static void ResetMapColors(GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    ExtraMapData extra = roomObj.GetComponent<ExtraMapData>();
                    SpriteRenderer sr = roomObj.GetComponent<SpriteRenderer>();

                    if (sr == null || extra == null) continue;

                    if (roomObj.name.Contains("White_Palace"))
                    {
                        foreach (Transform roomObj2 in roomObj.transform)
                        {
                            if (!roomObj2.name.Contains("RWP")) continue;
                            sr = roomObj2.GetComponent<SpriteRenderer>();
                            break;
                        }
                    }

                    sr.color = extra.origColor;
                }
            }
        }
    }
}