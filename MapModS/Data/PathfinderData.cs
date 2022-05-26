using RandomizerCore.Logic;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RD = RandomizerMod.RandomizerData.Data;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Data
{
    public static class PathfinderData
    {
        // Waterways_02: from bench to top3, need to check if floor is broken
        // also need to fix all the logic for each transition from the bench
        // Ruins1_31: from left to right side (Ruins1_31), need to check if wall is broken

        public static readonly Dictionary<string, string> conditionalTerms = new()
        {
            { "Ruins1_31[left3]", "ELEGANT" },
            { "Ruins2_11_b[left1]", "LOVE" },
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
            { "Waterways_07[right1]", "Lever-Dung_Defender" }
        };

        public static readonly HashSet<string> persistentTerms = new()
        {
            { "Town[door_station]" },
            { "Town[door_sly]" },
            { "Town[door_mapper]" },
            { "Town[door_jiji]" },
            { "Town[door_bretta]" },
            { "Town[room_divine]" },
            { "Town[room_grimm]" },
            { "Crossroads_09[left1]" },
            { "Crossroads_09[right1]" },
            { "LEFTSLASH" },
            { "RIGHTSLASH" },
            { "UPSLASH" },
            { "DOWNSLASH" },
            { "SWIM" },
        };

        public static string GetScene(this string transition)
        {
            if (transition == "Warp_Start")
            {
                return RD.GetStartDef(RM.RS.GenerationSettings.StartLocationSettings.StartLocation).SceneName;
            }

            if (transition.IsSpecialTransition())
            {
                return specialTransitions[transition].GetScene();
            }

            return TransitionData.GetTransitionScene(transition);
        }

        public static string GetAdjacentTransition(this string source)
        {
            if (source == "Warp_Start")
            {
                return RD.GetStartDef(RM.RS.GenerationSettings.StartLocationSettings.StartLocation).Transition;
            }

            // Some top transitions don't have an adjacent transition
            if (TransitionData.IsInTransitionLookup(source))
            {
                return TransitionData.GetAdjacentTransition(source);
            }

            if (source.IsSpecialTransition())
            {
                return source;
            }

            MapModS.Instance.LogWarn($"No adjacent transition for {source}");

            return null;
        }

        // Returns all benchwarps based on benches sat on + Start
        public static HashSet<string> GetBenchwarpTransitions()
        {
            IEnumerable<string> visitedBenches = Dependencies.GetVisitedBenchScenes();

            HashSet<string> transitions = new(benchwarpScenes.Where(b => visitedBenches.Contains(b.Value)).Select(b => b.Key));

            transitions.Add("Warp_Start");

            return transitions;
        }

        public static bool IsSpecialTransition(this string transition)
        {
            return specialTransitions.ContainsKey(transition);
        }

        public static bool IsBenchwarpTransition(this string transition)
        {
            return transition.IsSpecialTransition() && transition.StartsWith("Warp");
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

        public static HashSet<string> GetTransitionsInScene(this string scene)
        {
            HashSet<string> transitions = TransitionData.GetTransitionsByScene(scene);

            if (sceneSpecialTransitions.ContainsKey(scene))
            {
                transitions.UnionWith(sceneSpecialTransitions[scene]);
            }

            return transitions;
        }

        private static Dictionary<string, string> benchwarpScenes;
        private static Dictionary<string, string> specialTransitions;
        private static Dictionary<string, HashSet<string>> sceneSpecialTransitions;

        public static void Load()
        {
            benchwarpScenes = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Data.benchwarp.json");
            specialTransitions = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Data.specialTransitions.json");
            sceneSpecialTransitions = JsonUtil.Deserialize<Dictionary<string, HashSet<string>>>("MapModS.Resources.Pathfinder.Data.sceneTransitions.json");
        }

        private static readonly (LogicManagerBuilder.JsonType type, string fileName)[] files = new[]
        {
            (LogicManagerBuilder.JsonType.Macros, "macros"),
            (LogicManagerBuilder.JsonType.Transitions, "transitions"),
            (LogicManagerBuilder.JsonType.LogicEdit, "logicEdits"),
            (LogicManagerBuilder.JsonType.LogicSubst, "logicSubstitutions")
        };

        private static LogicManagerBuilder lmb;

        public static LogicManager lm;

        public static void MakeLogicManager()
        {
            lmb = new(RM.RS.Context.LM);

            foreach ((LogicManagerBuilder.JsonType type, string fileName) in files)
            {
                lmb.DeserializeJson(type, Assembly.GetExecutingAssembly().GetManifestResourceStream($"MapModS.Resources.Pathfinder.Logic.{fileName}.json"));
            }

            lm = new(lmb);
        }
    }
}
