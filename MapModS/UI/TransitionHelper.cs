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
        //// if Key is true, then Has(Value) should be true for the TransitionTracker to think CanGet(Key) is true
        //private static readonly Dictionary<string, string> waypointLogicPairs = new()
        //{
        //    { "Abyss_01[left1]", "Opened_Dung_Defender_Wall" },
        //    { "Crossroads_09[left1]", "Defeated_Brooding_Mawlek" },
        //    { "Crossroads_09[right1]", "Defeated_Brooding_Mawlek" },
        //    { "Crossroads_33[left1]", "Opened_Mawlek_Wall" },
        //    { "Crossroads_33[right1]", "Opened_Shaman_Pillar" },
        //    { "Deepnest_East_03[left2]", "Opened_Lower_Kingdom's_Edge_Wall" },
        //    { "Fungus3_02[right1]", "Opened_Archives_Exit_Wall" },
        //    { "Fungus3_13[left2]", "Opened_Gardens_Stag_Exit" },
        //    { "RestingGrounds_02[bot1]", "Opened_Resting_Grounds_Floor" },
        //    { "RestingGrounds_05[right1]", "Opened_Glade_Door" },
        //    { "Ruins1_05b[bot1]", "Opened_Waterways_Manhole" },
        //    { "Ruins1_31[left2]", "Lever-Shade_Soul" },
        //    { "Ruins2_04[door_Ruin_House_03]", "Opened_Emilitia_Door" },
        //    { "Ruins2_10[right1]", "Right_Elevator" },
        //    { "Ruins2_10[right1]", "Opened_Resting_Grounds_Catacombs_Wall" },
        //    { "Ruins2_10b[left1]", "Opened_Pleasure_House_Wall" },
        //    { "Ruins2_11_b[left1]", "LOVE" },
        //    { "Town[door_station]", "Dirtmouth_Stag" },
        //    { "Town[door_sly]", "Rescued_Sly" },
        //    { "Town[door_mapper]", "Town" },
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

            public void GetNewItems()
            {
                // Get new progression since last time GetNewItems() was called
                foreach (int id in RandomizerMod.RandomizerMod.RS.TrackerData.obtainedItems.Union(RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicObtainedItems))
                {
                    if (!addedItems.Contains(id))
                    {
                        addedItems.Add(id);
                        pm.Add(pm.ctx.itemPlacements[id].item);
                    }
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
            public SearchNode(string scene, List<string> route)
            {
                currentScene = scene;
                currentRoute = new(route);
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
        }

        private readonly TransitionTracker tt;
        private static Dictionary<string, string> transitionPlacementsDict = new();
        private static string searchScene;

        // Calculates the shortest route (by number of transitions) from startScene to finalScene.
        // The search space will be purely limited to rooms that have been visited + unreached reachable locations
        // A ProgressionManager is used to track logic while traversing through the search space
        public List<string> ShortestRoute(string startScene, string finalScene, HashSet<KeyValuePair<string, string>> rejectedTransitionPairs, bool allowBenchWarp)
        {   
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

            // Get starting transitions
            foreach (string transition in transitionSpace)
            {
                if (GetScene(transition) == startScene)
                {
                    string target = GetAdjacentTransition(transition);

                    if (target == null) continue;

                    SearchNode startNode = new(GetScene(target), new() { transition });
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

                if (currentNode.currentScene == finalScene)
                {
                    if (rejectedTransitionPairs.Any(pair => pair.Key == currentNode.currentRoute.First() && pair.Value == currentNode.currentRoute.Last()))
                    {
                        continue;
                    }

                    return currentNode.currentRoute;
                }

                foreach (string transition in transitionSpace)
                {
                    if (GetScene(transition) != currentNode.currentScene) continue;

                    tt.pm.StartTemp();

                    if (currentNode.currentRoute.Count != 0)
                    {
                        tt.pm.Add(tt.pm.lm.GetTransition(GetAdjacentTransition(currentNode.currentRoute.Last())));
                    }

                    if (GetAdjacentTransition(transition) != null
                        && !visitedTransitions.Contains(transition)
                        && tt.pm.lm.TransitionLookup[transition].CanGet(tt.pm))
                    {
                        SearchNode newNode = new(GetScene(GetAdjacentTransition(transition)), currentNode.currentRoute);
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
