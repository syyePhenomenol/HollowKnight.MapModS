using RandomizerCore.Logic;
using RandomizerMod.RC;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RD = RandomizerMod.RandomizerData.Data;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Data
{
    public static class PathfinderData
    {
        internal static Dictionary<string, string> conditionalTerms;

        private static Dictionary<string, string> adjacentScenes;
        private static Dictionary<string, string> adjacentTerms;
        private static Dictionary<string, string> scenesByTransition;
        private static Dictionary<string, HashSet<string>> transitionsByScene;

        private static Dictionary<string, LogicWaypoint> waypointScenes;

        internal static Dictionary<string, string> doorObjectsByScene;
        internal static Dictionary<string, string> doorObjectsByTransition;

        private static readonly (LogicManagerBuilder.JsonType type, string fileName)[] files = new[]
        {
            (LogicManagerBuilder.JsonType.Macros, "macros"),
            (LogicManagerBuilder.JsonType.Waypoints, "waypoints"),
            (LogicManagerBuilder.JsonType.Transitions, "transitions"),
            (LogicManagerBuilder.JsonType.LogicEdit, "logicEdits"),
            (LogicManagerBuilder.JsonType.LogicSubst, "logicSubstitutions")
        };

        private static readonly (LogicManagerBuilder.JsonType type, string fileName)[] godhomeFiles = new[]
        {
            (LogicManagerBuilder.JsonType.Transitions, "godhomeTransitions"),
            (LogicManagerBuilder.JsonType.LogicSubst, "godhomeLogicSubstitutions")
        };

        private static readonly (LogicManagerBuilder.JsonType type, string fileName)[] benchFiles = new[]
        {
            (LogicManagerBuilder.JsonType.LogicEdit, "benchLogicEdits"),
            (LogicManagerBuilder.JsonType.Waypoints, "benchWaypoints")
        };

        private static LogicManagerBuilder lmb;

        public static LogicManager lm;

        public static void Load()
        {
            conditionalTerms = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Data.conditionalTerms.json");
            adjacentScenes = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Data.adjacentScenes.json");
            adjacentTerms = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Data.adjacentTerms.json");
            scenesByTransition = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Data.scenesByTransition.json");
            transitionsByScene = JsonUtil.Deserialize<Dictionary<string, HashSet<string>>>("MapModS.Resources.Pathfinder.Data.transitionsByScene.json");
            doorObjectsByScene = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Compass.doorObjectsByScene.json");
            doorObjectsByTransition = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Compass.doorObjectsByTransition.json");

            lmb = new(RM.RS.Context.LM);

            foreach ((LogicManagerBuilder.JsonType type, string fileName) in files)
            {
                lmb.DeserializeJson(type, Assembly.GetExecutingAssembly().GetManifestResourceStream($"MapModS.Resources.Pathfinder.Logic.{fileName}.json"));
            }

            // Add godhome logic only for rando 4.0.6 or upwards
            if (RM.RS.Context.LM.TermLookup.ContainsKey("GG_Waterways"))
            {
                foreach ((LogicManagerBuilder.JsonType type, string fileName) in godhomeFiles)
                {
                    lmb.DeserializeJson(type, Assembly.GetExecutingAssembly().GetManifestResourceStream($"MapModS.Resources.Pathfinder.Logic.{fileName}.json"));
                }

                transitionsByScene["GG_Waterways"] = new() { "GG_Waterways[warp]" };
                transitionsByScene["GG_Atrium"] = new() { "GG_Atrium[warp]", "GG_Atrium[Door_Workshop]" };
                transitionsByScene["GG_Workshop"] = new() { "GG_Workshop[left1]" };
            }

            // Use BenchRando's BenchDefs if it is enabled for this save
            if (Dependencies.HasBenchRando() && BenchwarpInterop.IsBenchRandoEnabled())
            {
                foreach(KeyValuePair<(string, string), string> kvp in BenchwarpInterop.benchTransitions)
                {
                    adjacentScenes[kvp.Value] = kvp.Key.Item1;
                    adjacentTerms[kvp.Value] = kvp.Value;
                }

                if (lmb.TermLookup.ContainsKey("Bench-Upper_Tram"))
                {
                    lmb.DoLogicEdit(new("Upper_Tram-Left", "ORIG | Bench-Upper_Tram"));
                    lmb.DoLogicEdit(new("Upper_Tram-Right", "ORIG | Bench-Upper_Tram"));
                }

                if (lmb.TermLookup.ContainsKey("Bench-Lower_Tram"))
                {
                    lmb.DoLogicEdit(new("Lower_Tram-Left", "ORIG | Bench-Lower_Tram"));
                    lmb.DoLogicEdit(new("Lower_Tram-Middle", "ORIG | Bench-Lower_Tram"));
                    lmb.DoLogicEdit(new("Lower_Tram-Right", "ORIG | Bench-Lower_Tram"));
                }
            }
            else
            {
                foreach ((LogicManagerBuilder.JsonType type, string fileName) in benchFiles)
                {
                    lmb.DeserializeJson(type, Assembly.GetExecutingAssembly().GetManifestResourceStream($"MapModS.Resources.Pathfinder.Logic.{fileName}.json"));
                }
            }

            // Set Start Warp
            string[] startTerms = GetStartTerms();
            if (startTerms.Length > 0)
            {
                adjacentScenes["Warp-Start"] = ItemChanger.Internal.Ref.Settings.Start.SceneName;
                adjacentTerms["Warp-Start"] = "Warp-Start";
                lmb.AddWaypoint(new("Warp-Start", "FALSE"));
                foreach (string term in startTerms)
                {
                    lmb.DoLogicEdit(new(term, "ORIG | Warp-Start"));
                }
            }

            lm = new(lmb);

            waypointScenes = lm.Waypoints.Where(w => RD.IsRoom(w.Name) || w.Name.IsSpecialRoom()).ToDictionary(w => w.Name, w => w);
        }

        public static string[] GetStartTerms()
        {
            if (RM.RS.Context.InitialProgression is ProgressionInitializer pi && RM.RS.Context.LM is LogicManager lm)
            {
                return pi.Setters.Concat(pi.Increments)
                            .Where(tv => (lm.TransitionLookup.ContainsKey(tv.Term.Name) || lm.Waypoints.Any(waypoint => waypoint.Name == tv.Term.Name)) && tv.Value > 0)
                            .Select(tv => tv.Term.Name)
                            .ToArray();
            }
            return new string[] { };
        }

        public static HashSet<string> GetTransitionsInScene(this string scene)
        {
            HashSet<string> transitions = TransitionData.GetTransitionsByScene(scene);

            if (transitionsByScene.ContainsKey(scene))
            {
                transitions.UnionWith(transitionsByScene[scene]);
            }

            return transitions;
        }

        public static string GetScene(this string transition)
        {
            if (TransitionData.IsInTransitionLookup(transition))
            {
                return TransitionData.GetScene(transition);
            }
            else if (scenesByTransition.ContainsKey(transition))
            {
                return scenesByTransition[transition];
            }
            else if (transition.Contains("["))
            {
                return transition.Split('[')[0];
            }

            return null;
        }

        // Returns the correct adjacent scene for special transitions
        public static string GetAdjacentScene(this string transition)
        {
            if (transition.IsSpecialTransition())
            {
                return adjacentScenes[transition];
            }

            return transition.GetAdjacentTerm().GetScene();
        }

        public static string GetAdjacentTerm(this string transition)
        {
            if (adjacentTerms.ContainsKey(transition))
            {
                return adjacentTerms[transition];
            }

            // Some top transitions don't have an adjacent transition
            if (TransitionData.IsInTransitionLookup(transition))
            {
                return TransitionData.GetAdjacentTransition(transition);
            }

            MapModS.Instance.LogWarn($"No adjacent term for {transition}");

            return null;
        }

        public static bool TryGetSceneWaypoint(string scene, out LogicWaypoint waypoint)
        {
            if (waypointScenes.ContainsKey(scene))
            {
                waypoint = waypointScenes[scene];
                return true;
            }

            waypoint = null;
            return false;
        }

        public static bool IsSpecialTransition(this string transition)
        {
            return adjacentTerms.ContainsKey(transition) || transition.IsBenchwarpTransition();
        }

        public static bool IsBenchwarpTransition(this string transition)
        {
            return BenchwarpInterop.benchKeys.ContainsKey(transition);
        }

        public static bool IsStagTransition(this string transition)
        {
            return transition.IsSpecialTransition() && transition.StartsWith("Stag");
        }

        public static bool IsElevatorTransition(this string transition)
        {
            return transition.IsSpecialTransition() && (transition.StartsWith("Left_Elevator") || transition.StartsWith("Right_Elevator"));
        }

        public static bool IsTramTransition(this string transition)
        {
            return transition.IsSpecialTransition() && (transition.StartsWith("Lower_Tram") || transition.StartsWith("Upper_Tram"));
        }

        public static bool IsWarpTransition(this string transition)
        {
            return transition.IsSpecialTransition() && transition.Contains("[warp]");
        }
    }
}
