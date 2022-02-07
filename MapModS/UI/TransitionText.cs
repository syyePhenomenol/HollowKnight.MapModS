using MapModS.CanvasUtil;
using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

namespace MapModS.UI
{
    internal class TransitionText
    {
        public static GameObject Canvas;

        public static bool LockToggleEnable;

        private static CanvasPanel _instructionPanel;
        private static CanvasPanel _transitionPanel;
        private static CanvasPanel _uncheckedTransitionsPanel;

        public static TransitionHelper th;

        public static string lastStartScene = "";
        public static string lastFinalScene = "";
        public static string lastStartTransition = "";
        public static string lastFinalTransition = "";
        public static string selectedScene = "None";
        public static List<string> selectedRoute = new();
        public static HashSet<KeyValuePair<string, string>> rejectedTransitionPairs = new();

        public static void Show()
        {
            if (Canvas == null) return;

            Canvas.SetActive(true);
            LockToggleEnable = false;
            SetTexts();
        }

        public static void ShowWorldMap()
        {
            if (Canvas == null || _instructionPanel == null) return;

            bool isActive = !LockToggleEnable && MapModS.LS.ModEnabled
                && SettingsUtil.IsTransitionRando()
                && (MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt);

            _instructionPanel.SetActive(isActive, isActive);
            _uncheckedTransitionsPanel.SetActive(false, false);

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

            lastStartScene = "";
            lastFinalScene = "";
            lastStartTransition = "";
            lastFinalTransition = "";
            selectedScene = "None";
            selectedRoute = new();
            rejectedTransitionPairs = new();
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

            _uncheckedTransitionsPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 20f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _uncheckedTransitionsPanel.AddText("Unchecked", "Transitions: None", new Vector2(-37f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperRight);

            _uncheckedTransitionsPanel.SetActive(false, false);

            SetTexts();
        }

        public static void SetTexts()
        {
            if (GameManager.instance.gameMap == null
                || _instructionPanel == null) return;

            bool isActive = !LockToggleEnable && MapModS.LS.ModEnabled
                && SettingsUtil.IsTransitionRando()
                && (MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt);

            _transitionPanel.SetActive(isActive, isActive);
            _uncheckedTransitionsPanel.SetActive(isActive && !_instructionPanel.Active, isActive && !_instructionPanel.Active);
            
            SetTransitionsText();
            SetUncheckedTransitionsText();
            SetRouteText();
        }

        private static Thread searchThread;

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
                searchThread = new(GetRoute);
                searchThread.Start();
            }

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.B))
            {
                MapModS.GS.ToggleAllowBenchWarp();
                SetRouteText();
                rejectedTransitionPairs = new();
            }
        }

        private static Thread colorUpdateThread;

        // Called every 0.1 seconds
        public static void UpdateSelectedScene()
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

            colorUpdateThread = new(() =>
            {
                if (GetRoomClosestToMiddle(selectedScene, out selectedScene))
                {
                    SetInstructionsText();
                    SetRoomColors();
                }
            });

            colorUpdateThread.Start();
        }

        private static double DistanceToMiddle(Transform transform, bool shift)
        {
            if (shift)
            {
                return Math.Pow(transform.position.x - 4.5f, 2) + Math.Pow(transform.position.y + 1.2f, 2);
            }

            return Math.Pow(transform.position.x, 2) + Math.Pow(transform.position.y, 2);
        }

        private static readonly Vector4 selectionColor = new(255, 255, 0, 0.8f);
        
        public static bool GetRoomClosestToMiddle(string previousScene, out string selectedScene)
        {
            selectedScene = null;
            double minDistance = double.PositiveInfinity;

            GameObject go_GameMap = GameManager.instance.gameMap;

            if (go_GameMap == null) return false;

            foreach (Transform areaObj in go_GameMap.transform)
            {
                bool shift = areaObj.name == "MMS Custom Map Rooms";

                foreach (Transform roomObj in areaObj.transform)
                {
                    if (!roomObj.gameObject.activeSelf) continue;

                    Transition.ExtraMapData extra = roomObj.GetComponent<Transition.ExtraMapData>();

                    if (extra == null) continue;

                    double distance = DistanceToMiddle(roomObj, shift);

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

        // This method handles highlighting the selected room
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

                    if (areaObj.name == "MMS Custom Map Rooms")
                    {
                        TextMeshPro tmp = roomObj.gameObject.GetComponent<TextMeshPro>();

                        if (extra.sceneName == selectedScene)
                        {
                            tmp.color = selectionColor;
                        }
                        else
                        {
                            tmp.color = extra.origTransitionColor;
                        }

                        continue;
                    }

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

            if (selectedScene == StringUtils.CurrentNormalScene())
            {
                instructionsText += " You are here.";
            }
            else
            {
                instructionsText += $" Press [Menu Select] to find new route or switch starting / final transitions.";
            }

            _instructionPanel.GetText("Instructions").UpdateText(instructionsText);
        }

        public static void SetRouteText()
        {
            string routeText = "Current route: ";

            if (lastStartScene != ""
                && lastFinalScene != ""
                && lastStartTransition != ""
                && lastFinalTransition != ""
                && selectedRoute.Any())
            {
                routeText += $"{lastStartTransition} ->...-> {lastFinalTransition}      ";
                routeText += $"Transitions: {selectedRoute.Count()}";
            }
            else
            {
                routeText += "None";
            }

            if (MapModS.GS.allowBenchWarpSearch)
            {
                routeText += "\nInclude benchwarp (Ctrl-B): On";
            }
            else
            {
                routeText += "\nInclude benchwarp (Ctrl-B): Off";
            }

            _instructionPanel.GetText("Route").UpdateText(routeText);
        }

        public static void SetTransitionsText()
        {
            string transitionsText = "";

            if (selectedRoute.Any())
            {
                foreach (string transition in selectedRoute)
                {
                    if (transitionsText.Length > 128)
                    {
                        transitionsText += " -> ... -> " + selectedRoute.Last();
                        break;
                    }

                    transitionsText += " -> " + transition;
                }
            }

            _transitionPanel.GetText("Transitions").UpdateText(transitionsText);
        }

        // Display both unchecked and visited transitions of current room
        public static void SetUncheckedTransitionsText()
        {
            string uncheckedTransitionsText = "";
            IEnumerable<string> uncheckedTransitions = GetUncheckedTransitions();

            if (uncheckedTransitions.Any())
            {
                uncheckedTransitionsText += "Unchecked:";

                foreach (string transition in uncheckedTransitions)
                {
                    uncheckedTransitionsText += "\n" + transition;
                }

                uncheckedTransitionsText += "\n\n";
            }

            IEnumerable<Tuple<string, string>> visitedTransitions = GetVisitedTransitions();

            if (visitedTransitions.Any())
            {
                uncheckedTransitionsText += "Visited:";

                foreach (Tuple<string, string> transition in visitedTransitions)
                {
                    uncheckedTransitionsText += "\n" + transition.Item1 +  " -> " + transition.Item2;
                }
            }

            _uncheckedTransitionsPanel.GetText("Unchecked").UpdateText(uncheckedTransitionsText);
        }

        public static IEnumerable<string> GetUncheckedTransitions()
        {
            return RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableTransitions
                .Where(t => RandomizerMod.RandomizerData.Data.GetTransitionDef(t).SceneName == StringUtils.CurrentNormalScene())
                .Select(t => RandomizerMod.RandomizerData.Data.GetTransitionDef(t).DoorName);
        }

        public static IEnumerable<Tuple<string, string>> GetVisitedTransitions()
        {
            return RandomizerMod.RandomizerMod.RS.TrackerData.visitedTransitions
                .Where(t => RandomizerMod.RandomizerData.Data.GetTransitionDef(t.Key).SceneName == StringUtils.CurrentNormalScene())
                .Select(t => new Tuple<string, string>(RandomizerMod.RandomizerData.Data.GetTransitionDef(t.Key).DoorName, t.Value));
        }

        public static void GetRoute()
        {
            if (_instructionPanel == null
                || _transitionPanel == null
                || !_instructionPanel.Active
                || !_transitionPanel.Active
                || HeroController.instance == null
                || GameManager.instance.IsGamePaused()
                || th == null)
            {
                return;
            }

            if (lastStartScene != StringUtils.CurrentNormalScene() || lastFinalScene != selectedScene)
            {
                rejectedTransitionPairs.Clear();
            }

            selectedRoute = th.ShortestRoute(StringUtils.CurrentNormalScene(), selectedScene, rejectedTransitionPairs, MapModS.GS.allowBenchWarpSearch);

            if (!selectedRoute.Any())
            {
                lastFinalScene = "";
                rejectedTransitionPairs.Clear();
            }
            else
            {
                lastStartScene = StringUtils.CurrentNormalScene();
                lastFinalScene = selectedScene;
                lastStartTransition = selectedRoute.First();
                lastFinalTransition = TransitionHelper.GetAdjacentTransition(selectedRoute.Last());

                rejectedTransitionPairs.Add(new(selectedRoute.First(), selectedRoute.Last()));
            }

            SetTexts();
        }

        public static void RemoveTraversedTransition(string previousScene, string currentScene)
        {
            if (selectedRoute == null || !selectedRoute.Any()) return;

            previousScene = StringUtils.RemoveBossSuffix(previousScene);
            currentScene = StringUtils.RemoveBossSuffix(currentScene);

            if (TransitionHelper.IsSpecialTransition(selectedRoute.First()))
            {
                if (currentScene == TransitionHelper.GetScene(TransitionHelper.GetAdjacentTransition(selectedRoute.First())))
                {
                    selectedRoute.Remove(selectedRoute.First());
                    SetTexts();
                }

                return;
            }

            if (selectedRoute.Count >= 2 && previousScene == RandomizerMod.RandomizerData.Data.GetTransitionDef(selectedRoute.First()).SceneName)
            {
                if (TransitionHelper.IsSpecialTransition(selectedRoute.ElementAt(1)))
                {
                    if (TransitionHelper.VerifySpecialTransition(selectedRoute.ElementAt(1), currentScene))
                    {
                        selectedRoute.Remove(selectedRoute.First());
                        SetTexts();
                    }
                }
                else if (currentScene == RandomizerMod.RandomizerData.Data.GetTransitionDef(selectedRoute.ElementAt(1)).SceneName)
                {
                    selectedRoute.Remove(selectedRoute.First());
                    SetTexts();
                }

                return;
            }

            if (previousScene == RandomizerMod.RandomizerData.Data.GetTransitionDef(selectedRoute.First()).SceneName
                && currentScene == lastFinalScene)
            {
                selectedRoute.Remove(selectedRoute.First());
                rejectedTransitionPairs.Clear();
                SetTexts();
            }

        }
    }
}
