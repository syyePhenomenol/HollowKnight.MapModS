using RandomizerCore;
using RandomizerCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapModS.UI
{
    public class TransitionHelper
    {
        private static readonly Dictionary<string, string> transitionTermFixes = new()
        {
            { "Ruins1_31[left3]", "ELEGANT" },
            { "Ruins2_11_b[left1]", "LOVE" }
        };

        private static readonly Dictionary<string, string> waypointFixes = new()
        {
            { "Abyss_01[left1]", "Opened_Dung_Defender_Wall" },
            //{ "Crossroads_09[left1]", "Defeated_Brooding_Mawlek" },
            //{ "Crossroads_09[right1]", "Defeated_Brooding_Mawlek" },
            { "Crossroads_33[left1]", "Opened_Mawlek_Wall" },
            { "Crossroads_33[right1]", "Opened_Shaman_Pillar" },
            { "Deepnest_East_03[left2]", "Opened_Lower_Kingdom's_Edge_Wall" },
            { "Fungus3_02[right1]", "Opened_Archives_Exit_Wall" },
            { "Fungus3_13[left2]", "Opened_Gardens_Stag_Exit" },
            { "RestingGrounds_02[bot1]", "Opened_Resting_Grounds_Floor" },
            { "RestingGrounds_05[right1]", "Opened_Glade_Door" },
            { "Ruins1_05b[bot1]", "Opened_Waterways_Manhole" },
            { "Ruins1_31[left2]", "Lever-Shade_Soul" },
            { "Ruins2_04[door_Ruin_House_03]", "Opened_Emilitia_Door" },
            { "Ruins2_10[right1]", "Opened_Resting_Grounds_Catacombs_Wall" },
            { "Ruins2_10b[left1]", "Opened_Pleasure_House_Wall" },
            { "Waterways_01[top1]", "Opened_Waterways_Manhole" },
            { "Waterways_07[right1]", "Lever-Dung_Defender" }
        };

        private static readonly HashSet<string> persistentTransitions = new()
        {
            { "Town[door_station]" },
            { "Town[door_sly]" },
            { "Town[door_mapper]" },
            { "Town[door_jiji]" },
            { "Town[door_bretta]" },
            { "Town[room_divine]" },
            { "Town[room_grimm]" },
            { "Crossroads_09[left1]" },
            { "Crossroads_09[right1]" }
        };

        // Pair of bench warp instruction + logically equivalent transition
        public static Dictionary<string, string> benchWarpTransitions = new()
        {
            { "Warp Dirtmouth", "Town[bot1]" },
            { "Warp Mato", "Room_nailmaster[left1]" },
            { "Warp Crossroads Hot Springs", "Crossroads_30[left1]" },
            { "Warp Crossroads Stag", "Crossroads_47[right1]" },
            { "Warp Salubra", "Crossroads_04[door_charmshop]" },
            { "Warp Ancestral Mound", "Crossroads_ShamanTemple[left1]" },
            { "Warp Black Egg Temple", "Room_temple[left1]" },
            { "Warp Waterfall", "Fungus1_01b[left1]" },
            { "Warp Stone Sanctuary", "Fungus1_37[left1]" },
            { "Warp Greenpath Toll", "Fungus1_31[top1]" },
            { "Warp Greenpath Stag", "Fungus1_16_alt[right1]" },
            { "Warp Lake of Unn", "Room_Slug_Shrine[left1]" },
            { "Warp Sheo", "Fungus1_15[door1]" },
            { "Warp Archives", "Fungus3_archive[left1]" },
            { "Warp Queens Station", "Fungus2_02[right1]" },
            { "Warp Leg Eater", "Fungus2_26[left1]" },
            { "Warp Bretta", "Fungus2_13[left2]" },
            { "Warp Mantis Village", "Fungus2_31[left1]" },
            { "Warp Quirrel", "Ruins1_02[top1]" },
            { "Warp City Toll", "Ruins1_31[left1]" },
            { "Warp City Storerooms", "Ruins1_29[left1]" },
            { "Warp Watcher's Spire", "Ruins1_18[right2]" },
            { "Warp King's Station", "Ruins2_08[left1]" },
            { "Warp Pleasure House", "Ruins_Bathhouse[door1]" },
            // Need to handle logic from here in a special manner. Going to leave it out for now...
            { "Warp Waterways?", "Waterways_02[top1]" },
            { "Warp Deepnest Hot Springs", "Deepnest_30[left1]" },
            { "Warp Failed Tramway", "Deepnest_14[left1]" },
            { "Warp Beast's Den", "Deepnest_Spider_Town[left1]" },
            { "Warp Ancient Basin Toll", "Abyss_18[right1]" },
            { "Warp Hidden Station", "Abyss_22[left1]" },
            { "Warp Oro", "Deepnest_East_06[door1]" },
            { "Warp Kingdom's Edge Camp", "Deepnest_East_13[bot1]" },
            // Probably needs its own special waypoint too, if player falls in from top and has no claw or wings
            { "Warp Colosseum?", "Room_Colosseum_02[top1]" },
            { "Warp Hive", "Hive_01[right2]" },
            { "Warp Crystal Peak Dark Room", "Mines_29[left1]" },
            { "Warp Crystal Guardian", "Mines_18[left1]" },
            { "Warp Resting Grounds Stag", "RestingGrounds_09[left1]" },
            { "Warp Grey Mourner", "RestingGrounds_12[door_Mansion]" },
            { "Warp Queen's Gardens Cornifer", "Fungus1_24[left1]" },
            { "Warp Queen's Gardens Toll", "Fungus3_50[right1]" },
            { "Warp Queen's Gardens Stag", "Fungus3_40[right1]" },
            // Special waypoint needed
            { "Warp White Palace Entrance", "White_Palace_01[left1]" },
            // Special waypoint needed
            { "Warp White Palace Atrium", "White_Palace_03_hub[right1]" },
            { "Warp White Palace Balcony", "White_Palace_06[top1]" },
            { "Warp Upper Tram -> Exit Left", "Crossroads_46[left1]" },
            { "Warp Upper Tram -> Exit Right", "Crossroads_46b[right1]" },
            { "Warp Lower Tram -> Exit Left", "Abyss_03_b[left1]" },
            { "Warp Lower Tram -> Exit Middle", "Abyss_03[top1]" },
            { "Warp Lower Tram -> Exit Right", "Abyss_03_c[right1]" }
        };

        public static Dictionary<string, Tuple<string, string>> stagTransitions = new()
        {
            { "Stag Dirtmouth", new("Dirtmouth_Stag", "Room_Town_Stag_Station[left1]") },
            { "Stag Crossroads", new("Crossroads_Stag", "Crossroads_47[right1]") },
            { "Stag Greenpath", new("Greenpath_Stag", "Fungus1_16_alt[right1]") },
            { "Stag Queen's Station", new("Queen's_Station_Stag", "Fungus2_02[right1]") },
            { "Stag Distant Village", new("Distant_Village_Stag", "Deepnest_09[left1]") },
            { "Stag Hidden Station", new("Hidden_Station_Stag", "Abyss_22[left1]") },
            { "Stag City Storerooms", new("City_Storerooms_Stag", "Ruins1_29[left1]") },
            { "Stag King's Station", new("King's_Station_Stag", "Ruins2_08[left1]") },
            { "Stag Resting Grounds", new("Resting_Grounds_Stag", "RestingGrounds_09[left1]") },
            { "Stag Queen's Gardens", new("Queen's_Gardens_Stag", "Fungus3_40[right1]") },
            { "Stag Stag Nest", new("Stag_Nest_Stag", "Cliffs_03[right1]") }
        };

        // Tuple is (waypoint requirement, scene requirement, adjacent transition)
        public static Dictionary<string, Tuple<string, string, string>> elevatorTransitions = new()
        {
            { "Left Elevator Up", new("Left_Elevator", "Crossroads_49b", "Crossroads_49[right1]") },
            { "Left Elevator Down", new("Left_Elevator", "Crossroads_49", "Crossroads_49b[right1]") },
            { "Right Elevator Up", new("Right_Elevator", "Ruins2_10b", "Ruins2_10[left1]") },
            { "Right Elevator Down", new("Right_Elevator", "Ruins2_10", "Ruins2_10b[right2]") }
        };

        public static Dictionary<string, Tuple<string, string>> tramTransitions = new()
        {
            { "Upper Tram -> Exit Left", new("Upper_Tram", "Crossroads_46[left1]") },
            { "Upper Tram -> Exit Right", new("Upper_Tram", "Crossroads_46b[right1]") },
            { "Lower Tram -> Exit Left", new("Lower_Tram", "Abyss_03_b[left1]") },
            { "Lower Tram -> Exit Middle", new("Lower_Tram", "Abyss_03[top1]") },
            { "Lower Tram -> Exit Right", new("Lower_Tram", "Abyss_03_c[right1]") }
        };

        public static Dictionary<string, Tuple<string, string>> normalWarpTransitions = new()
        {
            { "Abyss_08[warp]", new("Warp-Lifeblood_Core_to_Abyss", "Abyss_06_Core[left1]") },
            { "Abyss_05[warp]", new("Warp-Palace_Grounds_to_White_Palace", "White_Palace_11[door2]") },
            { "White_Palace_11[warp]", new("Warp-White_Palace_Entrance_to_Palace_Grounds", "Abyss_05[left1]") },
            { "White_Palace_03_hub[warp]", new("Warp-White_Palace_Atrium_to_Palace_Grounds", "Abyss_05[left1]") },
            { "White_Palace_20[warp]", new("Warp-Path_of_Pain_Complete", "White_Palace_06[bot1]") }
        };

        public TransitionHelper()
        {
            tt = new();
        }

        // Code based on homothety's implementation of RandomizerMod's TrackerData
        class TransitionTracker
        {
            public readonly HashSet<int> addedItems = new();
            public readonly HashSet<int> addedTerms = new();

            public ProgressionManager pm;
            private readonly MainUpdater mu;

            public TransitionTracker()
            {
                pm = new(RandomizerMod.RandomizerMod.RS.Context.LM, RandomizerMod.RandomizerMod.RS.Context);

                mu = new(pm.lm);

                mu.AddEntries(pm.lm.Waypoints.Select(w => new DelegateUpdateEntry(w, pm => { pm.Add(w); }, pm.lm)));

                if (RandomizerMod.RandomizerMod.RS.Context.transitionPlacements != null)
                {
                    mu.AddEntries(RandomizerMod.RandomizerMod.RS.Context.transitionPlacements.Select((p, id) => new DelegateUpdateEntry(p.Source, pm =>
                    {
                        (RandoTransition source, RandoTransition target) = RandomizerMod.RandomizerMod.RS.Context.transitionPlacements[id];

                        if (!pm.Has(source.lt.term))
                        {
                            pm.Add(source);
                        }
                    }, pm.lm)));
                }

                mu.Hook(pm);
            }

            public void GetNewItems()
            {
                // Get new progression since last time GetNewItems() was called
                foreach (int id in RandomizerMod.RandomizerMod.RS.TrackerData.obtainedItems.Union(RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicObtainedItems))
                {
                    if (!addedItems.Contains(id))
                    {
                        // MapModS.Instance.Log("Adding " + pm.ctx.itemPlacements[id].item.Name + " with id " + id);
                        addedItems.Add(id);
                        pm.Add(RandomizerMod.RandomizerMod.RS.Context.itemPlacements[id].Item);
                    }
                }

                // Emulate a transition being possibly available via having the required term
                foreach (KeyValuePair<string, string> pair in transitionTermFixes)
                {
                    int keyId = pm.lm.TermLookup[pair.Key];
                    int ValueId = pm.lm.TermLookup[pair.Value];

                    if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(keyId)
                        && !addedTerms.Contains(ValueId))
                    {
                        addedTerms.Add(ValueId);
                        pm.Set(ValueId, 1);
                    }
                }

                // Persistent waypoints should always be added
                foreach (LogicWaypoint waypoint in pm.lm.Waypoints)
                {
                    int id = waypoint.term.Id;

                    if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(id)
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
                            // MapModS.Instance.Log("Has " + waypoint.Name + " with id " + waypoint.term.Id);

                            addedTerms.Add(id);
                            pm.Add(waypoint);
                        }
                    }
                }

                // Emulate a transition being possibly available via having the required waypoint
                foreach (KeyValuePair<string, string> pair in waypointFixes)
                {
                    Term waypointTerm = pm.lm.TermLookup[pair.Value];

                    if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Get(pair.Key) > 0
                        && !addedTerms.Contains(waypointTerm.Id))
                    {
                        // MapModS.Instance.Log("Adding " + pair.Value + " with id " + waypointTerm.Id);

                        addedTerms.Add(waypointTerm.Id);
                        pm.Add(new LogicWaypoint(waypointTerm, pm.lm.LogicLookup[pair.Value]));
                    }
                }

                // Persistent transitions should always be added
                foreach (string transition in persistentTransitions)
                {
                    int id = pm.lm.TermLookup[transition];

                    if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(id)
                        && !addedTerms.Contains(id))
                    {
                        //MapModS.Instance.Log("Persistent transition: " + transition);

                        addedTerms.Add(id);
                        pm.Add(pm.lm.TransitionLookup[transition]);
                    }
                }

                if (PlayerData.instance.mineLiftOpened)
                {
                    pm.Add(pm.lm.TransitionLookup["Town[right1]"]);
                }
            }

            public class DelegateUpdateEntry : UpdateEntry
            {
                readonly Action<ProgressionManager> onAdd;
                readonly ILogicDef location;
                readonly LogicManager lm;

                public DelegateUpdateEntry(ILogicDef location, Action<ProgressionManager> onAdd, LogicManager lm)
                {
                    this.location = location;
                    this.onAdd = onAdd;
                    this.lm = lm;
                }

                // Lazy evaluation for transitions
                public override bool CanGet(ProgressionManager pm)
                {
                    if (lm.TransitionLookup.ContainsKey(location.Name))
                    {
                        if (RandomizerMod.RandomizerData.Data.GetTransitionDef(location.Name).SceneName != searchScene) return false;
                    }

                    return location.CanGet(pm);
                }

                public override IEnumerable<Term> GetTerms()
                {
                    return location.GetTerms();
                }

                public override void OnAdd(ProgressionManager pm)
                {
                    onAdd?.Invoke(pm);
                }

                public override void OnRemove(ProgressionManager pm) { }
            }
        }

        public static string GetScene(string transition)
        {
            if (benchWarpTransitions.ContainsKey(transition))
            {
                return GetScene(benchWarpTransitions[transition]);
            }

            // Handle in a special way
            if (stagTransitions.ContainsKey(transition)) return null;

            if (elevatorTransitions.ContainsKey(transition))
            {
                return elevatorTransitions[transition].Item2;
            }

            // Handle in a special way
            if (tramTransitions.ContainsKey(transition)) return null;

            if (normalWarpTransitions.ContainsKey(transition))
            {
                return transition.Substring(0, transition.Length - 6);
            }

            return RandomizerMod.RandomizerData.Data.GetTransitionDef(transition).SceneName;
        }

        public static string GetAdjacentTransition(string source)
        {
            if (source == "Warp Start")
            {
                return RandomizerMod.RandomizerData.Data.GetStartDef(RandomizerMod.RandomizerMod.RS.GenerationSettings.StartLocationSettings.StartLocation).Transition;
            }

            if (transitionPlacementsDict.ContainsKey(source))
            {
                return transitionPlacementsDict[source];
            }

            if (benchWarpTransitions.ContainsKey(source))
            {
                return benchWarpTransitions[source];
            }

            if (stagTransitions.ContainsKey(source))
            {
                return stagTransitions[source].Item2;
            }

            if (elevatorTransitions.ContainsKey(source))
            {
                return elevatorTransitions[source].Item3;
            }

            if (tramTransitions.ContainsKey(source))
            {
                return tramTransitions[source].Item2;
            }

            if (normalWarpTransitions.ContainsKey(source))
            {
                return normalWarpTransitions[source].Item2;
            }

            RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(source);

            if (transitionDef == null) return null;

            // If it's not in TransitionPlacements, it's the vanilla target.
            // NOTE that this can be null
            return transitionDef.VanillaTarget;
        }

        private bool ApplyAltLogic(string transition)
        {
            if (transition == "Warp White Palace Entrace")
            {
                tt.pm.Add(new LogicWaypoint(tt.pm.lm.TermLookup["White_Palace_01"], tt.pm.lm.LogicLookup["White_Palace_01"]));
                return true;
            }

            if (transition == "Warp White Palace Atrium")
            {
                tt.pm.Add(new LogicWaypoint(tt.pm.lm.TermLookup["White_Palace_03_hub"], tt.pm.lm.LogicLookup["White_Palace_03_hub"]));
                return true;
            }

            return false;
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

        private readonly TransitionTracker tt;
        private static Dictionary<string, string> transitionPlacementsDict = new();
        private static string searchScene;

        // Calculates the shortest route (by number of transitions) from startScene to finalScene.
        // The search space will be purely limited to rooms that have been visited + unreached reachable locations
        // A ProgressionManager is used to track logic while traversing through the search space
        public List<string> ShortestRoute(string startScene, string finalScene, HashSet<KeyValuePair<string, string>> rejectedTransitionPairs, bool allowBenchWarp)
        {
            IEnumerable<string> visitedBenches = Dependencies.GetVisitedBenchScenes();

            transitionPlacementsDict = RandomizerMod.RandomizerMod.RS.Context.transitionPlacements.ToDictionary(tp => tp.Source.Name, tp => tp.Target.Name);

            HashSet<string> transitionSpace = new();
            Dictionary<string, string> startTransitionPlacements = new();

            // Get all relevant transitions
            foreach (KeyValuePair<string, LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
            {
                string scene = GetScene(transitionEntry.Key);

                if (RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableTransitions.Contains(transitionEntry.Key)) continue;

                if (MapModS.LS.mapMode == Settings.MapMode.TransitionRandoAlt
                    && !PlayerData.instance.scenesVisited.Contains(scene)) continue;

                //MapModS.Instance.Log(transitionEntry.Key);

                if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(transitionEntry.Value.term.Id))
                {
                    //MapModS.Instance.Log("Search space includes: " + transitionEntry.Key);
                    transitionSpace.Add(transitionEntry.Key);
                }
            }

            // Add stag transitions
            foreach (KeyValuePair<string, Tuple<string, string>> stagTransition in stagTransitions)
            {
                string adjacentScene = GetScene(stagTransition.Value.Item2);

                if (allowBenchWarp && visitedBenches.Contains(adjacentScene)) continue;

                if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Get(stagTransition.Value.Item1) > 0)
                {
                    //MapModS.Instance.Log("Adding " + stagTransition.Key);
                    transitionSpace.Add(stagTransition.Key);
                }
            }

            // Add elevator transitions
            foreach (KeyValuePair<string, Tuple<string, string, string>> elevatorTransition in elevatorTransitions)
            {
                if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Get(elevatorTransition.Value.Item1) > 0)
                {
                    //MapModS.Instance.Log("Adding " + elevatorTransition.Key);
                    transitionSpace.Add(elevatorTransition.Key);
                }
            }

            // Add tram transitions
            foreach (KeyValuePair<string, Tuple<string, string>> tramTransition in tramTransitions)
            {
                if (allowBenchWarp
                    && (tramTransition.Key.StartsWith("Upper Tram")
                            && visitedBenches.Contains("Room_Tram_RG"))
                        || (tramTransition.Key.StartsWith("Lower Tram")
                            && visitedBenches.Contains("Room_Tram"))) continue;

                if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Get(tramTransition.Value.Item1) > 0)
                {
                    //MapModS.Instance.Log("Adding " + tramTransition.Key);
                    transitionSpace.Add(tramTransition.Key);
                }
            }

            // Add normal warp transitions (check if reachable during algorithm)
            foreach (KeyValuePair<string, Tuple<string, string>> normalWarpTransition in normalWarpTransitions)
            {
                if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Get(normalWarpTransition.Value.Item1) > 0)
                {
                    transitionSpace.Add(normalWarpTransition.Key);
                }
            }

            // Just in case
            if (startScene == null || finalScene == null)
                //|| startScene == finalScene)
            {
                return new();
            }

            //MapModS.Instance.Log($"Start scene: {startScene}");
            //MapModS.Instance.Log($"Final scene: {finalScene}");

            // Just in case
            transitionSpace.Remove(null);

            tt.GetNewItems();

            // Algorithm (BFS)
            HashSet<string> visitedTransitions = new();
            LinkedList<SearchNode> queue = new();

            // Add initial bench warp transitions if setting is enabled
            if (allowBenchWarp)
            {
                Dictionary<string, string> startWarp = new() { { "Warp Start", RandomizerMod.RandomizerData.Data.GetStartDef(RandomizerMod.RandomizerMod.RS.GenerationSettings.StartLocationSettings.StartLocation).Transition } };

                foreach (KeyValuePair<string, string> warpPair in benchWarpTransitions.Union(startWarp))
                {
                    string scene = GetScene(warpPair.Value);

                    // Remove the single transition rejected routes
                    if (rejectedTransitionPairs.Any(p => p.Key == warpPair.Key && p.Value == warpPair.Key)) continue;

                    if (visitedBenches.Contains(scene)
                        || (warpPair.Key.StartsWith("Warp Upper Tram")
                            && visitedBenches.Contains("Room_Tram_RG")
                        || (warpPair.Key.StartsWith("Warp Lower Tram")
                            && visitedBenches.Contains("Room_Tram")
                        || warpPair.Key == "Warp Start")))
                    {
                        SearchNode startNode = new(scene, new() { warpPair.Key }, warpPair.Value);
                        queue.AddLast(startNode);
                    }
                }
            }

            // Get other starting transitions
            foreach (string transition in transitionSpace)
            {
                if (GetScene(transition) == startScene
                    || (stagTransitions.ContainsKey(transition)
                        && stagTransitions.Any(t => GetScene(t.Value.Item2) == startScene))
                    || (transition.StartsWith("Upper Tram") && startScene.StartsWith("Crossroads_46"))
                    || (transition.StartsWith("Lower Tram") && startScene.StartsWith("Abyss_03")))
                {
                    string target = GetAdjacentTransition(transition);

                    if (target == null) continue;

                    // Remove the single transition rejected routes
                    if (rejectedTransitionPairs.Any(p => p.Key == transition && p.Value == transition)) continue;

                    SearchNode startNode = new(GetScene(target), new() { transition }, target);
                    queue.AddLast(startNode);
                    visitedTransitions.Add(transition);
                }
            }

            while (queue.Any())
            {
                SearchNode currentNode = queue.First();
                queue.RemoveFirst();

                searchScene = currentNode.currentScene;

                //currentNode.PrintRoute();

                if (currentNode.currentScene == finalScene) return currentNode.currentRoute;

                tt.pm.StartTemp();

                if (currentNode.currentRoute.Count != 0 && !ApplyAltLogic(currentNode.currentRoute.Last()))
                {
                    tt.pm.Add(tt.pm.lm.GetTransition(currentNode.lastAdjacentTransition));
                }

                foreach (string transition in transitionSpace)
                {
                    if (GetScene(transition) != currentNode.currentScene
                        && !stagTransitions.ContainsKey(transition)
                        && !elevatorTransitions.ContainsKey(transition)
                        && !tramTransitions.ContainsKey(transition)) continue;

                    if (GetAdjacentTransition(transition) != null
                        && !visitedTransitions.Contains(transition)
                        && !rejectedTransitionPairs.Any(pair => pair.Key == currentNode.currentRoute.First() && pair.Value == transition)
                        && ((tt.pm.lm.TransitionLookup.ContainsKey(transition)
                                && tt.pm.lm.TransitionLookup[transition].CanGet(tt.pm))
                            || (VerifySpecialTransition(transition, currentNode.currentScene)
                                && CanGetSpecialTransition(transition))))
                    {
                        SearchNode newNode = new(GetScene(GetAdjacentTransition(transition)), currentNode.currentRoute, GetAdjacentTransition(transition));
                        newNode.currentRoute.Add(transition);
                        visitedTransitions.Add(transition);
                        queue.AddLast(newNode);
                    }
                }

                tt.pm.RemoveTempItems();
            }

            // No route found, or the parameters are invalid
            return new();
        }

        public static bool IsSpecialTransition(string transition)
        {
            return transition.StartsWith("Warp")
                || transition.StartsWith("Stag")
                || transition.StartsWith("Left Elevator")
                || transition.StartsWith("Right Elevator")
                || transition.StartsWith("Upper Tram")
                || transition.StartsWith("Lower Tram")
                || transition.EndsWith("[warp]");
        }

        // Checks if the player has transitioned into a scene with the special transition
        public static bool VerifySpecialTransition(string transition, string currentScene)
        {
            if (transition.StartsWith("Stag") && stagTransitions.Any(t => t.Value.Item2.StartsWith(currentScene))) return true;

            if ((transition.StartsWith("Left Elevator") || transition.StartsWith("Right Elevator"))
                && currentScene == elevatorTransitions[transition].Item2) return true;

            if ((transition.StartsWith("Upper Tram") && currentScene.StartsWith("Crossroads_46"))
                || (transition.StartsWith("Lower Tram") && currentScene.StartsWith("Abyss_03"))) return true;

            if (transition.EndsWith("[warp]") && transition.StartsWith(currentScene)) return true;

            return false;
        }

        // Checks if the player can get the transition
        public bool CanGetSpecialTransition(string transition)
        {
            if (transition.EndsWith("[warp]")) return (tt.pm.Get(normalWarpTransitions[transition].Item1) > 0);

            // Others are true because they have been added to the search space based on being available
            return true;
        }
    }
}
