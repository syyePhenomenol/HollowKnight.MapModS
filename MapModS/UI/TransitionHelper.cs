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
    public static class TransitionHelper
    {
        private static string GetScene(string transition)
        {
            return RandomizerMod.RandomizerData.Data.GetTransitionDef(transition).SceneName;
        }

        private static string GetAdjacentTransition(string transition)
        {
            foreach (TransitionPlacement tp in RandomizerMod.RandomizerMod.RS.Context.transitionPlacements)
            {
                if (tp.source.Name == transition)
                {
                    return tp.target.Name;
                }
            }

            RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(transition);

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

        // Code based on homothety's implementation of RandomizerMod's TrackerData
        class TransitionTracker
        {
            public HashSet<int> obtainedItems = new(RandomizerMod.RandomizerMod.RS.TrackerData.obtainedItems);
            public HashSet<int> outOfLogicObtainedItems = new(RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicObtainedItems);
            public Dictionary<string, string> visitedTransitions = new(RandomizerMod.RandomizerMod.RS.TrackerData.visitedTransitions);
            public HashSet<string> outOfLogicVisitedTransitions = new(RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicVisitedTransitions);

            public ProgressionManager pm;
            public LogicManager lm = RandomizerMod.RandomizerMod.RS.Context.LM;
            public RandoContext ctx = RandomizerMod.RandomizerMod.RS.Context;
            private MainUpdater mu;

            public void Setup(string startScene)
            {
                List<RandoItem> items = obtainedItems.Where(i => !outOfLogicObtainedItems.Contains(i)).Select(i => ctx.itemPlacements[i].item).ToList();
                List<LogicTransition> transitions = visitedTransitions.Keys.Concat(visitedTransitions.Values).Where(t => !outOfLogicVisitedTransitions.Contains(t)).Distinct().Select(t => lm.GetTransition(t)).ToList();

                pm = new(lm, ctx);
                pm.Add(items);

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

                        if (outOfLogicVisitedTransitions.Remove(source.Name) && !pm.Has(target.lt.term))
                        {
                            pm.Add(target);
                        }
                    }, lm)));
                }

                mu.Hook(pm);

                // Add transitions for current room
                foreach (LogicTransition transition in transitions)
                {
                    if (transition.data.SceneName == startScene || GetScene(GetAdjacentTransition(transition.Name)) == startScene)
                    {
                        MapModS.Instance.Log($"Starting by adding {transition.Name}");
                        pm.Add(transition);
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

        private static string searchScene;

        // Calculates the shortest route (by number of transitions) from startScene to finalScene.
        // The search space will be purely limited to rooms that have been visited + unreached reachable locations
        // A ProgressionManager is used to track logic while traversing through the search space
        public static List<string> ShortestRoute(string startScene, string finalScene)
        {
            List<string> shortestRoute = new();
            HashSet<string> transitionSpace = new();

            // Get all relevant transitions
            foreach (KeyValuePair<string, LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
            {
                if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(transitionEntry.Value.term.Id))
                {
                    transitionSpace.Add(transitionEntry.Key);
                }
            }

            // Sample two scenes
            if (startScene == "" && finalScene == "")
            {
                startScene = GetScene(transitionSpace.First());
                finalScene = GetScene(transitionSpace.Last());
            }

            MapModS.Instance.Log($"Start scene: {startScene}");
            MapModS.Instance.Log($"Final scene: {finalScene}");

            TransitionTracker tt = new();
            tt.Setup(startScene);

            // Algorithm (BFS)
            HashSet<string> visitedTransitions = new();
            LinkedList<SearchNode> queue = new();

            SearchNode currentNode = new(startScene, new());
            queue.AddLast(currentNode);

            while (queue.Any())
            {
                currentNode = queue.First();
                queue.RemoveFirst();

                searchScene = currentNode.currentScene;

                currentNode.PrintRoute();

                if (currentNode.currentScene == finalScene)
                {
                    MapModS.Instance.Log("Successful termination");
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

                    if (!visitedTransitions.Contains(transition) && tt.lm.TransitionLookup[transition].CanGet(tt.pm))
                    {
                        SearchNode newNode = new(GetScene(GetAdjacentTransition(transition)), currentNode.currentRoute);
                        newNode.currentRoute.Add(transition);
                        visitedTransitions.Add(transition);
                        queue.AddLast(newNode);
                    }

                    tt.pm.RemoveTempItems();
                }
            }

            return shortestRoute;
        }

        public static void TestAlgorithm()
        {
            string startScene = "Tutorial_01";
            string finalScene = "Deepnest_10";

            foreach (string transition in ShortestRoute(startScene, finalScene))
            {
                MapModS.Instance.Log(transition);
            }
        }
    }
}
