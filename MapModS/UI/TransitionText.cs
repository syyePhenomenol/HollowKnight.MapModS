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
    // Spaghetti warning
    internal class TransitionText
    {
        public static GameObject Canvas;

        //public static bool LockToggleEnable;

        private static CanvasPanel _instructionPanel;
        private static CanvasPanel _routePanel;
        private static CanvasPanel _uncheckedTransitionsPanelQuickMap;
        private static CanvasPanel _uncheckedTransitionsPanelWorldMap;

        public static TransitionHelper th;

        public static string lastStartScene = "";
        public static string lastFinalScene = "";
        public static string lastStartTransition = "";
        public static string lastFinalTransition = "";
        public static string selectedScene = "None";
        public static List<string> selectedRoute = new();
        public static HashSet<KeyValuePair<string, string>> rejectedTransitionPairs = new();

        public static bool TransitionModeActive()
        {
            return MapModS.LS.ModEnabled
                && SettingsUtil.IsTransitionRando()
                && (MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt);
        }

        public static void ShowQuickMap()
        {
            if (Canvas == null || _instructionPanel == null) return;

            if (!TransitionModeActive())
            {
                HideAll();
                return;
            }

            //LockToggleEnable = false;

            _instructionPanel.SetActive(false, false);
            _routePanel.SetActive(true, true);
            _uncheckedTransitionsPanelWorldMap.SetActive(false, false);
            _uncheckedTransitionsPanelQuickMap.SetActive(true, true);

            SetTexts();
        }

        public static void ShowWorldMap()
        {
            if (Canvas == null || _instructionPanel == null) return;

            if (!TransitionModeActive())
            {
                HideAll();
                return;
            }

            //LockToggleEnable = false;

            _instructionPanel.SetActive(true, true);
            _routePanel.SetActive(true, true);
            SetUncheckedTransitionsWorldMapActive();
            _uncheckedTransitionsPanelQuickMap.SetActive(false, false);

            SetTexts();
        }

        public static void Hide()
        {
            if (Canvas == null || _instructionPanel == null) return;

            if (!TransitionModeActive())
            {
                HideAll();
                return;
            }

            //LockToggleEnable = false;

            _instructionPanel.SetActive(false, false);
            _uncheckedTransitionsPanelWorldMap.SetActive(false, false);
            _uncheckedTransitionsPanelQuickMap.SetActive(false, false);

            SetRouteActive();
        }

        public static void HideAll()
        {
            _instructionPanel.SetActive(false, false);
            _routePanel.SetActive(false, false);
            _uncheckedTransitionsPanelWorldMap.SetActive(false, false);
            _uncheckedTransitionsPanelQuickMap.SetActive(false, false);
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

            _routePanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 20f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _routePanel.AddText("Route", "", new Vector2(20f, 20f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);

            _routePanel.SetActive(false, false);

            _uncheckedTransitionsPanelQuickMap = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 20f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _uncheckedTransitionsPanelQuickMap.AddText("Unchecked", "Transitions: None", new Vector2(-37f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperRight);

            _uncheckedTransitionsPanelQuickMap.SetActive(false, false);

            _uncheckedTransitionsPanelWorldMap = new CanvasPanel
                (_canvas, GUIController.Instance.Images["EnemiesBg"], new Vector2(1400f, 150f), Vector2.zero, new Rect(0f, 0f, GUIController.Instance.Images["EnemiesBg"].width, GUIController.Instance.Images["EnemiesBg"].height));
            _uncheckedTransitionsPanelWorldMap.AddText("UncheckedSelected", "Transitions: None", new Vector2(20f, 20f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);

            _uncheckedTransitionsPanelWorldMap.SetActive(false, false);

            SetTexts();
        }

        public static void SetTexts()
        {
            SetInstructionsText();
            SetControlText();
            SetRouteText();
            SetUncheckedTransitionsWorldMapText();
            SetUncheckedTransitionsQuickMapText();
        }

        private static Thread searchThread;

        // Called every frame
        public static void Update()
        {
            if (Canvas == null || HeroController.instance == null) return;

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.B))
            {
                MapModS.GS.ToggleAllowBenchWarp();
                SetTexts();
                rejectedTransitionPairs = new();
            }

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.U))
            {
                MapModS.GS.ToggleUncheckedPanel();
                SetTexts();
                SetUncheckedTransitionsWorldMapActive();
            }

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.R))
            {
                MapModS.GS.ToggleRouteTextInGame();
                SetTexts();
                SetRouteActive();
            }

            if (!_instructionPanel.Active
                || !_routePanel.Active
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
        }

        private static Thread colorUpdateThread;

        // Called every 0.1 seconds
        public static void UpdateSelectedScene()
        {
            if (Canvas == null
                || !_instructionPanel.Active
                || !_routePanel.Active
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
                    SetUncheckedTransitionsWorldMapText();
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

        public static void SetControlText()
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

            if (MapModS.GS.uncheckedPanelActive)
            {
                routeText += "\nShow unchecked/visited (Ctrl-U): On";
            }
            else
            {
                routeText += "\nShow unchecked/visited (Ctrl-U): Off";
            }

            switch (MapModS.GS.routeTextInGame)
            {
                case RouteTextInGame.Hide:
                    routeText += "\nShow route in-game (Ctrl-R): Off";
                    break;
                case RouteTextInGame.Show:
                    routeText += "\nShow route in-game (Ctrl-R): Full";
                    break;
                case RouteTextInGame.ShowNextTransitionOnly:
                    routeText += "\nShow route in-game (Ctrl-R): Next Transition Only";
                    break;
            }

            _instructionPanel.GetText("Route").UpdateText(routeText);
        }

        public static void SetRouteActive()
        {
            if (Canvas == null || _routePanel == null) return;

            bool isActive = MapModS.LS.ModEnabled
                && SettingsUtil.IsTransitionRando()
                && (MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt)
                && HeroController.instance != null && !HeroController.instance.GetCState("isPaused")
                && RandomizerMod.RandomizerData.Data.IsRoom(StringUtils.CurrentNormalScene())
                && (MapModS.GS.routeTextInGame != RouteTextInGame.Hide
                        || _instructionPanel.Active
                        || _uncheckedTransitionsPanelQuickMap.Active);

            _routePanel.SetActive(isActive, isActive);

            SetRouteText();
        }

        public static void SetRouteText()
        {
            string transitionsText = "";

            if (selectedRoute.Any())
            {
                if (MapModS.GS.routeTextInGame == RouteTextInGame.ShowNextTransitionOnly
                    && !_instructionPanel.Active
                    && !_uncheckedTransitionsPanelQuickMap.Active)
                {
                    transitionsText += " -> " + selectedRoute.First();
                }
                else
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
            }

            _routePanel.GetText("Route").UpdateText(transitionsText);
        }

        // Display both unchecked and visited transitions of current room
        public static void SetUncheckedTransitionsQuickMapText()
        {
            _uncheckedTransitionsPanelQuickMap.GetText("Unchecked").UpdateText(GetUncheckedVisited(StringUtils.CurrentNormalScene()));
        }

        public static void SetUncheckedTransitionsWorldMapActive()
        {
            if (Canvas == null || _uncheckedTransitionsPanelWorldMap == null) return;

            bool isActive = _instructionPanel.Active
                && MapModS.GS.uncheckedPanelActive;

            _uncheckedTransitionsPanelWorldMap.SetActive(isActive, isActive);
        }

        public static void SetUncheckedTransitionsWorldMapText()
        {
            _uncheckedTransitionsPanelWorldMap.GetText("UncheckedSelected").UpdateText(GetUncheckedVisited(selectedScene));
        }

        public static string GetUncheckedVisited(string scene)
        {
            string uncheckedTransitionsText = "";
            IEnumerable<string> uncheckedTransitions = GetUncheckedTransitions(scene);

            if (uncheckedTransitions.Any())
            {
                uncheckedTransitionsText += "Unchecked:";

                foreach (string transition in uncheckedTransitions)
                {
                    uncheckedTransitionsText += "\n" + transition;
                }

                uncheckedTransitionsText += "\n\n";
            }

            IEnumerable<Tuple<string, string>> visitedTransitions = GetVisitedTransitions(scene);

            if (visitedTransitions.Any())
            {
                uncheckedTransitionsText += "Visited:";

                foreach (Tuple<string, string> transition in visitedTransitions)
                {
                    uncheckedTransitionsText += "\n" + transition.Item1 + " -> " + transition.Item2;
                }
            }

            return uncheckedTransitionsText;
        }

        public static IEnumerable<string> GetUncheckedTransitions(string scene)
        {
            return RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableTransitions
                .Where(t => RandomizerMod.RandomizerData.Data.GetTransitionDef(t).SceneName == scene)
                .Select(t => RandomizerMod.RandomizerData.Data.GetTransitionDef(t).DoorName);
        }

        public static IEnumerable<Tuple<string, string>> GetVisitedTransitions(string scene)
        {
            return RandomizerMod.RandomizerMod.RS.TrackerData.visitedTransitions
                .Where(t => RandomizerMod.RandomizerData.Data.GetTransitionDef(t.Key).SceneName == scene)
                .Select(t => new Tuple<string, string>(RandomizerMod.RandomizerData.Data.GetTransitionDef(t.Key).DoorName, t.Value));
        }

        public static void GetRoute()
        {
            if (_instructionPanel == null
                || _routePanel == null
                || !_instructionPanel.Active
                || !_routePanel.Active
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
