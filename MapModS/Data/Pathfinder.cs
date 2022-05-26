using RandomizerCore.Logic;
using System.Collections.Generic;
using System.Linq;
using PD = MapModS.Data.PathfinderData;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Data
{
    public class Pathfinder
    {
        private RandomizerMod.Settings.TrackerData Td => RM.RS.TrackerData;

        private readonly ProgressionManager localPm;

        public Pathfinder()
        {
            localPm = new(PD.lm, RM.RS.Context);
        }

        private string searchScene;
        private HashSet<string> candidateReachableTransitions = new();

        // Calculates the shortest route (by number of transitions) from startScene to finalScene.
        // The search space will be purely limited to rooms that have been visited + unreached reachable locations
        // A ProgressionManager is used to track logic while traversing through the search space
        public List<string> ShortestRoute(string startScene, string finalScene, HashSet<KeyValuePair<string, string>> rejectedTransitionPairs, bool allowBenchWarp)
        {
            if (startScene == null || finalScene == null) return new();

            HashSet<string> normalTransitionSpace = new();

            // Add normal transitions
            foreach (KeyValuePair<string, LogicTransition> transitionEntry in Td.lm.TransitionLookup)
            {
                if (Td.uncheckedReachableTransitions.Contains(transitionEntry.Key)
                    || PD.GetAdjacentTransition(transitionEntry.Key) == null) continue;

                string scene = PD.GetScene(transitionEntry.Key);

                if (MapModS.LS.mapMode == Settings.MapMode.TransitionRandoAlt
                    && !PlayerData.instance.scenesVisited.Contains(scene)) continue;

                if (Td.pm.Has(transitionEntry.Value.term.Id)
                    // Prevents adding certain randomized transitions that haven't been visited yet in uncoupled rando
                    && !(TransitionData.IsRandomizedTransition(transitionEntry.Key)
                        && !Td.visitedTransitions.ContainsKey(transitionEntry.Key)))
                {
                    normalTransitionSpace.Add(transitionEntry.Key);
                }
            }

            normalTransitionSpace.Remove(null);

            UpdateProgression();

            // Algorithm (BFS)
            HashSet<string> visitedTransitions = new();
            LinkedList<SearchNode> queue = new();

            // Add initial bench warp transitions if setting is enabled
            if (allowBenchWarp && Dependencies.HasDependency("Benchwarp"))
            {
                foreach (string transition in PD.GetBenchwarpTransitions())
                {
                    // Remove the single transition rejected routes
                    if (rejectedTransitionPairs.Any(p => p.Key == transition && p.Value == transition)) continue;
                    
                    queue.AddLast(InitializeNode(transition));
                }
            }
            
            searchScene = startScene;

            localPm.StartTemp();

            // Use all normal transitions in current scene as "seed" for special transitions
            foreach (string transition in TransitionData.GetTransitionsByScene(startScene))
            {
                if (normalTransitionSpace.Contains(transition))
                {
                    localPm.Set(transition, 1);
                }
            }

            candidateReachableTransitions = PD.GetTransitionsInScene(startScene);

            while (UpdateReachableTransitions()) { }

            foreach (string transition in candidateReachableTransitions)
            {
                // Remove the single transition rejected routes
                if (rejectedTransitionPairs.Any(p => p.Key == transition && p.Value == transition)) continue;

                if (localPm.Get(transition) > 0)
                {
                    queue.AddLast(InitializeNode(transition));
                }
            }

            localPm.RemoveTempItems();

            while (queue.Any())
            {
                SearchNode currentNode = queue.First();
                queue.RemoveFirst();

                // Avoid going through a rejected path, and remove redudant new paths
                if (currentNode.currentScene == finalScene && !rejectedTransitionPairs.Any(pair => pair.Key == currentNode.currentRoute.First() && PD.GetAdjacentTransition(pair.Value) == PD.GetAdjacentTransition(currentNode.currentRoute.Last())))
                {
                    // No other paths to same final transition with a different starting benchwarp
                    if (rejectedTransitionPairs.Any(pair => PD.GetAdjacentTransition(pair.Value) == PD.GetAdjacentTransition(currentNode.currentRoute.Last()) && pair.Key.StartsWith("Warp"))) continue;

                    return currentNode.currentRoute;
                }

                searchScene = currentNode.currentScene;

                localPm.StartTemp();

                localPm.Set(currentNode.currentRoute.Last().GetAdjacentTransition(), 1);

                candidateReachableTransitions = new(PD.GetTransitionsInScene(searchScene));

                while (UpdateReachableTransitions()) { }

                foreach(string transition in candidateReachableTransitions.Where(t => !visitedTransitions.Contains(t)))
                {
                    if (transition.IsSpecialTransition()
                        || normalTransitionSpace.Contains(transition))
                    {
                        string adjacent = transition.GetAdjacentTransition();

                        // No circling back on previous transition
                        if (adjacent == null || currentNode.currentRoute.Any(t => t == adjacent)) continue;

                        SearchNode newNode = new(PD.GetScene(adjacent), currentNode.currentRoute, adjacent);
                        newNode.currentRoute.Add(transition);

                        queue.AddLast(newNode);

                        visitedTransitions.Add(transition);
                    }
                }

                localPm.RemoveTempItems();
                candidateReachableTransitions.Clear();
            }

            // No route found, or the parameters are invalid
            return new();
        }

        private void UpdateProgression()
        {
            foreach (Term term in Td.pm.lm.Terms)
            {
                if (!RandomizerMod.RandomizerData.Data.IsTransition(term.Name)
                    && !RandomizerMod.RandomizerData.Data.IsRoom(term.Name))
                {
                    localPm.Set(term.Id, Td.pm.Get(term.Id));
                }
            }

            // Emulate a transition being possibly available via having the required term
            foreach (KeyValuePair<string, string> pair in PD.conditionalTerms)
            {
                if (Td.pm.Get(pair.Key) > 0)
                {
                    localPm.Set(pair.Value, 1);
                }
            }

            // Persistent terms should always be true (reachable anywhere in the scene without movement requirements)
            foreach (string term in PD.persistentTerms)
            {
                if (Td.pm.Get(term) > 0)
                {
                    localPm.Set(term, 1);
                }
            }

            if (PlayerData.instance.GetBool("mineLiftOpened"))
            {
                localPm.Set("Town[right1]", 1);
            }

            foreach (PersistentBoolData pbd in SceneData.instance.persistentBoolItems)
            {
                if (pbd.sceneName == "Waterways_02" && pbd.id == "Quake Floor (1)")
                {
                    localPm.Set("Broke_Waterways_Bench_Ceiling", pbd.activated ? 1 : 0);
                }
                else if (pbd.sceneName == "Waterways_02" && pbd.id == "Quake Floor")
                {
                    localPm.Set("Broke_Waterways_Bench_Floor", pbd.activated ? 1 : 0);
                }
                else if (pbd.sceneName == "Ruins1_31" && pbd.id == "Ruins Lift")
                {
                    localPm.Set("City_Toll_Wall_Broken", pbd.activated ? 1 : 0);
                }
            }

            foreach (PersistentIntData pid in SceneData.instance.persistentIntItems)
            {
                if (pid.sceneName == "Ruins1_31" && pid.id == "Ruins Lift")
                {
                    localPm.Set("City_Toll_Elevator_Up", pid.value % 2 == 1 ? 1 : 0);
                    localPm.Set("City_Toll_Elevator_Down", pid.value % 2 == 0 ? 1 : 0);
                }
            }
        }

        private SearchNode InitializeNode(string transition)
        {
            string scene = transition.GetAdjacentTransition().GetScene();

            return new SearchNode(scene, new() { transition }, transition.GetAdjacentTransition());
        }

        // Add other in-logic transitions in the current room
        private bool UpdateReachableTransitions()
        {
            bool continueUpdating = false;

            foreach (string transition in candidateReachableTransitions)
            {
                if (localPm.lm.TransitionLookup[transition].CanGet(localPm)
                    && localPm.Get(transition) < 1)
                {
                    localPm.Set(transition, 1);
                    continueUpdating = true;
                }
            }

            if (TransitionData.TryGetSceneWaypoint(searchScene, out LogicWaypoint waypoint)
                && !localPm.Has(waypoint.term) && waypoint.CanGet(localPm))
            {
                localPm.Add(waypoint);
                continueUpdating = true;
            }

            return continueUpdating;
        }

        private class SearchNode
        {
            public SearchNode(string scene, List<string> route, string lat)
            {
                currentScene = scene;
                currentRoute = new(route);
                lastAdjacentTransition = lat;
            }

            public void PrintRoute()
            {
                string text = "Current route:";

                foreach (string transition in currentRoute)
                {
                    text += " -> " + transition;
                }

                MapModS.Instance.Log(text);
            }

            public string currentScene;
            public List<string> currentRoute = new();
            public string lastAdjacentTransition;
        }
    }
}
