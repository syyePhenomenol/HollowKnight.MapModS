using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MapChanger;
using RandoMapMod.Settings;

namespace RandoMapMod.Transition
{
    internal static class RouteTracker
    {
        public static string lastStartScene = "";
        public static string lastFinalScene = "";
        public static string lastStartTransition = "";
        public static string lastFinalTransition = "";
        public static int transitionsCount = 0;
        public static List<string> selectedRoute = new();
        public static List<List<string>> rejectedRoutes = new();

        private static Thread GetRouteThread;

        public static void SelectRoute(string scene)
        {
            if (GetRouteThread is null || !GetRouteThread.IsAlive)
            {
                GetRouteThread = new Thread(() => GetRoute(scene));
                GetRouteThread.Start();
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

            //RouteCompass.UpdateCompass();
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
                    //RouteCompass.UpdateCompass();
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
    }
}
