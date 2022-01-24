using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerMod.Settings;

namespace MapModS.UI
{
    public class TransitionHelper
    {
        private static readonly Dictionary<string, string> itemFixes = new()
        {
            { "Ruins1_31[left3]", "ELEGANT" },
            { "Ruins2_11_b[left1]", "LOVE" }
        };

        private static readonly Dictionary<string, string> waypointFixes = new()
        {
            { "Abyss_01[left1]", "Opened_Dung_Defender_Wall" },
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
            { "Waterways_07[right1]", "Lever-Dung_Defender" },
        };

        private static readonly HashSet<string> persistentTransitions = new()
        {
            { "Town[door_station]" },
            { "Town[door_sly]" },
            { "Town[door_mapper]" },
            { "Town[door_jiji]" },
            { "Town[door_bretta]" },
            { "Town[room_divine]" },
            { "Town[room_grimm]" }
        };

        // Pair of bench warp instruction + logically equivalent transition
        private static Dictionary<string, string> warpTransitions = new()
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
            //{ "Warp Waterways", "Waterways_02" },
            { "Warp Deepnest Hot Springs", "Deepnest_30[left1]" },
            { "Warp Failed Tramway", "Deepnest_14[left1]" },
            { "Warp Beast's Den", "Deepnest_Spider_Town[left1]" },
            { "Warp Ancient Basin Toll", "Abyss_18[right1]" },
            { "Warp Hidden Station", "Abyss_22[left1]" },
            { "Warp Oro", "Deepnest_East_06[door1]" },
            { "Warp Kingdom's Edge Camp", "Deepnest_East_13[bot1]" },
            // Probably needs its own special waypoint too, if player falls in from top and has no claw or wings
            //{ "Warp Colosseum", "Room_Colosseum_02[top1]" },
            { "Warp Hive", "Hive_01[right2]" },
            { "Warp Crystal Peak Dark Room", "Mines_29[left1]" },
            { "Warp Crystal Guardian", "Mines_18[left1]" },
            { "Warp Resting Grounds Stag", "RestingGrounds_09[left1]" },
            { "Warp Grey Mourner", "RestingGrounds_12[door_Mansion]" },
            { "Warp Queen's Gardens Cornifer", "Fungus1_24[left1]" },
            { "Warp Queen's Gardens Toll", "Fungus3_50[right1]" },
            { "Warp Queen's Gardens Stag", "Fungus3_40[right1]" },
            // Special waypoint needed
            //{ "Warp White Palace Entrance", "White_Palace_01" },
            { "Warp White Palace Atrium", "White_Palace_03_hub[right1]" },
            { "Warp White Palace Balcony", "White_Palace_06[top1]" },
            { "Warp Upper Tram -> Exit Left", "Crossroads_46[left1]" },
            { "Warp Upper Tram -> Exit Right", "Crossroads_46b[right1]" },
            { "Warp Lower Tram -> Exit Left", "Abyss_03_b[left1]" },
            { "Warp Lower Tram -> Exit Middle", "Abyss_03[top1]" },
            { "Warp Lower Tram -> Exit Right", "Abyss_03_c[right1]" },
        };

        //private static Dictionary<string, string> nonBenchStags
        //{

        //};

        public TransitionHelper()
        {
            tt = new();
        }

        // Code based on homothety's implementation of RandomizerMod's TrackerData
        class TransitionTracker
        {
            public readonly HashSet<int> addedItems = new();

            public ProgressionManager pm;
            private readonly MainUpdater mu;

            public TransitionTracker()
            {
                pm = new(RandomizerMod.RandomizerMod.RS.Context.LM, RandomizerMod.RandomizerMod.RS.Context);

                mu = new(pm.lm);

                mu.AddEntries(pm.lm.Waypoints.Select(w => new DelegateUpdateEntry(w, pm => { pm.Add(w); }, pm.lm)));

                if (pm.ctx.transitionPlacements != null)
                {
                    mu.AddEntries(pm.ctx.transitionPlacements.Select((p, id) => new DelegateUpdateEntry(p.source, pm =>
                    {
                        (RandoTransition source, RandoTransition target) = pm.ctx.transitionPlacements[id];

                        if (!pm.Has(source.lt.term))
                        {
                            pm.Add(source);
                        }
                    }, pm.lm)));
                }

                mu.Hook(pm);
            }

            //public void Unhook()
            //{
            //    pm.AfterAddItem -= mu.EnqueueUpdates;
            //    pm.AfterAddRange -= mu.EnqueueUpdates;
            //    pm.AfterEndTemp -= mu.OnEndTemp;
            //    pm.OnRemove -= mu.OnRemove;
            //    pm.AfterRemove -= mu.DoRecalculate;
            //}

            public void GetNewItems()
            {
                //Unhook();

                // Get new progression since last time GetNewItems() was called
                foreach (int id in RandomizerMod.RandomizerMod.RS.TrackerData.obtainedItems.Union(RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicObtainedItems))
                {
                    if (!addedItems.Contains(id))
                    {
                        addedItems.Add(id);
                        pm.Add(pm.ctx.itemPlacements[id].item);
                    }
                }

                // Emulate a transition being possibly available via having the required item
                foreach (KeyValuePair<string, string> pair in itemFixes)
                {
                    int keyId = pm.lm.TermLookup[pair.Key];
                    int ValueId = pm.lm.TermLookup[pair.Value];

                    if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(keyId)
                        && !addedItems.Contains(ValueId))
                    {
                        addedItems.Add(ValueId);
                        pm.Add(pm.ctx.itemPlacements[keyId].item);
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
                        || waypoint.Name.Contains("Warp-")
                        || waypoint.Name.Contains("_Tram")
                        || waypoint.Name.Contains("_Elevator")
                        || waypoint.Name.Contains("Grimmchild_Upgrade")
                        || waypoint.Name.Contains("Lit_")
                        || waypoint.Name.Contains("_Lit")
                        || waypoint.Name.Contains("Opened_")
                        || waypoint.Name.Contains("_Opened")
                        || waypoint.Name.Contains("Broke_")))
                    {
                        if (!addedItems.Contains(id))
                        {
                            addedItems.Add(id);
                            pm.Add(waypoint);
                        }
                    }
                }

