using RandomizerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapModS.Map
{
    internal static class Transition
    {
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

            if (sceneSplit.Length < 2) return "";

            return sceneSplit[0] + "_" + sceneSplit[1];
        }

        private static string GetTruncatedScene(string roomName)
        {
            // Some room objects have non-standard scene names, so we truncate the name
            // in these situations
            if (RandomizerMod.RandomizerData.Data.IsRoom(roomName))
            {
                return roomName;
            }

            if (RandomizerMod.RandomizerData.Data.IsRoom(DropSuffix(roomName)))
            {
                return DropSuffix(roomName);
            }

            return roomName;
        }

        private static void SetActiveColor(Transform transform, SpriteRenderer sr, Color color)
        {
            if (sr == null) return;

            transform.gameObject.SetActive(true);
            sr.color = color;
        }

        public static HashSet<string> SetupMapTransitionMode(GameMap gameMap)
        {
            HashSet<string> inLogicScenes = new();

            HashSet<string> visitedAdjacentScenes = new();

            HashSet<string> uncheckedReachableScenes = new();

            RandomizerCore.Logic.ProgressionManager pm = RandomizerMod.RandomizerMod.RS.TrackerData.pm;

            // Get the scenes that are visited or connected in vanilla fashion to other visited scenes
            foreach (KeyValuePair<string, RandomizerCore.Logic.LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
            {
                if (pm.Has(transitionEntry.Value.term.Id))
                {
                    RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(transitionEntry.Key);
                    inLogicScenes.Add(transitionDef.SceneName);
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
                foreach (Transform roomObj in areaObj.transform)
                {
                    string roomName = GetTruncatedScene(roomObj.name);

                    SpriteRenderer sr = roomObj.GetComponent<SpriteRenderer>();

                    if (roomName.Contains("White_Palace"))
                    {
                        foreach (Transform roomObj2 in roomObj.transform)
                        {
                            if (!roomObj2.name.Contains("RWP")) continue;
                            sr = roomObj2.GetComponent<SpriteRenderer>();
                            break;
                        }
                    }

                    if (visitedAdjacentScenes.Contains(roomName))
                    {
                        SetActiveColor(roomObj, sr, Color.blue);
                    }
                    else if (uncheckedReachableScenes.Contains(roomName))
                    {
                        SetActiveColor(roomObj, sr, Color.cyan);
                    }
                    else if (roomName == GameManager.instance.sceneName)
                    {
                        SetActiveColor(roomObj, sr, Color.green);
                    }
                    else if (inLogicScenes.Contains(roomName))
                    {
                        SetActiveColor(roomObj, sr, Color.white);
                    }
                    else
                    {
                        roomObj.gameObject.SetActive(false);
                    }
                }
            }

            return inLogicScenes;
        }

        private class ColorCopy : MonoBehaviour
        {
            public Color origColor;
        }

        public static void StoreOrigMapColors(GameMap gameMap)
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

        public static void ResetMapColors(GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    ColorCopy colorCopy = roomObj.GetComponent<ColorCopy>();
                    SpriteRenderer sr = roomObj.GetComponent<SpriteRenderer>();

                    if (sr == null || colorCopy == null) continue;

                    if (roomObj.name.Contains("White_Palace"))
                    {
                        foreach (Transform roomObj2 in roomObj.transform)
                        {
                            if (!roomObj2.name.Contains("RWP")) continue;
                            sr = roomObj2.GetComponent<SpriteRenderer>();
                            break;
                        }
                    }

                    sr.color = colorCopy.origColor;
                }
            }
        }
    }
}
