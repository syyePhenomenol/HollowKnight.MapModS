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

        // Calculates the shortest route (by number of transitions) from start to final.
        // The search space will be purely limited to rooms that have been visited + unreached reachable locations
        // A ProgressionManager is used to track logic while traversing through the search space
        // If reevaluating, start and final are transitions instead of scenes
        public List<string> ShortestRoute(string start, string final, HashSet<KeyValuePair<string, string>> rejectedTransitionPairs, bool allowBenchWarp, bool reevaluate)
        {
            if (start == null || final == null) return new();

            HashSet<string> candidateReachableTransitions = new();
            HashSet<string> normalTransitionSpace = new();

            // Add normal transitions
            foreach (string transition in Td.lm.TransitionLookup.Keys)
            {
                if (Td.uncheckedReachableTransitions.Contains(transition)
                    || PD.GetAdjacentTransition(transition) == null) continue;

                string scene = PD.GetScene(transition);

                if (MapModS.LS.mapMode == Settings.MapMode.TransitionRandoAlt
                    && !PlayerData.instance.scenesVisited.Contains(scene)) continue;

                if (Td.pm.Get(transition) > 0
                    // Prevents adding certain randomized transitions that haven't been visited yet in uncoupled rando
                    && !(TransitionData.IsRandomizedTransition(transition)
                        && !Td.visitedTransitions.ContainsKey(transition)))
                {
                    normalTransitionSpace.Add(transition);
                }
            }

            normalTransitionSpace.Remove(null);

            UpdateProgression();

            // Algorithm (BFS)
            HashSet<string> visitedTransitions = new();
            LinkedList<SearchNode> queue = new();
            string searchScene;

            InitializeNodeQueue();

            while (queue.Any())
            {
                SearchNode node = queue.First();
                queue.RemoveFirst();

                if (!reevaluate)
                {
                    // Avoid going through a rejected path, and remove redudant new paths
                    if (node.scene == final && !rejectedTransitionPairs.Any(pair => pair.Key == node.route.First() && PD.GetAdjacentTransition(pair.Value) == node.lastAdjacentTransition))
                    {
                        // No other paths to same final transition with a different starting benchwarp
                        if (rejectedTransitionPairs.Any(pair => pair.Value.GetAdjacentTransition() == node.lastAdjacentTransition && pair.Key.StartsWith("Warp"))) continue;

                        return node.route;
                    }
                }
                else
                {
                    // If reevaluating, we just check if the final transition is correct
                    if (node.scene == final.GetScene())
                    {
                        if (final != "" && node.lastAdjacentTransition != final) continue;

                        return node.route;
                    }
                }

                searchScene = node.scene;

                localPm.StartTemp();

                localPm.Set(node.lastAdjacentTransition, 1);

                candidateReachableTransitions = new(PD.GetTransitionsInScene(searchScene));

                while (UpdateReachableTransitions()) { }

                foreach (string transition in candidateReachableTransitions
                    .Where(t => !visitedTransitions.Contains(t))
                    .Where(t => localPm.Get(t) > 0))
                {
                    TryAddNode(node, transition);
                }

                localPm.RemoveTempItems();
                candidateReachableTransitions.Clear();
            }

            // No route found, or the parameters are invalid
            return new();

            void UpdateProgression()
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

                if (PlayerData.instance.GetBool("mineLiftOpened"))
                {
                    localPm.Set("Town_Lift_Activated", 1);
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

                //foreach (Term term in localPm.lm.Terms)
                //{
                //    MapModS.Instance.Log(term.Name + ": " + localPm.Get(term));
                //}
            }

            void InitializeNodeQueue()
            {
                // Add initial bench warp transitions if setting is enabled
                if (allowBenchWarp && Dependencies.HasDependency("Benchwarp"))
                {
                    foreach (string transition in PD.GetBenchwarpTransitions())
                    {
                        // Remove the single transition rejected routes
                        if (rejectedTransitionPairs.Any(p => p.Key == transition && p.Value == transition)) continue;

                        TryAddNode(null, transition);
                    }
                }

                localPm.StartTemp();

                // If reevaluating, start is a transition instead of a scene
                if (!reevaluate)
                {
                    // Use all normal transitions in current scene as "seed" for special transitions
                    foreach (string transition in TransitionData.GetTransitionsByScene(start))
                    {
                        if (normalTransitionSpace.Contains(transition))
                        {
                            localPm.Set(transition, 1);
                        }
                    }

                    searchScene = start;
                    candidateReachableTransitions = PD.GetTransitionsInScene(start);

                    while (UpdateReachableTransitions()) { }
                }
                else
                {
                    if (start.GetScene() != null)
                    {
                        localPm.Set(start, 1);
                        searchScene = start.GetScene();
                        candidateReachableTransitions = PD.GetTransitionsInScene(start.GetScene());

                        while (UpdateReachableTransitions()) { }
                    }
                }

                foreach (string transition in candidateReachableTransitions)
                {
                    // Remove the single transition rejected routes
                    if (rejectedTransitionPairs.Any(p => p.Key == transition && p.Value == transition)) continue;

                    TryAddNode(null, transition);
                }

                localPm.RemoveTempItems();

                if (reevaluate)
                {
                    // Prefer doubling back if possible, so make that transition highest priority
                    IEnumerable<SearchNode> startNode = queue.TakeWhile(n => n.route.First() == start);

                    if (startNode.Any())
                    {
                        queue.AddFirst(startNode.First());
                    }
                }
            }

            void TryAddNode(SearchNode node, string transition)
            {
                if (transition.IsSpecialTransition() || normalTransitionSpace.Contains(transition))
                {
                    SearchNode newNode;

                    string adjacent = transition.GetAdjacentTransition();

                    if (node != null)
                    {
                        // No circling back on previous transition
                        if (adjacent == null || node.route.Any(t => t == adjacent)) return;

                        newNode = new(transition.GetAdjacentScene(), node.route, adjacent);
                        newNode.route.Add(transition);
                    }
                    else
                    {
                        newNode = new(transition.GetAdjacentScene(), new() { transition }, adjacent);
                    }

                    queue.AddLast(newNode);

                    visitedTransitions.Add(transition);
                }

                return;
            }

            // Add other in-logic transitions in the current room
            bool UpdateReachableTransitions()
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

                if (PD.TryGetSceneWaypoint(searchScene, out LogicWaypoint waypoint)
                    && !localPm.Has(waypoint.term) && waypoint.CanGet(localPm))
                {
                    localPm.Add(waypoint);
                    continueUpdating = true;
                }

                return continueUpdating;
            }
        }

        class SearchNode
        {
            public SearchNode(string scene, List<string> route, string lat)
            {
                this.scene = scene;
                this.route = new(route);
                lastAdjacentTransition = lat;
            }

            public void PrintRoute()
            {
                string text = "Current route:";

                foreach (string transition in route)
                {
                    text += " -> " + transition;
                }

                MapModS.Instance.Log(text);
            }

            public string scene;
            public List<string> route = new();
            public string lastAdjacentTransition;
        }
    }
}
