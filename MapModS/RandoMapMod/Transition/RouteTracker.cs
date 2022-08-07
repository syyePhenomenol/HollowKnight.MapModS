using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using MapChanger;
using RandoMapMod.Settings;
using RandoMapMod.UI;

namespace RandoMapMod.Transition
{
    internal class RouteTracker : HookModule
    {
        private static List<string> selectedRoute = new();
        internal static ReadOnlyCollection<string> SelectedRoute => selectedRoute?.AsReadOnly();

        private static string lastStartScene = "";
        private static string lastFinalScene = "";
        private static string lastStartTransition = "";
        private static string lastFinalTransition = "";
        private static int transitionsCount = 0;

        private static readonly List<List<string>> rejectedRoutes = new();

        public override void OnEnterGame()
        {
            ItemChanger.Events.OnBeginSceneTransition += OnBeginSceneTransition;
            MapChanger.Settings.OnSettingChanged += OnSettingChanged;
        }

        public override void OnQuitToMenu()
        {
            ItemChanger.Events.OnBeginSceneTransition -= OnBeginSceneTransition;
            MapChanger.Settings.OnSettingChanged -= OnSettingChanged;
        }

        private static void OnBeginSceneTransition(ItemChanger.Transition obj)
        {
            UpdateRoute(obj);
        }

        private static void OnSettingChanged()
        {
            ResetRoute();
        }

        private static Thread SelectRouteThread;
        public static void SelectRoute(string scene)
        {
            if (SelectRouteThread is null || !SelectRouteThread.IsAlive)
            {
                SelectRouteThread = new Thread(() => GetRoute(scene));
                SelectRouteThread.Start();
                //Benchwarp.attackHoldTimer.Reset();
            }
        }

        public static void GetRoute(string scene)
        {
            if (Pathfinder.localPm == null) return;

            if (lastStartScene != Utils.CurrentScene() || lastFinalScene != scene)
            {
                rejectedRoutes.Clear();
            }

            try
            {
                selectedRoute = Pathfinder.ShortestRoute(Utils.CurrentScene(), scene, rejectedRoutes, false);
            }
            catch (Exception e)
            {
                RandoMapMod.Instance.LogError(e);
            }

            AfterGetRoute();
        }

        public static void ReevaluateRoute(ItemChanger.Transition lastTransition)
        {
            if (Pathfinder.localPm == null) return;

            rejectedRoutes.Clear();

            string transition = lastTransition.ToString();

            if (transition == "Fungus2_15[top2]")
            {
                transition = "Fungus2_15[top3]";
            }
            if (transition == "Fungus2_14[bot1]")
            {
                transition = "Fungus2_14[bot3]";
            }

            try
            {
                selectedRoute = Pathfinder.ShortestRoute(transition, lastFinalTransition.GetAdjacentTerm(), rejectedRoutes, true);
            }
            catch (Exception e)
            {
                RandoMapMod.Instance.LogError(e);
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

            //UpdateAll();
            //TransitionWorldMap.UpdateInstructions();
            //TransitionWorldMap.UpdateRouteSummary();

            RouteCompass.Update();
        }

        internal static void ResetRoute()
        {
            lastStartScene = "";
            lastFinalScene = "";
            lastStartTransition = "";
            lastFinalTransition = "";
            transitionsCount = 0;
            selectedRoute.Clear();
            rejectedRoutes.Clear();
        }

        internal static void UpdateRoute(ItemChanger.Transition lastTransition)
        {
            RandoMapMod.Instance.LogDebug("Last transition: " + lastTransition.ToString());

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
            else if (lastTransition.ToString() == transition.GetAdjacentTerm()
                || (lastTransition.ToString() == "Fungus2_15[top2]" && transition.GetAdjacentTerm() == "Fungus2_15[top3]")
                || (lastTransition.ToString() == "Fungus2_14[bot1]" && transition.GetAdjacentTerm() == "Fungus2_14[bot3]"))
            {
                UpdateRoute();
                return;
            }

            // The transition doesn't match the route
            switch (RandoMapMod.GS.WhenOffRoute)
            {
                case OffRouteBehaviour.Cancel:
                    ResetRoute();
                    //UpdateAll();
                    //TransitionWorldMap.UpdateAll();
                    RouteCompass.Update();
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
                //UpdateAll();
                //TransitionWorldMap.UpdateInstructions();
                //TransitionWorldMap.UpdateRouteSummary();

                if (!selectedRoute.Any())
                {
                    rejectedRoutes.Clear();
                }
            }
        }

        internal static string GetRouteText()
        {
            string text = "";

            if (!selectedRoute.Any()) return text;

            if (RandoMapMod.GS.RouteTextInGame is RouteTextInGame.NextTransitionOnly
                && !States.QuickMapOpen && !States.WorldMapOpen)
            {
                return text + " -> " + selectedRoute.First().ToCleanName();
            }

            foreach (string transition in selectedRoute)
            {
                if (text.Length > 128)
                {
                    text += " -> ... -> " + selectedRoute.Last().ToCleanName();
                    break;
                }

                text += " -> " + transition.ToCleanName();
            }

            return text;
        }

        internal static void TryBenchwarp()
        {
            if (selectedRoute.Any() && selectedRoute.First().IsBenchwarpTransition())
            {
                GameManager.instance.StartCoroutine(BenchwarpInterop.DoBenchwarp(selectedRoute.First()));
            }
        }
    }
}
