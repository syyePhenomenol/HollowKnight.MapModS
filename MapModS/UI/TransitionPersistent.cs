using MagicUI.Core;
using MagicUI.Elements;
using MapModS.Data;
using MapModS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static MapModS.Map.MapRooms;

namespace MapModS.UI
{
    internal class TransitionPersistent
    {
        private static LayoutRoot layout;

        private static TextObject route;

        public static string lastStartScene = "";
        public static string lastFinalScene = "";
        public static string lastStartTransition = "";
        public static string lastFinalTransition = "";
        public static int transitionsCount = 0;
        public static List<string> selectedRoute = new();
        public static List<List<string>> rejectedRoutes = new();

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
        }

        public static void Destroy()
        {
            layout?.Destroy();
            layout = null;

            ResetRoute();
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

        private static Thread searchThread;

        // Called every frame
        public static void Update()
        {
            if (!TransitionData.TransitionModeActive()
                || !GUI.worldMapOpen
                || GUI.lockToggleEnable
                || GameManager.instance.IsGamePaused()
                || InputHandler.Instance == null)
            {
                return;
            }

            // Use menu selection button for control
            if (InputHandler.Instance.inputActions.menuSubmit.WasPressed
                && (searchThread == null || !searchThread.IsAlive))
            {
                searchThread = new(GetRoute);
                searchThread.Start();
                Benchwarp.attackHoldTimer.Reset();
            }
        }

        public static void GetRoute()
        {
            if (Pathfinder.localPm == null) return;

            if (lastStartScene != Utils.CurrentScene() || lastFinalScene != InfoPanels.selectedScene)
            {
                rejectedRoutes.Clear();
            }

            try
            {
                selectedRoute = Pathfinder.ShortestRoute(Utils.CurrentScene(), InfoPanels.selectedScene, rejectedRoutes, MapModS.GS.allowBenchWarpSearch, false);
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(e);
            }

            AfterGetRoute();
        }

        public static void ReevaluateRoute(ItemChanger.Transition lastTransition)
        {
            if (Pathfinder.localPm == null) return;

            rejectedRoutes.Clear();

            try
            {
                selectedRoute = Pathfinder.ShortestRoute(lastTransition.ToString(), lastFinalTransition.GetAdjacentTerm(), rejectedRoutes, MapModS.GS.allowBenchWarpSearch, true);
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(e);
            }

            AfterGetRoute();
        }

        public static void AfterGetRoute()
        {
            if (!selectedRoute.Any())
            {
                ResetRoute();
            }
            else
            {
                lastStartScene = Utils.CurrentScene();
                lastFinalScene = selectedRoute.Last().GetAdjacentScene();
                lastStartTransition = selectedRoute.First();
                lastFinalTransition = selectedRoute.Last(); 
                transitionsCount = selectedRoute.Count();

                rejectedRoutes.Add(selectedRoute);
            }

            UpdateAll();
            TransitionWorldMap.UpdateInstructions();
            TransitionWorldMap.UpdateRouteSummary();

            RouteCompass.UpdateCompass();
        }

        public static void ResetRoute()
        {
            lastStartScene = "";
            lastFinalScene = "";
            lastStartTransition = "";
            lastFinalTransition = "";
            transitionsCount = 0;
            selectedRoute.Clear();
            rejectedRoutes.Clear();
        }

        public static void UpdateRoute(ItemChanger.Transition lastTransition)
        {
            if (!selectedRoute.Any()) return;

            string transition = selectedRoute.First();

            // Check adjacent transition matches the route's transition
            if (lastTransition.GateName == "" && transition.IsBenchwarpTransition())
            {
                (string scene, string respawnMarker) = BenchwarpInterop.benchKeys[transition];

                if (lastTransition.SceneName == scene && PlayerData.instance.respawnMarkerName == respawnMarker)
                {
                    UpdateRoute();
                    return;
                }
            }
            else if (lastTransition.ToString() == transition.GetAdjacentTerm())
            {
                UpdateRoute();
                return;
            }

            // The transition doesn't match the route
            switch (MapModS.GS.whenOffRoute)
            {
                case OffRouteBehaviour.Cancel:
                    ResetRoute();
                    UpdateAll();
                    TransitionWorldMap.UpdateAll();
                    RouteCompass.UpdateCompass();
                    break;
                case OffRouteBehaviour.Reevaluate:
                    ReevaluateRoute(lastTransition);
                    break;
                default:
                    break;
            }

            void UpdateRoute()
            {
                selectedRoute.Remove(transition);
                UpdateAll();
                TransitionWorldMap.UpdateInstructions();
                TransitionWorldMap.UpdateRouteSummary();

                if (!selectedRoute.Any())
                {
                    rejectedRoutes.Clear();
                }
            }
        }
    }
}
