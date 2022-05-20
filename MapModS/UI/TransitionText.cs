using MapModS.CanvasUtil;
using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using RandomizerMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using PD = MapModS.Data.PathfinderData;

namespace MapModS.UI
{
    internal class TransitionText
    {
        public static GameObject Canvas;

        private static CanvasPanel _instructionPanel;
        private static CanvasPanel _routePanel;
        private static CanvasPanel _uncheckedTransitionsPanelQuickMap;
        private static CanvasPanel _uncheckedTransitionsPanelWorldMap;

        public static Pathfinder pf;

        public static string lastStartScene = "";
        public static string lastFinalScene = "";
        public static string lastStartTransition = "";
        public static string lastFinalTransition = "";
        public static string selectedScene = "None";
        public static List<string> selectedRoute = new();
        public static HashSet<KeyValuePair<string, string>> rejectedTransitionPairs = new();

        public static void Initialize()
        {
            pf = new();
        }

        public static void ClearData()
        {
            lastStartScene = "";
            lastFinalScene = "";
            lastStartTransition = "";
            lastFinalTransition = "";
            selectedScene = "None";
            selectedRoute.Clear();
            rejectedTransitionPairs.Clear();
        }

        public static void ShowQuickMap()
        {
            if (Canvas == null || _instructionPanel == null) return;

            if (!TransitionData.TransitionModeActive())
            {
                HideAll();
                return;
            }

            _instructionPanel.SetActive(false, false);
            _routePanel.SetActive(true, true);
            _uncheckedTransitionsPanelWorldMap.SetActive(false, false);
            _uncheckedTransitionsPanelQuickMap.SetActive(true, true);

            SetTexts();
        }

        public static void ShowWorldMap()
        {
            if (Canvas == null || _instructionPanel == null) return;

            if (!TransitionData.TransitionModeActive())
            {
                HideAll();
                return;
            }

            _instructionPanel.SetActive(true, true);
            _routePanel.SetActive(true, true);
            SetUncheckedTransitionsWorldMapActive();
            _uncheckedTransitionsPanelQuickMap.SetActive(false, false);

            SetTexts();

            SetRoomColors();
        }

        public static void Hide()
        {
            if (Canvas == null || _instructionPanel == null) return;

            if (!TransitionData.TransitionModeActive())
            {
                HideAll();
                return;
            }

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

        public static void BuildText(GameObject _canvas)
        {
            Canvas = _canvas;
            _instructionPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 20f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _instructionPanel.AddText("Instructions", "None", new Vector2(20f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);
            _instructionPanel.AddText("Control", "", new Vector2(-37f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperRight);

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
                (_canvas, GUIController.Instance.Images["UncheckedBG"], new Vector2(1400f, 150f), Vector2.zero, new Rect(0f, 0f, GUIController.Instance.Images["UncheckedBG"].width, GUIController.Instance.Images["UncheckedBG"].height));
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
            if (Canvas == null
                || HeroController.instance == null
                || !TransitionData.TransitionModeActive()) return;

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.B)
                && Dependencies.HasDependency("Benchwarp"))
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

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.C))
            {
                MapModS.GS.ToggleRouteCompassEnabled();
                SetTexts();
            }

            if (!_instructionPanel.Active
                || !_routePanel.Active
                || GameManager.instance.IsGamePaused())
            {
                return;
            }

            // Use menu selection button for control
            if (InputHandler.Instance != null && InputHandler.Instance.inputActions.menuSubmit.WasPressed
                && (searchThread == null || !searchThread.IsAlive))
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
                || GameManager.instance.IsGamePaused()
                || !TransitionData.TransitionModeActive())
            {
                return;
            }

            if (colorUpdateThread != null && colorUpdateThread.IsAlive) return;

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

        private static double DistanceToMiddle(Transform transform)
        {
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
            string instructionsText = $"{Localization.Localize("Selected room")}: {selectedScene}.";

            List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.menuSubmit.Bindings);

            if (selectedScene == Utils.CurrentScene())
            {
                instructionsText += $" {Localization.Localize("You are here")}.";
            }
            else
            {
                instructionsText += $" {Localization.Localize("Press")} ";

                instructionsText += $"[{bindings.First().Name}]";

                if (bindings.Count > 1 && bindings[1].BindingSourceType == InControl.BindingSourceType.DeviceBindingSource)
                {
                    instructionsText += $" {Localization.Localize("or")} ";

                    instructionsText += $"({bindings[1].Name})";
                }

                instructionsText += $" {Localization.Localize("to find new route or switch starting / final transitions")}.";
            }

