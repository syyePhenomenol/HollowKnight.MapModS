using MagicUI.Core;
using MagicUI.Elements;
using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PD = MapModS.Data.PathfinderData;

namespace MapModS.UI
{
    internal class TransitionPersistent
    {
        private static LayoutRoot layout;

        private static TextObject route;

        public static Pathfinder pf;

        public static string lastStartScene = "";
        public static string lastFinalScene = "";
        public static string lastStartTransition = "";
        public static string lastFinalTransition = "";
        public static string selectedScene = "None";
        public static List<string> selectedRoute = new();
        public static HashSet<KeyValuePair<string, string>> rejectedTransitionPairs = new();

        private static bool Condition()
        {
            return TransitionData.TransitionModeActive()
                && !GUI.lockToggleEnable
                && (GUI.worldMapOpen
                    || GUI.quickMapOpen
                    || (!GameManager.instance.IsGamePaused()
                        && (MapModS.GS.routeTextInGame == RouteTextInGame.ShowNextTransitionOnly
                            || MapModS.GS.routeTextInGame == RouteTextInGame.Show)));
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Transition Persistent");
                layout.VisibilityCondition = Condition;

                route = UIExtensions.TextFromEdge(layout, "Route", false);

                UpdateAll();
            }

            pf = new();
        }

        public static void Destroy()
        {
            layout?.Destroy();
            layout = null;

            lastStartScene = "";
            lastFinalScene = "";
            lastStartTransition = "";
            lastFinalTransition = "";
            selectedScene = "None";
            selectedRoute.Clear();
            rejectedTransitionPairs.Clear();
        }

        public static void UpdateAll()
        {
            UpdateRoute();
        }

        public static void UpdateRoute()
        {
            string text = "\n";

            if (GUI.worldMapOpen)
            {
                text += "\n";
            }

            if (selectedRoute.Any())
            {
                if (MapModS.GS.routeTextInGame == RouteTextInGame.ShowNextTransitionOnly
                    && !GUI.quickMapOpen && !GUI.worldMapOpen)
                {
                    text += " -> " + selectedRoute.First().ToCleanName();
                }
                else
                {
                    foreach (string transition in selectedRoute)
                    {
                        if (text.Length > 128)
                        {
                            text += " -> ... -> " + selectedRoute.Last().ToCleanName();
                            break;
                        }

                        text += " -> " + transition.ToCleanName();
                    }
                }
            }

            route.Text = text;
        }

        private static Thread colorUpdateThread;

        // Called every 0.1 seconds
        public static void UpdateSelectedScene()
        {
            if (layout == null
                || !GUI.worldMapOpen
                || GUI.lockToggleEnable
                || GameManager.instance.IsGamePaused()
                || !TransitionData.TransitionModeActive())
            {
                return;
            }

            if (colorUpdateThread != null && colorUpdateThread.IsAlive) return;

            colorUpdateThread = new(() =>
            {
                if (Transition.GetRoomClosestToMiddle(selectedScene, out selectedScene))
                {
                    Transition.SetSelectedRoomColor(selectedScene);
                    UpdateAll();
                    TransitionWorldMap.UpdateAll();
                }
            });

            colorUpdateThread.Start();
        }

        private static Thread searchThread;

        // Called every frame
        public static void Update()
        {
            if (!TransitionData.TransitionModeActive()
                || !GUI.worldMapOpen
                || GUI.lockToggleEnable
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

        public static void GetRoute()
        {
            if (!GUI.worldMapOpen
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

            UpdateAll();
            TransitionWorldMap.UpdateAll();

            RouteCompass.UpdateCompass();
        }

        public static void RemoveTraversedTransition(string previousScene, string currentScene)
        {
            if (!selectedRoute.Any()) return;

            previousScene = Utils.RemoveBossSuffix(previousScene);
            currentScene = Utils.RemoveBossSuffix(currentScene);

            string transition = selectedRoute.First();

            if (transition.IsSpecialTransition() || previousScene == transition.GetScene())
            {
                if (currentScene == transition.GetAdjacentTransition().GetScene())
                {
                    selectedRoute.Remove(transition);
                    UpdateAll();
                }
            }

            if (!selectedRoute.Any())
            {
                rejectedTransitionPairs.Clear();
            }
        }
    }
}
