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
        public TransitionHelper()
        {
            tt = new();
        }

        // Code based on homothety's implementation of RandomizerMod's TrackerData
        class TransitionTracker
        {
            public readonly HashSet<int> obtainedItems = RandomizerMod.RandomizerMod.RS.TrackerData.obtainedItems;
            public readonly HashSet<int> outOfLogicObtainedItems = RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicObtainedItems;

            public LogicManager lm = RandomizerMod.RandomizerMod.RS.Context.LM;
            public RandoContext ctx = RandomizerMod.RandomizerMod.RS.Context;
            public ProgressionManager pm;
            private readonly MainUpdater mu;

            public TransitionTracker()
            {
                pm = new(lm, ctx);

                mu = new(lm);

                mu.AddEntries(lm.Waypoints.Select(w => new DelegateUpdateEntry(w, pm => { pm.Add(w); }, lm)));

                if (ctx.transitionPlacements != null)
                {
                    mu.AddEntries(ctx.transitionPlacements.Select((p, id) => new DelegateUpdateEntry(p.source, pm =>
                    {
                        (RandoTransition source, RandoTransition target) = ctx.transitionPlacements[id];

                        if (!pm.Has(source.lt.term))
                        {
                            pm.Add(source);
                        }
                    }, lm)));
                }

                mu.Hook(pm);
            }

            public void GetNewItems()
            {
                // Get new progression since last time Setup() was called
                List<RandoItem> items = obtainedItems.Where(i => !outOfLogicObtainedItems.Contains(i)).Select(i => ctx.itemPlacements[i].item).ToList();
                pm.Add(items);
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

            // If it's not in TransitionPlacements, it's the vanilla target
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
        public List<string> ShortestRoute(string startScene, string finalScene, HashSet<string> rejectedTransitions)
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

            // Remove rejected start transitions
            if (rejectedTransitions != null)
            {
                foreach (var key in transitionSpace.Intersect(rejectedTransitions).ToList())
                {
                    transitionSpace.Remove(key);
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

            // Just in case
            transitionSpace.Remove(null);

            // Get starting transitions
            foreach (string transition in transitionSpace)
            {
                if (GetScene(transition) == startScene)
                {
                    startTransitionPlacements.Add(transition, GetAdjacentTransition(transition));
                }
            }

            tt.GetNewItems();

            // Algorithm (BFS)
            HashSet<string> visitedTransitions = new();
            LinkedList<SearchNode> queue = new();

            foreach (KeyValuePair<string, string> tp in startTransitionPlacements)
            {
                if (tp.Value == null) continue;

                SearchNode startNode = new(GetScene(tp.Value), new() { tp.Key });
                queue.AddLast(startNode);
            }

            while (queue.Any())
            {
                SearchNode currentNode = queue.First();
                queue.RemoveFirst();

                searchScene = currentNode.currentScene;

                //currentNode.PrintRoute();

                if (currentNode.currentScene == finalScene)
                {
                    return currentNode.currentRoute;
                }

                foreach (string transition in transitionSpace)
                {
                    if (GetScene(transition) != currentNode.currentScene) continue;

                    tt.pm.StartTemp();

                    foreach (string routeTransition in currentNode.currentRoute)
                    {
                        tt.pm.Add(tt.lm.GetTransition(routeTransition));
                    }

                    if (currentNode.currentRoute.Count != 0)
                    {
                        tt.pm.Add(tt.lm.GetTransition(GetAdjacentTransition(currentNode.currentRoute.Last())));
                    }

                    if (GetAdjacentTransition(transition) != null
                        && !visitedTransitions.Contains(transition)
                        && tt.lm.TransitionLookup[transition].CanGet(tt.pm))
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
