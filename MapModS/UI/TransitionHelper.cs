using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomizerCore;
using RandomizerMod;

namespace MapModS.UI
{
    public static class TransitionHelper
    {
        private static string ToScene(string transition)
        {
            return transition.Split('[')[0];
        }

        class Node
        {
            public Node(string scene, List<string> route)
            {
                currentScene = scene;
                currentRoute = route;
            }

            public string currentScene;
            public List<string> currentRoute = new();
        }

        // Calculates the shortest route (by number of transitions) from startScene to finalScene.
        // The search space will be purely limited to rooms that have been visited + unreached reachable locations
        // A ProgressionManager is used to track logic while traversing through the search space
        public static List<string> ShortestRoute(string startScene, string finalScene)
        {
            List<string> shortestRoute = new();

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
            RandomizerCore.Logic.ProgressionManager pm = new(RandomizerMod.RandomizerMod.RS.TrackerData.lm, RandomizerMod.RandomizerMod.RS.Context);

            //Dictionary<string, string> inLogicTransitions = new();

            //foreach (KeyValuePair<string, string> transitionPair in transitions)
            //{
            //    if (RandomizerMod.RandomizerMod.RS.TrackerData.lm.LogicLookup[transitionPair.Key].CanGet(pm))
            //    {
            //        inLogicTransitions.Add(transitionPair.Key, transitionPair.Value);
            //    }
            //}

            // Create set of all scenes for convenience
            HashSet<string> scenes = new();

            foreach (KeyValuePair<string, string> transitionPair in transitions)
            {
                MapModS.Instance.Log($"Transition: {transitionPair.Key} -> {transitionPair.Value}");

                string sourceScene = ToScene(transitionPair.Key);
                string targetScene = ToScene(transitionPair.Value);

                scenes.Add(sourceScene);
                scenes.Add(targetScene);
            }

            foreach (string scene in scenes)
            {
                MapModS.Instance.Log($"Scenes: {scene}");
            }

            if (!scenes.Contains(startScene) || !scenes.Contains(finalScene))
            {
                return shortestRoute;
            }

            foreach (KeyValuePair<string, RandomizerCore.Logic.OptimizedLogicDef> pair in pm.lm.LogicLookup)
            {
                MapModS.Instance.Log(pair.Key);
                MapModS.Instance.Log(pair.Value.CanGet(pm));
            }

            //// Clear current scene and transitions
            ////pm.Set(startScene, 0);
            //foreach (KeyValuePair<string, string> transition in transitions)
            //{
            //    if (ToScene(transition.Key) == startScene)
            //    {
            //        pm.Set(transition.Key, 0);
            //    }
            //}

            // Algorithm (BFS)
            HashSet<string> visitedTransitions = new();
            LinkedList<Node> queue = new();

            Node currentNode = new(startScene, new());
            queue.AddLast(currentNode);

            while(queue.Any())
            {
                MapModS.Instance.Log("New queue pop");

                currentNode = queue.First();
                queue.RemoveFirst();

                if (currentNode.currentScene == finalScene)
                {
                    MapModS.Instance.Log("Successful termination");
                    return currentNode.currentRoute;
                }

                foreach (KeyValuePair<string, string> transitionPair in transitions)
                {
                    if (ToScene(transitionPair.Key) != currentNode.currentScene) continue;

                    pm.StartTemp();

                    //pm.Set(currentNode.currentScene, 1);

                    //foreach (KeyValuePair<string, string> transition in transitions)
                    //{
                    //    if (ToScene(transition.Key) == currentNode.currentScene)
                    //    {
                    //        pm.Set(transition.Key, 1);
                    //    }
                    //}

                    //foreach (string transitionSource in currentNode.currentRoute)
                    //{
                    //    //MapModS.Instance.Log(pm.Get(transitionSource));
                    //    pm.Add(pm.lm.GetTransition(transitionSource));
                    //}

                    //pm.Set(transition.Key, 1);

                    if (!visitedTransitions.Contains(transitionPair.Key) && pm.lm.LogicLookup[transitionPair.Key].CanGet(pm))
                    {
                        Node newNode = new(ToScene(transitionPair.Value), currentNode.currentRoute);
                        newNode.currentRoute.Add(transitionPair.Key);
                        visitedTransitions.Add(transitionPair.Key);
                        queue.AddLast(newNode);
                    }

                    pm.RemoveTempItems();
                }
            }

            return shortestRoute;
        }
    }
}
