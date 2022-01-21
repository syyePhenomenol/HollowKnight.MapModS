using MapModS.CanvasUtil;
using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MapModS.UI
{
    internal class TransitionText
    {
        public static GameObject Canvas;

        public static bool LockToggleEnable;

        private static CanvasPanel _instructionPanel;
        private static CanvasPanel _transitionPanel;

        public static TransitionHelper th;

        public static string lastStartScene = null;
        public static string lastFinalScene = null;
        public static string selectedScene = "None";
        public static List<string> selectedRoute = new();
        public static HashSet<string> rejectedTransitions = new();

        public static void Show()
        {
            if (Canvas == null) return;

            Canvas.SetActive(true);
            LockToggleEnable = false;
            SetTexts();
        }

        public static void ShowWorldMap(GameMap gameMap)
        {
            bool isActive = !LockToggleEnable && MapModS.LS.ModEnabled
                && RandomizerMod.RandomizerMod.RS.GenerationSettings.TransitionSettings.Mode != RandomizerMod.Settings.TransitionSettings.TransitionMode.None
                && MapModS.LS.mapMode == MapMode.TransitionRando;

            _instructionPanel.SetActive(isActive, isActive);

            // The following stores the colors of the rooms prior to selection
            if (gameMap == null) return;

            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    Transition.ExtraMapData extra = roomObj.GetComponent<Transition.ExtraMapData>();

                    if (extra == null) continue;

                    SpriteRenderer sr = roomObj.GetComponent<SpriteRenderer>();

                    // For AdditionalMaps room objects, the child has the SR
                    if (extra.sceneName.Contains("White_Palace"))
                    {
                        foreach (Transform roomObj2 in roomObj.transform)
                        {
                            if (!roomObj2.name.Contains("RWP")) continue;
                            sr = roomObj2.GetComponent<SpriteRenderer>();
                            break;
                        }
                    }

                    if (sr == null) continue;

                    extra.origTransitionColor = sr.color;
                }
            }
        }

        public static void Hide()
        {
            if (Canvas == null || _instructionPanel == null) return;

            Canvas.SetActive(false);
            LockToggleEnable = false;

            _instructionPanel.SetActive(false, false);
        }

        public static void Initialize()
        {
            th = new();

            lastStartScene = null;
            lastFinalScene = null;
            selectedScene = "None";
            selectedRoute = new();
            rejectedTransitions = new();
        }

        public static void BuildText(GameObject _canvas)
        {
            Canvas = _canvas;
            _instructionPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 20f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _instructionPanel.AddText("Instructions", "None", new Vector2(20f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);
            _instructionPanel.AddText("Route", "", new Vector2(-37f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperRight);

            _instructionPanel.SetActive(false, false);

            _transitionPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 20f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _transitionPanel.AddText("Transitions", "Transitions: None", new Vector2(20f, 20f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);

            _transitionPanel.SetActive(false, false);

            SetTexts();
        }

        public static void SetTexts()
        {
            if (GameManager.instance.gameMap == null
                || _instructionPanel == null) return;

            bool isActive = !LockToggleEnable && MapModS.LS.ModEnabled
                && RandomizerMod.RandomizerMod.RS.GenerationSettings.TransitionSettings.Mode != RandomizerMod.Settings.TransitionSettings.TransitionMode.None
                && MapModS.LS.mapMode == MapMode.TransitionRando;

            _transitionPanel.SetActive(isActive, isActive);

            SetTransitionsText();
            SetRouteText();
        }

        private static int frameCounter = 0;

        // Called every frame
        public static void Update()
        {
            if (Canvas == null
                || !Canvas.activeSelf
                || !_instructionPanel.Active
                || !_transitionPanel.Active
                || HeroController.instance == null
                || GameManager.instance.IsGamePaused())
            {
                return;
            }

            // Use menu selection button for control
            if (InputHandler.Instance != null && InputHandler.Instance.inputActions.menuSubmit.WasPressed)
            {
                GetRoute();
            }

            frameCounter = (frameCounter + 1) % 24;

            if (frameCounter == 0)
            {
                if (GetRoomClosestToMiddle(selectedScene, out selectedScene))
                {
                    SetInstructionsText();
                    SetRoomColors();
                }
            }
        }

        private static double DistanceToMiddle(Transform transform)
        {
            return Math.Pow(transform.position.x, 2) + Math.Pow(transform.position.y, 2);
        }

        private static readonly Vector4 selectionColor = new(255, 255, 0, 0.5f);

        // This method also handles highlighting the selected room
        public static bool GetRoomClosestToMiddle(string previousScene, out string selectedScene)
        {
            selectedScene = null;
            double minDistance = double.PositiveInfinity;

            GameObject go_GameMap = GameManager.instance.gameMap;

            if (go_GameMap == null) return false;

            foreach (Transform areaObj in go_GameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (!roomObj.gameObject.activeSelf) continue;

                    Transition.ExtraMapData extra = roomObj.GetComponent<Transition.ExtraMapData>();

                    if (extra == null) continue;

                    double distance = DistanceToMiddle(roomObj);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        selectedScene = extra.sceneName;
                    }
                }
            }

            // True if the scene name has changed
            return selectedScene != previousScene;
        }

        private static void SetRoomColors()
        {
            GameObject go_GameMap = GameManager.instance.gameMap;

            if (go_GameMap == null) return;

            foreach (Transform areaObj in go_GameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (!roomObj.gameObject.activeSelf) continue;

                    Transition.ExtraMapData extra = roomObj.GetComponent<Transition.ExtraMapData>();

                    if (extra == null) continue;

                    SpriteRenderer sr = roomObj.GetComponent<SpriteRenderer>();

                    // For AdditionalMaps room objects, the child has the SR
                    if (extra.sceneName.Contains("White_Palace"))
                    {
                        foreach (Transform roomObj2 in roomObj.transform)
                        {
                            if (!roomObj2.name.Contains("RWP")) continue;
                            sr = roomObj2.GetComponent<SpriteRenderer>();
                            break;
                        }
                    }

                    if (sr == null) continue;

                    if (extra.sceneName == selectedScene)
                    {
                        sr.color = selectionColor;
                    }
                    else
                    {
                        sr.color = extra.origTransitionColor;
                    }
                }
            }
        }

        public static void SetInstructionsText()
        {
            string instructionsText = $"Selected room: {selectedScene}.";

            if (selectedScene == GameManager.instance.sceneName)
            {
                instructionsText += " You are here.";
            }
            else
            {
                instructionsText += $" Press [Menu Select] to find new route / switch starting transition for current route.";
            }

            _instructionPanel.GetText("Instructions").UpdateText(instructionsText);
        }

        public static void SetRouteText()
        {
            string routeText = "Current route: ";

            if (lastStartScene != null && lastFinalScene != null && selectedRoute.Any())
            {
                routeText += $"{lastStartScene} -> {lastFinalScene}      ";
                routeText += $"Transitions: {selectedRoute.Count()}";
            }
            else
            {
                routeText += "None";
            }

            _instructionPanel.GetText("Route").UpdateText(routeText);
        }

        public static void SetTransitionsText()
        {
            string transitionsText = "";

            if (selectedRoute.Any())
            {
                int maxDisplayedTransitions = 10;
                int displayedTransitionCounter = 0;

                foreach (string transition in selectedRoute)
                {
                    if (displayedTransitionCounter == maxDisplayedTransitions)
                    {
                        transitionsText += " -> ";
                        break;
                    }

                    transitionsText += " -> " + transition;
                    displayedTransitionCounter ++;
                }
            }

            _transitionPanel.GetText("Transitions").UpdateText(transitionsText);
        }

        public static void GetRoute()
        {
            if (_instructionPanel == null
                || _transitionPanel == null
                || !_instructionPanel.Active
                || !_transitionPanel.Active
                || HeroController.instance == null
                || GameManager.instance.IsGamePaused()
                || selectedScene == "None"
                || th == null)
            {
                return;
            }

            if (lastStartScene != GameManager.instance.sceneName || lastFinalScene != selectedScene)
            {
                rejectedTransitions = new();
            }

            selectedRoute = th.ShortestRoute(GameManager.instance.sceneName, selectedScene, rejectedTransitions);

            if (!selectedRoute.Any())
            {
                lastFinalScene = null;
                rejectedTransitions = new();
            }
            else
            {
                lastStartScene = GameManager.instance.sceneName;
                lastFinalScene = selectedScene;
                rejectedTransitions.Add(selectedRoute.First());
            }

            SetTexts();
        }

        public static void RemoveTraversedTransition(string previousScene, string currentScene)
        {
            if ((selectedRoute.Count >= 2
                && RandomizerMod.RandomizerData.Data.GetTransitionDef(selectedRoute.First()).SceneName == previousScene
                && RandomizerMod.RandomizerData.Data.GetTransitionDef(selectedRoute.ElementAt(1)).SceneName == currentScene)
                || (selectedRoute.Count == 1
                && RandomizerMod.RandomizerData.Data.GetTransitionDef(selectedRoute.First()).SceneName == previousScene
                && lastFinalScene == currentScene))
            {
                selectedRoute.Remove(selectedRoute.First());
                SetTexts();
            }
        }
    }
}