            _instructionPanel.GetText("Instructions").UpdateText(instructionsText);
        }

        public static void SetControlText()
        {
            string controlText = $"{Localization.Localize("Current route")}: ";

            if (lastStartScene != ""
                && lastFinalScene != ""
                && lastStartTransition != ""
                && lastFinalTransition != ""
                && selectedRoute.Any())
            {
                controlText += $"{lastStartTransition} ->...-> {lastFinalTransition}      ";
                controlText += $"{Localization.Localize("Transitions")}: {selectedRoute.Count()}";
            }
            else
            {
                controlText += Localization.Localize("None");
            }

            if (Dependencies.HasDependency("Benchwarp"))
            {
                controlText += $"\n{Localization.Localize("Include benchwarp")} (Ctrl-B): ";

                if (MapModS.GS.allowBenchWarpSearch)
                {
                    controlText += Localization.Localize("On");
                }
                else
                {
                    controlText += Localization.Localize("Off");
                }
            }

            controlText += $"\n{Localization.Localize("Show unchecked/visited")} (Ctrl-U): ";

            if (MapModS.GS.uncheckedPanelActive)
            {
                controlText += Localization.Localize("On");
            }
            else
            {
                controlText += Localization.Localize("Off");
            }

            controlText += $"\n{Localization.Localize("Show route in-game")} (Ctrl-R): ";

            switch (MapModS.GS.routeTextInGame)
            {
                case RouteTextInGame.Hide:
                    controlText += Localization.Localize("Off");
                    break;
                case RouteTextInGame.Show:
                    controlText += Localization.Localize("Full");
                    break;
                case RouteTextInGame.ShowNextTransitionOnly:
                    controlText += Localization.Localize("Next Transition Only");
                    break;
            }

            controlText += $"\n{Localization.Localize("Show route compass")} (Ctrl-C): ";

            if (MapModS.GS.routeCompassEnabled)
            {
                controlText += Localization.Localize("On");
            }
            else
            {
                controlText += Localization.Localize("Off");
            }

            _instructionPanel.GetText("Control").UpdateText(controlText);
        }

        public static void SetRouteActive()
        {
            if (Canvas == null || _routePanel == null) return;

            bool isActive = TransitionData.TransitionModeActive()
                && HeroController.instance != null && !HeroController.instance.GetCState("isPaused")
                && (RandomizerMod.RandomizerData.Data.IsRoom(Utils.CurrentScene())
                    || Utils.CurrentScene() == "Room_Tram"
                    || Utils.CurrentScene() == "Room_Tram_RG")
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
            _uncheckedTransitionsPanelQuickMap.GetText("Unchecked").UpdateText(TransitionData.GetUncheckedVisited(Utils.CurrentScene()));
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
            _uncheckedTransitionsPanelWorldMap.GetText("UncheckedSelected").UpdateText(TransitionData.GetUncheckedVisited(selectedScene));
        }

        public static void GetRoute()
        {
            if (_instructionPanel == null
                || _routePanel == null
                || !_instructionPanel.Active
                || !_routePanel.Active
                || HeroController.instance == null
                || GameManager.instance.IsGamePaused()
                || pf == null)
            {
                return;
            }

            if (lastStartScene != Utils.CurrentScene() || lastFinalScene != selectedScene)
            {
                rejectedTransitionPairs.Clear();
            }

            try
            {
                selectedRoute = pf.ShortestRoute(Utils.CurrentScene(), selectedScene, rejectedTransitionPairs, MapModS.GS.allowBenchWarpSearch);
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(e);
            }

            if (!selectedRoute.Any())
            {
                lastFinalScene = "";
                rejectedTransitionPairs.Clear();
            }
            else
            {
                lastStartScene = Utils.CurrentScene();
                lastFinalScene = selectedScene;
                lastStartTransition = selectedRoute.First();
                lastFinalTransition = PD.GetAdjacentTransition(selectedRoute.Last());

                rejectedTransitionPairs.Add(new(selectedRoute.First(), selectedRoute.Last()));
            }

            SetTexts();

            RouteCompass.UpdateCompass();
        }

        public static void RemoveTraversedTransition(string previousScene, string currentScene)
        {
            if (selectedRoute == null || !selectedRoute.Any()) return;

            previousScene = Utils.RemoveBossSuffix(previousScene);
            currentScene = Utils.RemoveBossSuffix(currentScene);

            if (PD.IsSpecialTransition(selectedRoute.First()))
            {
                if (currentScene == PD.GetScene(PD.GetAdjacentTransition(selectedRoute.First())))
                {
                    selectedRoute.Remove(selectedRoute.First());
                    SetTexts();
                }

                return;
            }

            if (selectedRoute.Count >= 2 && previousScene == TransitionData.GetTransitionScene(selectedRoute.First()))
            {
                if (PD.IsSpecialTransition(selectedRoute.ElementAt(1)))
                {
                    if (PD.VerifySpecialTransition(selectedRoute.ElementAt(1), currentScene))
                    {
                        selectedRoute.Remove(selectedRoute.First());
                        SetTexts();
                    }
                }
                else if (currentScene == TransitionData.GetTransitionScene(selectedRoute.ElementAt(1)))
                {
                    selectedRoute.Remove(selectedRoute.First());
                    SetTexts();
                }

                return;
            }

            if (previousScene == TransitionData.GetTransitionScene(selectedRoute.First())
                && currentScene == lastFinalScene)
            {
                selectedRoute.Remove(selectedRoute.First());
                rejectedTransitionPairs.Clear();
                SetTexts();
            }

        }
    }
}