                // Emulate a transition being possibly available via having the required waypoint
                foreach (KeyValuePair<string, string> pair in waypointFixes)
                {
                    int keyId = pm.lm.TermLookup[pair.Key];
                    int ValueId = pm.lm.TermLookup[pair.Value];

                    if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(keyId)
                        && !addedItems.Contains(ValueId))
                    {
                        addedItems.Add(ValueId);
                        pm.Add(new LogicWaypoint(pm.lm.TermLookup[pair.Value], pm.lm.LogicLookup[pair.Value]));
                    }
                }

                // Persistent transitions should always be added
                foreach (string transition in persistentTransitions)
                {
                    int id = pm.lm.TermLookup[transition];

                    if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(id)
                        && !addedItems.Contains(id))
                    {
                        addedItems.Add(id);
                        pm.Add(pm.lm.TransitionLookup[transition]);
                    }
                }

                //mu.Hook(pm);
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
                        if (lm.GetTransition(location.Name).data.SceneName != searchScene) return false;
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

        private static string GetScene(string transition)
        {
            return RandomizerMod.RandomizerData.Data.GetTransitionDef(transition).SceneName;
        }

        private static string GetAdjacentTransition(string source)
        {
            if (transitionPlacementsDict.ContainsKey(source))
            {
                return transitionPlacementsDict[source];
            }

            RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(source);

            if (transitionDef == null) return null;

            // If it's not in TransitionPlacements, it's the vanilla target.
            // NOTE that this can be null
            return transitionDef.VanillaTarget;
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
            if (!warpTransitions.ContainsKey("Warp Start"))
            {
                warpTransitions.Add("Warp Start", RandomizerMod.RandomizerData.Data.GetStartDef(RandomizerMod.RandomizerMod.RS.GenerationSettings.StartLocationSettings.StartLocation).Transition);
            }

            transitionPlacementsDict = RandomizerMod.RandomizerMod.RS.Context.transitionPlacements.ToDictionary(tp => tp.source.Name, tp => tp.target.Name);

            HashSet<string> transitionSpace = new();
            Dictionary<string, string> startTransitionPlacements = new();

            // Get all relevant transitions
            foreach (KeyValuePair<string, LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
            {
                if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(transitionEntry.Value.term.Id))
                {
                    transitionSpace.Add(transitionEntry.Key);
                }
            }

            // Remove unchecked reachable transitions to avoid spoilers
            foreach (var transition in RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableTransitions)
            {
                transitionSpace.Remove(transition);
            }

            // Just in case
            if (startScene == null || finalScene == null
                || startScene == finalScene)
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
                foreach (KeyValuePair<string, string> warpPair in warpTransitions)
                {
                    string scene = GetScene(warpPair.Value);

                    if ((Benchwarp.Benchwarp.LS.visitedBenchScenes.ContainsKey(scene) &&
                        Benchwarp.Benchwarp.LS.visitedBenchScenes[scene])
                        || (warpPair.Key.StartsWith("Warp Upper Tram")
                            && Benchwarp.Benchwarp.LS.visitedBenchScenes.ContainsKey("Room_Tram_RG")
                            && Benchwarp.Benchwarp.LS.visitedBenchScenes["Room_Tram_RG"])
                        || (warpPair.Key.StartsWith("Warp Lower Tram")
                            && Benchwarp.Benchwarp.LS.visitedBenchScenes.ContainsKey("Room_Tram")
                            && Benchwarp.Benchwarp.LS.visitedBenchScenes["Room_Tram"])
                        || (warpPair.Key == "Warp Start"))
                    {
                        SearchNode startNode = new(scene, new() { warpPair.Key }, warpPair.Value);
                        queue.AddLast(startNode);
                    }
                }
            }

            // Get starting transitions
            foreach (string transition in transitionSpace)
            {
                if (GetScene(transition) == startScene)
                {
                    string target = GetAdjacentTransition(transition);

                    if (target == null) continue;

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

                foreach (string transition in transitionSpace)
                {
                    if (GetScene(transition) != currentNode.currentScene) continue;

                    tt.pm.StartTemp();

                    if (currentNode.currentRoute.Count != 0)
                    {
                        tt.pm.Add(tt.pm.lm.GetTransition(currentNode.lastAdjacentTransition));
                    }

                    if (GetAdjacentTransition(transition) != null
                        && !visitedTransitions.Contains(transition)
                        && tt.pm.lm.TransitionLookup[transition].CanGet(tt.pm)
                        && !rejectedTransitionPairs.Any(pair => pair.Key == currentNode.currentRoute.First() && pair.Value == transition))
                    {
                        SearchNode newNode = new(GetScene(GetAdjacentTransition(transition)), currentNode.currentRoute, GetAdjacentTransition(transition));
                        newNode.currentRoute.Add(transition);
                        visitedTransitions.Add(transition);
                        queue.AddLast(newNode);
                    }

                    tt.pm.RemoveTempItems();
                }
            }

            // No route found, or the parameters are invalid
            return new();
        }
    }
}
