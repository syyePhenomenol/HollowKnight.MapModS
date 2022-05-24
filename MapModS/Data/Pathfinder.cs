using RandomizerCore.Logic;
using System;
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

        private readonly HashSet<int> addedItems = new();
        private readonly HashSet<int> addedTerms = new();

        public Pathfinder()
        {
            localPm = new(RM.RS.Context.LM, RM.RS.Context);
        }

        private string searchScene;
        private HashSet<string> candidateReachableTransitions = new();
        private readonly HashSet<string> reachableTransitions = new();

        // Calculates the shortest route (by number of transitions) from startScene to finalScene.
        // The search space will be purely limited to rooms that have been visited + unreached reachable locations
        // A ProgressionManager is used to track logic while traversing through the search space
        public List<string> ShortestRoute(string startScene, string finalScene, HashSet<KeyValuePair<string, string>> rejectedTransitionPairs, bool allowBenchWarp)
        {
            if (startScene == null || finalScene == null) return new();

            IEnumerable<string> visitedBenches = Dependencies.GetVisitedBenchScenes();

            HashSet<string> transitionSpace = new();

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
                    transitionSpace.Add(transitionEntry.Key);
                }
            }

            // Add stag transitions
            foreach (KeyValuePair<string, Tuple<string, string>> stagTransition in PD.stagTransitions)
            {
                string adjacentScene = PD.GetScene(stagTransition.Value.Item2);

                if (allowBenchWarp && visitedBenches.Contains(adjacentScene)) continue;

                if (Td.pm.Get(stagTransition.Value.Item1) > 0)
                {
                    transitionSpace.Add(stagTransition.Key);
                }
            }

            // Add elevator transitions
            foreach (KeyValuePair<string, Tuple<string, string, string>> elevatorTransition in PD.elevatorTransitions)
            {
                if (Td.pm.Get(elevatorTransition.Value.Item1) > 0)
                {
                    transitionSpace.Add(elevatorTransition.Key);
                }
            }

            // Add tram transitions
            foreach (KeyValuePair<string, Tuple<string, string>> tramTransition in PD.tramTransitions)
            {
                if (allowBenchWarp
                    && (tramTransition.Key.StartsWith("Upper Tram")
                            && visitedBenches.Contains("Room_Tram_RG"))
                        || (tramTransition.Key.StartsWith("Lower Tram")
                            && visitedBenches.Contains("Room_Tram"))) continue;

                if (Td.pm.Get(tramTransition.Value.Item1) > 0)
                {
                    transitionSpace.Add(tramTransition.Key);
                }
            }

            // Add normal warp transitions
            foreach (KeyValuePair<string, Tuple<string, string>> normalWarpTransition in PD.normalWarpTransitions)
            {
                if (Td.pm.Get(normalWarpTransition.Value.Item1) > 0)
                {
                    transitionSpace.Add(normalWarpTransition.Key);
                }
            }

            transitionSpace.Remove(null);

            GetNewItems();

            // Algorithm (BFS)
            HashSet<string> visitedTransitions = new();
            LinkedList<SearchNode> queue = new();

            // Add initial bench warp transitions if setting is enabled
            if (allowBenchWarp && Dependencies.HasDependency("Benchwarp"))
            {
                Dictionary<string, string> startWarp = new() { { "Warp Start", RandomizerMod.RandomizerData.Data.GetStartDef(RM.RS.GenerationSettings.StartLocationSettings.StartLocation).Transition } };

                foreach (KeyValuePair<string, string> warpPair in PD.benchWarpTransitions.Union(startWarp))
                {
                    string scene = PD.GetScene(warpPair.Value);

                    // Remove the single transition rejected routes
                    if (rejectedTransitionPairs.Any(p => p.Key == warpPair.Key && p.Value == warpPair.Key)) continue;

                    if (visitedBenches.Contains(scene)
                        || (warpPair.Key.StartsWith("Warp Upper Tram")
                            && visitedBenches.Contains("Room_Tram_RG"))
                        || (warpPair.Key.StartsWith("Warp Lower Tram")
                            && visitedBenches.Contains("Room_Tram"))
                        || warpPair.Key == "Warp Start")
                    {
                        SearchNode startNode = new(scene, new() { warpPair.Key }, warpPair.Value);
                        queue.AddLast(startNode);
                    }
                }
            }

            // Get other starting transitions
            foreach (string transition in transitionSpace)
            {
                if (PD.GetScene(transition) == startScene
                    || (PD.stagTransitions.ContainsKey(transition)
                        && PD.stagTransitions.Any(t => PD.GetScene(t.Value.Item2) == startScene))
                    || (transition.StartsWith("Upper Tram") && startScene.StartsWith("Crossroads_46"))
                    || (transition.StartsWith("Lower Tram") && startScene.StartsWith("Abyss_03")))
                {
                    string target = PD.GetAdjacentTransition(transition);

                    if (target == null) continue;

                    // Remove the single transition rejected routes
                    if (rejectedTransitionPairs.Any(p => p.Key == transition && p.Value == transition)) continue;

                    SearchNode startNode = new(PD.GetScene(target), new() { transition }, target);
                    queue.AddLast(startNode);
                    visitedTransitions.Add(transition);
                }
            }

            while (queue.Any())
            {
                SearchNode currentNode = queue.First();
                queue.RemoveFirst();

                searchScene = currentNode.currentScene;

                // Avoid going through a rejected path, and remove redudant new paths
                if (currentNode.currentScene == finalScene && !rejectedTransitionPairs.Any(pair => pair.Key == currentNode.currentRoute.First() && PD.GetAdjacentTransition(pair.Value) == PD.GetAdjacentTransition(currentNode.currentRoute.Last())))
                {
                    // No other paths to same final transition with a different starting benchwarp
                    if (rejectedTransitionPairs.Any(pair => PD.GetAdjacentTransition(pair.Value) == PD.GetAdjacentTransition(currentNode.currentRoute.Last()) && (PD.benchWarpTransitions.ContainsKey(pair.Key) || pair.Key == "Warp Start"))) continue;

                    return currentNode.currentRoute;
                }

                localPm.StartTemp();

                if (currentNode.currentRoute.Count != 0 && !ApplyAltLogic(currentNode.currentRoute.Last()))
                {
                    localPm.Add(localPm.lm.GetTransition(currentNode.lastAdjacentTransition));
                }

                AddMissingTransitionsBenchwarp(currentNode.currentRoute.Last());

                candidateReachableTransitions = new(TransitionData.GetTransitionsByScene(searchScene));

                while (UpdateReachableTransitions()) { }

                foreach (string transition in transitionSpace.Where(t => !visitedTransitions.Contains(t)))
                {
                    bool addNode = false;

                    if (reachableTransitions.Contains(transition))
                    {
                        addNode = true;
                    }
                    // Don't take stags/elevators/trams/normal warps twice in a row
                    else if (PD.stagTransitions.ContainsKey(transition))
                    {
                        if (!PD.stagTransitions.ContainsKey(currentNode.currentRoute.Last())
                            && PD.stagTransitions.Any(t => t.Value.Item2.StartsWith(currentNode.currentScene)))
                        {
                            addNode = true;
                        } 
                    }
                    else if (PD.elevatorTransitions.ContainsKey(transition))
                    {
                        if (!PD.elevatorTransitions.ContainsKey(currentNode.currentRoute.Last())
                            && currentNode.currentScene == PD.elevatorTransitions[transition].Item2)
                        {
                            addNode = true;
                        }
                    }
                    else if (PD.tramTransitions.ContainsKey(transition))
                    {
                        if (!PD.tramTransitions.ContainsKey(currentNode.currentRoute.Last())
                            && ((transition.StartsWith("Upper Tram") && currentNode.currentScene.StartsWith("Crossroads_46"))
                                || (transition.StartsWith("Lower Tram") && currentNode.currentScene.StartsWith("Abyss_03"))))
                        {
                            addNode = true;
                        }
                    }
                    else if (PD.normalWarpTransitions.ContainsKey(transition))
                    {
                        if (!PD.normalWarpTransitions.ContainsKey(currentNode.currentRoute.Last())
                            && transition.StartsWith(currentNode.currentScene)
                            && localPm.Get(PD.normalWarpTransitions[transition].Item1) > 0)
                        {
                            addNode = true;
                        }
                    }

                    if (addNode)
                    {
                        string adjacent = PD.GetAdjacentTransition(transition);

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
                reachableTransitions.Clear();
            }

            // No route found, or the parameters are invalid
            return new();
        }

        public void GetNewItems()
        {
            // Get new progression since last time GetNewItems() was called
            foreach (int id in Td.obtainedItems.Union(Td.outOfLogicObtainedItems))
            {
                if (!addedItems.Contains(id))
                {
                    addedItems.Add(id);
                    localPm.Add(RM.RS.Context.itemPlacements[id].Item);
                }
            }

            // Emulate a transition being possibly available via having the required term
            foreach (KeyValuePair<string, string> pair in PD.transitionTermFixes)
            {
                int keyId = localPm.lm.TermLookup[pair.Key];
                int ValueId = localPm.lm.TermLookup[pair.Value];

                if (Td.pm.Has(keyId)
                    && !addedTerms.Contains(ValueId))
                {
                    addedTerms.Add(ValueId);
                    localPm.Set(ValueId, 1);
                }
            }

            // Persistent waypoints should always be added
            foreach (LogicWaypoint waypoint in localPm.lm.Waypoints)
            {
                int id = waypoint.term.Id;

                if (Td.pm.Has(id)
                    && (waypoint.Name.Contains("Rescued_")
                    || waypoint.Name.Contains("Defeated_")
                    || waypoint.Name.Contains("Lever-")
                    || waypoint.Name.Contains("Completed_")
                    //|| waypoint.Name.Contains("Warp-")
                    || waypoint.Name.Contains("_Tram")
                    || waypoint.Name.Contains("_Elevator")
                    || waypoint.Name.Contains("Grimmchild_Upgrade")
                    || waypoint.Name.Contains("Lit_")
                    || waypoint.Name.Contains("_Lit")
                    || waypoint.Name.Contains("Opened_")
                    || waypoint.Name.Contains("_Opened")
                    || waypoint.Name.Contains("Broke_")))
                {
                    if (!addedTerms.Contains(id))
                    {
                        addedTerms.Add(id);
                        localPm.Add(waypoint);
                    }
                }
            }

            // Emulate a transition being possibly available via having the required waypoint
            foreach (KeyValuePair<string, string> pair in PD.waypointFixes)
            {
                Term waypointTerm = localPm.lm.TermLookup[pair.Value];

                if (Td.pm.Get(pair.Key) > 0
                    && !addedTerms.Contains(waypointTerm.Id))
                {
                    addedTerms.Add(waypointTerm.Id);
                    localPm.Add(new LogicWaypoint(waypointTerm, localPm.lm.LogicLookup[pair.Value]));
                }
            }

            // Persistent terms should always be added
            foreach (string term in PD.persistentTerms)
            {
                int id = localPm.lm.TermLookup[term];

                if (Td.pm.Has(id)
                    && !addedTerms.Contains(id))
                {
                    addedTerms.Add(id);
                    localPm.Set(id, 1);
                }
            }

            if (PlayerData.instance.GetBool("mineLiftOpened"))
            {
                localPm.Add(localPm.lm.TransitionLookup["Town[right1]"]);
            }

            //foreach (Term term in pm.lm.TermLookup.Values)
            //{
            //    MapModS.Instance.Log(term.Id + " " + term.Name + " " + pm.Has(term));
            //}
        }

        // Instead of adding the adjacent transition, add the corresponding waypoint instead
        private bool ApplyAltLogic(string transition)
        {
            if (transition == "Warp Waterways")
            {
                localPm.Add(new LogicWaypoint(localPm.lm.TermLookup["Waterways_02"], localPm.lm.LogicLookup["Waterways_02"]));
                return true;
            }

            if (transition == "Warp White Palace Entrace")
            {
                localPm.Add(new LogicWaypoint(localPm.lm.TermLookup["White_Palace_01"], localPm.lm.LogicLookup["White_Palace_01"]));
                return true;
            }

            if (transition == "Warp White Palace Atrium")
            {
                localPm.Add(new LogicWaypoint(localPm.lm.TermLookup["White_Palace_03_hub"], localPm.lm.LogicLookup["White_Palace_03_hub"]));
                return true;
            }

            return false;
        }



        private void AddMissingTransitionsBenchwarp(string lastSourceTransition)
        {
            if (lastSourceTransition == "Warp Colosseum")
            {
                if (localPm.Get("LEFTCLAW") > 0 || localPm.Get("RIGHTCLAW") > 0)
                {
                    reachableTransitions.Add("Room_Colosseum_02[top1]");
                    reachableTransitions.Add("Room_Colosseum_02[top2]");

                    localPm.Add(localPm.lm.TransitionLookup["Room_Colosseum_02[top1]"]);
                    localPm.Add(localPm.lm.TransitionLookup["Room_Colosseum_02[top2]"]);
                }
            }
            else if (lastSourceTransition == "Warp White Palace Balcony")
            {
                if (localPm.Get("RIGHTCLAW") > 0 && (localPm.Get("LEFTCLAW") > 0 || localPm.Get("WINGS") > 0))
                {
                    reachableTransitions.Add("White_Palace_06[top1]");

                    localPm.Add(localPm.lm.TransitionLookup["White_Palace_06[top1]"]);
                }
            }
        }

        // Add other in-logic transitions in the current room
        public bool UpdateReachableTransitions()
        {
            bool continueUpdating = false;

            foreach (string transition in candidateReachableTransitions)
            {
                if (localPm.lm.TransitionLookup[transition].CanGet(localPm) && !reachableTransitions.Contains(transition))
                {
                    reachableTransitions.Add(transition);
                    localPm.Add(localPm.lm.TransitionLookup[transition]);

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

        class SearchNode
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
