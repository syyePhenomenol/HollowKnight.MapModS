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
        private static string ToScene(string transition)
        {
            return transition.Split('[')[0];
        }

        class SearchNode
        {
            public SearchNode(string scene, List<string> route)
            {
                currentScene = scene;
                currentRoute = route;
            }

            public string currentScene;
            public List<string> currentRoute = new();
        }

        // Code borrowed from homothety's implementation of RandomizerMod's TrackerData
        class TempTracker
        {
            public HashSet<int> obtainedItems = new(RandomizerMod.RandomizerMod.RS.TrackerData.obtainedItems);
            public HashSet<int> outOfLogicObtainedItems = new(RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicObtainedItems);
            public Dictionary<string, string> visitedTransitions = new(RandomizerMod.RandomizerMod.RS.TrackerData.visitedTransitions);
            public HashSet<string> outOfLogicVisitedTransitions = new(RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicVisitedTransitions);

            public ProgressionManager pm;
            public LogicManager lm = RandomizerMod.RandomizerMod.RS.Context.LM;
            public RandoContext ctx = RandomizerMod.RandomizerMod.RS.Context;
            private MainUpdater mu;

            public void Setup()
            {
                List<RandoItem> items = obtainedItems.Where(i => !outOfLogicObtainedItems.Contains(i)).Select(i => ctx.itemPlacements[i].item).ToList();
                List<LogicTransition> transitions = visitedTransitions.Keys.Concat(visitedTransitions.Values).Where(t => !outOfLogicVisitedTransitions.Contains(t)).Distinct().Select(t => lm.GetTransition(t)).ToList();

                pm = new(lm, ctx);
                pm.Add(items);
                pm.Add(transitions);

                mu = new(lm);
                mu.AddEntries(lm.Waypoints.Select(w => new DelegateUpdateEntry(w, pm =>
                {
                    pm.Add(w);
                })));
                mu.AddEntries(ctx.Vanilla.Select(v => new DelegateUpdateEntry(v.Location, pm =>
                {
                    pm.Add(v.Item);
                })));
                if (ctx.itemPlacements != null)
                {
                    mu.AddEntries(ctx.itemPlacements.Select((p, id) => new DelegateUpdateEntry(p.location.logic, OnCanGetLocation(id))));
                }

                if (ctx.transitionPlacements != null)
                {
                    mu.AddEntries(ctx.transitionPlacements.Select((p, id) => new DelegateUpdateEntry(p.source, OnCanGetTransition(id))));
                }

                mu.Hook(pm); // automatically handle tracking reachable unobtained locations/transitions and adding vanilla progression to pm
            }

            private Action<ProgressionManager> OnCanGetLocation(int id)
            {
                return pm =>
                {
                    (RandoItem item, RandoLocation location) = ctx.itemPlacements[id];
                    if (outOfLogicObtainedItems.Remove(id))
                    {
                        pm.Add(item);
                    }
                };
            }

            private Action<ProgressionManager> OnCanGetTransition(int id)
            {
                return pm =>
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
                };
            }

            public void OnTransitionVisited(string source, string target)
            {
                visitedTransitions[source] = target;

                LogicTransition st = lm.GetTransition(source);
                if (st.CanGet(pm))
                {
                    LogicTransition tt = lm.GetTransition(target);
                    if (!pm.Has(st.term))
                    {
                        pm.Add(st);
                    }

                    if (!pm.Has(tt.term))
                    {
                        pm.Add(tt);
                    }
                }
                else
                {
                    outOfLogicVisitedTransitions.Add(source);
                }
            }

            public class DelegateUpdateEntry : UpdateEntry
            {
                readonly Action<ProgressionManager> onAdd;
                readonly ILogicDef location;

                public DelegateUpdateEntry(ILogicDef location, Action<ProgressionManager> onAdd)
                {
                    this.location = location;
                    this.onAdd = onAdd;
                }

                public override bool CanGet(ProgressionManager pm)
                {
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

        // Calculates the shortest route (by number of transitions) from startScene to finalScene.
        // The search space will be purely limited to rooms that have been visited + unreached reachable locations
        // A ProgressionManager is used to track logic while traversing through the search space
        public static List<string> ShortestRoute(string startScene, string finalScene)
        {
            List<string> shortestRoute = new();

            //HashSet<int> oI = RandomizerMod.RandomizerMod.RS.TrackerData.obtainedItems;
            //Dictionary<string, string> vT = RandomizerMod.RandomizerMod.RS.TrackerData.visitedTransitions;
            //HashSet<int> oOLOI = RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicObtainedItems;

            // Get all relevant transitions
            Dictionary<string, string> transitions = new(RandomizerMod.RandomizerMod.RS.TrackerData.visitedTransitions);

            foreach (string transition in RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableTransitions)
            {
                foreach (TransitionPlacement transitionPlacement in RandomizerMod.RandomizerMod.RS.Context.transitionPlacements)
                {
                    if (transitionPlacement.source.Name == transition)
                    {
                        transitions.Add(transitionPlacement.source.Name, transitionPlacement.target.Name);
                    }
                }
            }

            // Create new ProgressionManager from current state
            //ProgressionManager pm = new(RandomizerMod.RandomizerMod.RS.TrackerData.lm, RandomizerMod.RandomizerMod.RS.Context);
            //List<RandoItem> pm_items = RandomizerMod.RandomizerMod.RS.TrackerData.obtainedItems.Where(i => !RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicObtainedItems.Contains(i)).Select(i => ctx.itemPlacements[i].item).ToList();
            //List<LogicTransition> pm_transitions = vT.Keys.Concat(vT.Values).Where(t => !oOLOI.Contains(t)).Distinct().Select(t => lm.GetTransition(t)).ToList();

            //RandomizerCore.Logic.MainUpdater mu = new(pm.lm);
            //mu.Hook(pm);

            TempTracker tt = new();
            tt.Setup();

            // Create set of all scenes for convenience
            HashSet<string> scenes = new();

            foreach (KeyValuePair<string, string> transitionPair in transitions)
            {
                //MapModS.Instance.Log($"Transition: {transitionPair.Key} -> {transitionPair.Value}");

                string sourceScene = ToScene(transitionPair.Key);
                string targetScene = ToScene(transitionPair.Value);

                scenes.Add(sourceScene);
                scenes.Add(targetScene);
            }

            foreach (string scene in scenes)
            {
                //MapModS.Instance.Log($"Scenes: {scene}");
            }

            //if (!scenes.Contains(startScene) || !scenes.Contains(finalScene))
            //{
            //    return shortestRoute;
            //}

            foreach (KeyValuePair<string, OptimizedLogicDef> pair in tt.lm.LogicLookup)
            {
                if (pair.Value.CanGet(tt.pm))
                {
                    MapModS.Instance.Log(pair.Key);
                    //MapModS.Instance.Log("True");
                }
            }

            tt.pm.StartTemp();

            

            tt.pm.RemoveTempItems();

            //// Clear current scene and transitions
            ////pm.Set(startScene, 0);
            //foreach (KeyValuePair<string, string> transition in transitions)
            //{
            //    if (ToScene(transition.Key) == startScene)
            //    {
            //        pm.Set(transition.Key, 0);
            //    }
            //}

            //// Algorithm (BFS)
            //HashSet<string> visitedTransitions = new();
            //LinkedList<Node> queue = new();

            //Node currentNode = new(startScene, new());
            //queue.AddLast(currentNode);

            //while(queue.Any())
            //{
            //    MapModS.Instance.Log("New queue pop");

            //    currentNode = queue.First();
            //    queue.RemoveFirst();

            //    if (currentNode.currentScene == finalScene)
            //    {
            //        MapModS.Instance.Log("Successful termination");
            //        return currentNode.currentRoute;
            //    }

            //    foreach (KeyValuePair<string, string> transitionPair in transitions)
            //    {
            //        if (ToScene(transitionPair.Key) != currentNode.currentScene) continue;

            //        pm.StartTemp();

            //        //pm.Set(currentNode.currentScene, 1);

            //        //foreach (KeyValuePair<string, string> transition in transitions)
            //        //{
            //        //    if (ToScene(transition.Key) == currentNode.currentScene)
            //        //    {
            //        //        pm.Set(transition.Key, 1);
            //        //    }
            //        //}

            //        foreach (string transitionSource in currentNode.currentRoute)
            //        {
            //            pm.Add(pm.lm.GetTransition(transitionSource));
            //        }

            //        //pm.Set(transition.Key, 1);

            //        if (!visitedTransitions.Contains(transitionPair.Key) && pm.lm.LogicLookup[transitionPair.Key].CanGet(pm))
            //        {
            //            Node newNode = new(ToScene(transitionPair.Value), currentNode.currentRoute);
            //            newNode.currentRoute.Add(transitionPair.Key);
            //            visitedTransitions.Add(transitionPair.Key);
            //            queue.AddLast(newNode);
            //        }

            //        pm.RemoveTempItems();
            //    }
            //}

            return shortestRoute;
        }
    }
}
