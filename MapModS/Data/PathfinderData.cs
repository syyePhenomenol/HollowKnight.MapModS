using System;
using System.Collections.Generic;
using System.Linq;
using RD = RandomizerMod.RandomizerData;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Data
{
    public static class PathfinderData
    {
        public static readonly Dictionary<string, string> transitionTermFixes = new()
        {
            { "Ruins1_31[left3]", "ELEGANT" },
            { "Ruins2_11_b[left1]", "LOVE" }
        };

        public static readonly Dictionary<string, string> waypointFixes = new()
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
            // Special waypoint needed
            { "Warp Waterways", "Waterways_02[top1]" },
            { "Warp Deepnest Hot Springs", "Deepnest_30[left1]" },
            { "Warp Failed Tramway", "Deepnest_14[left1]" },
            { "Warp Beast's Den", "Deepnest_Spider_Town[left1]" },
            { "Warp Ancient Basin Toll", "Abyss_18[right1]" },
            { "Warp Hidden Station", "Abyss_22[left1]" },
            { "Warp Oro", "Deepnest_East_06[door1]" },
            { "Warp Kingdom's Edge Camp", "Deepnest_East_13[bot1]" },
            // Manually check transitions from this bench
            { "Warp Colosseum", "Room_Colosseum_02[top1]" },
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
            // Manually check transitions from this bench
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

        public static string GetScene(string transition)
        {
            if (transition == "Warp Start")
            {
                return RD.Data.GetStartDef(RM.RS.GenerationSettings.StartLocationSettings.StartLocation).SceneName;
            }

            if (benchWarpTransitions.ContainsKey(transition))
            {
                return GetScene(benchWarpTransitions[transition]);
            }

            // Handle in a special way
            if (stagTransitions.ContainsKey(transition))
            {
                return null;
            }

            if (elevatorTransitions.ContainsKey(transition))
            {
                return elevatorTransitions[transition].Item2;
            }

            // Handle in a special way
            if (tramTransitions.ContainsKey(transition))
            {
                return null;
            }

            if (normalWarpTransitions.ContainsKey(transition))
            {
                return transition.Substring(0, transition.Length - 6);
            }

            return TransitionData.GetTransitionScene(transition);
        }

        public static string GetAdjacentTransition(string source)
        {
            if (source == "Warp Start")
            {
                return RD.Data.GetStartDef(RM.RS.GenerationSettings.StartLocationSettings.StartLocation).Transition;
            }

            // Some top transitions don't have an adjacent transition
            if (TransitionData.IsInTransitionLookup(source))
            {
                return TransitionData.GetAdjacentTransition(source);
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

            MapModS.Instance.LogWarn($"No adjacent transition for {source}");

            return null;
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
            if (stagTransitions.ContainsKey(transition) && stagTransitions.Any(t => t.Value.Item2.StartsWith(currentScene))) return true;

            if (elevatorTransitions.ContainsKey(transition) && currentScene == elevatorTransitions[transition].Item2) return true;

            if ((transition.StartsWith("Upper Tram") && currentScene.StartsWith("Crossroads_46"))
                || (transition.StartsWith("Lower Tram") && currentScene.StartsWith("Abyss_03"))) return true;

            if (normalWarpTransitions.ContainsKey(transition) && transition.StartsWith(currentScene)) return true;

            return false;
        }
    }
}
