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

        private static string GetAdjacentScene(string transition)
        {
            foreach (TransitionPlacement tp in RandomizerMod.RandomizerMod.RS.Context.transitionPlacements)
            {
                if (tp.target == null) return null;

                if (tp.source.Name == transition)
                {
                    return GetScene(tp.target.Name);
                }
            }

            RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(transition);

            if (transitionDef.VanillaTarget == null) return null;

            // If it's not in TransitionPlacements, it's the vanilla target
            return GetScene(transitionDef.VanillaTarget);
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
                string text = "Current route: ";

                foreach (string transition in currentRoute)
                {
                    text += "-> " + transition;
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

            public void Setup()
            {
                List<RandoItem> items = obtainedItems.Where(i => !outOfLogicObtainedItems.Contains(i)).Select(i => ctx.itemPlacements[i].item).ToList();
                List<LogicTransition> transitions = visitedTransitions.Keys.Concat(visitedTransitions.Values).Where(t => !outOfLogicVisitedTransitions.Contains(t)).Distinct().Select(t => lm.GetTransition(t)).ToList();

                pm = new(lm, ctx);
                pm.Add(items);
                pm.Add(transitions);

                //foreach (KeyValuePair<string, string> visitedTransition in visitedTransitions)
                //{
                //    pm.Set(lm.GetTransition(visitedTransition.Key).term.Id, 0);
                //}

                // Get the scenes that are visited or connected in vanilla fashion to other visited scenes
                foreach (KeyValuePair<string, LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
                {
                    if (pm.Has(transitionEntry.Value.term.Id))
                    {
                        RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(transitionEntry.Key);
                        pm.Set(lm.GetTerm(transitionDef.SceneName), 0);
                    }
                }

                mu = new(lm);

                mu.AddEntries(lm.Waypoints.Select(w => new DelegateUpdateEntry(w, pm => {pm.Add(w);})));

                mu.AddEntries(ctx.Vanilla.Select(v => new DelegateUpdateEntry(v.Location, pm => {pm.Add(v.Item);})));

                if (ctx.itemPlacements != null)
                {
                    mu.AddEntries(ctx.itemPlacements.Select((p, id) => new DelegateUpdateEntry(p.location.logic, OnCanGetLocation(id))));
                }

                if (ctx.transitionPlacements != null)
                {
                    mu.AddEntries(ctx.transitionPlacements.Select((p, id) => new DelegateUpdateEntry(p.source, OnCanGetTransition(id))));
                }

                mu.Hook(pm);
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

            //public void OnTransitionVisited(string source, string target)
            //{
            //    visitedTransitions[source] = target;

            //    LogicTransition st = lm.GetTransition(source);
            //    if (st.CanGet(pm))
            //    {
            //        LogicTransition tt = lm.GetTransition(target);
            //        if (!pm.Has(st.term))
            //        {
            //            pm.Add(st);
            //        }

            //        if (!pm.Has(tt.term))
            //        {
            //            pm.Add(tt);
            //        }
            //    }
            //    else
            //    {
            //        outOfLogicVisitedTransitions.Add(source);
            //    }
            //}

            private Action<ProgressionManager> OnCanGetTransition(int id)
            {
                return pm =>
                {
                    //MapModS.Instance.Log(" - - OnCanGetTransition " + id);

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
            HashSet<string> transitionSpace = new();

            // Get all relevant transitions
            foreach (KeyValuePair<string, LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
            {
                if (RandomizerMod.RandomizerMod.RS.TrackerData.pm.Has(transitionEntry.Value.term.Id))
                {
                    transitionSpace.Add(transitionEntry.Key);
                }
            }

            TransitionTracker tt = new();
            tt.Setup();

            //foreach (KeyValuePair<string, OptimizedLogicDef> transitionEntry in tt.lm.LogicLookup)
            //{

            //    MapModS.Instance.Log($"{transitionEntry.Key}: {transitionEntry.Value.CanGet(tt.pm)}");
            //}

            //MapModS.Instance.Log(tt.lm.LogicLookup["Tutorial_01[right1]"].CanGet(tt.pm));
            //MapModS.Instance.Log(tt.lm.LogicLookup["Ruins2_04[right1]"].CanGet(tt.pm));

            startScene = GetScene(transitionSpace.First());
            finalScene = GetScene(transitionSpace.Last());
            MapModS.Instance.Log($"Start scene: {startScene}");
            MapModS.Instance.Log($"Final scene: {finalScene}");

            // Clear existing transitions
            foreach (KeyValuePair<string, LogicTransition> transition in tt.lm.TransitionLookup)
            {
                tt.pm.Set(transition.Key, 0);
            }

            // Algorithm (BFS)
            HashSet<string> visitedTransitions = new();
            LinkedList<SearchNode> queue = new();

            SearchNode currentNode = new(startScene, new());
            queue.AddLast(currentNode);

            while (queue.Any())
            {
                currentNode = queue.First();
                queue.RemoveFirst();

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

                    //tt.pm.Add(tt.lm.GetItem(currentNode.currentScene));

                    //foreach (string routeTransition in currentNode.currentRoute)
                    //{
                    //    tt.pm.Add(tt.lm.GetTransition(routeTransition));
                    //}

                    //MapModS.Instance.Log($"- Adding transition {transition} to pm");

                    // Add transition to update pm
                    //tt.pm.Add(tt.lm.GetTransition(transition));

                    foreach (KeyValuePair<string, OptimizedLogicDef> transitionEntry in tt.lm.LogicLookup)
                    {
                        if (transitionEntry.Value.CanGet(tt.pm))
                        {
                            MapModS.Instance.Log($"- Can get: {transitionEntry.Value.Name}");
                        }
                    }

                    if (!visitedTransitions.Contains(transition) && tt.lm.TransitionLookup[transition].CanGet(tt.pm))
                    {
                        MapModS.Instance.Log($"- Adding transition {transition} to node");
                        SearchNode newNode = new(GetAdjacentScene(transition), currentNode.currentRoute);
                        newNode.currentRoute.Add(transition);
                        visitedTransitions.Add(transition);
                        queue.AddLast(newNode);
                    }

                    tt.pm.RemoveTempItems();
                }
            }

            return shortestRoute;
        }
    }
}
