using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerMod;
using RandomizerMod.RC;
using System.Collections.Generic;
using System.Linq;
using RD = RandomizerMod.RandomizerData.Data;
using RM = RandomizerMod.RandomizerMod;
using TM = RandomizerMod.Settings.TransitionSettings.TransitionMode;

namespace MapModS.Data
{
    public static class TransitionData
    {
        private static RandoModContext Ctx => RM.RS?.Context;
        private static LogicManager Lm => Ctx?.LM;

        private static HashSet<string> _randomizedTransitions = new();
        private static Dictionary<string, TransitionPlacement> _transitionLookup = new();
        private static Dictionary<string, HashSet<string>> _transitionsByScene = new();
        private static Dictionary<string, LogicWaypoint> _waypointScenes = new();

        public static bool IsTransitionRando()
        {
            return RM.RS.GenerationSettings.TransitionSettings.Mode != TM.None
                || (RM.RS.Context.transitionPlacements != null && RM.RS.Context.transitionPlacements.Any());
        }

        public static bool TransitionModeActive()
        {
            return MapModS.LS.ModEnabled
                && (MapModS.LS.mapMode == Settings.MapMode.TransitionRando
                    || MapModS.LS.mapMode == Settings.MapMode.TransitionRandoAlt);
        }

        public static bool IsRandomizedTransition(string source)
        {
            return _randomizedTransitions.Contains(source);
        }

        public static bool IsInTransitionLookup(string source)
        {
            return _transitionLookup.ContainsKey(source);
        }

        public static string GetTransitionScene(string source)
        {
            if (_transitionLookup.TryGetValue(source, out TransitionPlacement placement))
            {
                return placement.Source.TransitionDef.SceneName;
            }

            //MapModS.Instance.Log("GetTransitionScene null " + source);

            return null;
        }

        public static string GetTransitionDoor(string source)
        {
            if (_transitionLookup.TryGetValue(source, out TransitionPlacement placement))
            {
                return placement.Source.TransitionDef.DoorName;
            }

            //MapModS.Instance.Log("GetTransitionDoor null " + source);

            return null;
        }

        public static string GetAdjacentTransition(string source)
        {
            if (_transitionLookup.TryGetValue(source, out TransitionPlacement placement)
                && placement.Target != null)
            {
                return placement.Target.Name;
            }

            //MapModS.Instance.Log("GetAdjacentTransition null " + source);

            return null;
        }

        public static string GetAdjacentScene(string source)
        {
            if (_transitionLookup.TryGetValue(source, out TransitionPlacement placement)
                && placement.Target != null && placement.Target.TransitionDef != null)
            {
                return placement.Target.TransitionDef.SceneName;
            }

            //MapModS.Instance.Log("GetAdjacentScene null " + source);

            return null;
        }

        public static HashSet<string> GetTransitionsByScene(string scene)
        {
            if (scene != null && _transitionsByScene.ContainsKey(scene))
            {
                if (_transitionsByScene[scene].Count() == 0)
                {
                    MapModS.Instance.LogWarn("Empty?");
                }

                return _transitionsByScene[scene];
            }

            MapModS.Instance.LogWarn("No transitions found for scene " + scene);

            return null;
        }

        public static bool TryGetSceneWaypoint(string scene, out LogicWaypoint waypoint)
        {
            if (_waypointScenes.ContainsKey(scene))
            {
                waypoint = _waypointScenes[scene];
                return true;
            }

            waypoint = null;
            return false;
        }

        public static string GetUncheckedVisited(string scene)
        {
            string text = "";

            IEnumerable<string> uncheckedTransitions = RM.RS.TrackerData.uncheckedReachableTransitions
                .Where(t => GetTransitionScene(t) == scene);

            if (uncheckedTransitions.Any())
            {
                text += $"{Localization.Localize("Unchecked")}:";

                foreach (string transition in uncheckedTransitions)
                {
                    text += "\n";

                    if (!RM.RS.TrackerDataWithoutSequenceBreaks.uncheckedReachableTransitions.Contains(transition))
                    {
                        text += "*";
                    }

                    text += GetTransitionDoor(transition);
                }
            }

            Dictionary<string, string> visitedTransitions = RM.RS.TrackerData.visitedTransitions
                .Where(t => GetTransitionScene(t.Key) == scene).ToDictionary(t => t.Key, t => t.Value);

            if (visitedTransitions.Any())
            {
                if (text != "")
                {
                    text += "\n\n";
                }

                text += $"{Localization.Localize("Visited")}:";

                foreach (KeyValuePair<string, string> pair in visitedTransitions)
                {
                    text += "\n";

                    if (RM.RS.TrackerDataWithoutSequenceBreaks.outOfLogicVisitedTransitions.Contains(pair.Key))
                    {
                        text += "*";
                    }

                    text += GetTransitionDoor(pair.Key) + " -> " + pair.Value;
                }
            }

            if (!RM.RS.GenerationSettings.TransitionSettings.Coupled)
            {
                Dictionary<string, string> visitedTransitionsTo = RM.RS.TrackerData.visitedTransitions
                .Where(t => GetTransitionScene(t.Value) == scene).ToDictionary(t => t.Key, t => t.Value);

                if (visitedTransitionsTo.Any())
                {
                    if (text != "")
                    {
                        text += "\n\n";
                    }

                    text += $"{Localization.Localize("Visited to")}:";

                    foreach (KeyValuePair<string, string> pair in visitedTransitionsTo)
                    {
                        text += "\n";

                        if (RM.RS.TrackerDataWithoutSequenceBreaks.outOfLogicVisitedTransitions.Contains(pair.Key))
                        {
                            text += "*";
                        }

                        text += pair.Key + " -> " + GetTransitionDoor(pair.Value);
                    }
                }
            }

            return text;
        }

        public static void SetTransitionLookup()
        {
            _randomizedTransitions = new();
            _transitionLookup = new();
            _transitionsByScene = new();
            _waypointScenes = new();

            if (Ctx.transitionPlacements != null)
            {
                _randomizedTransitions = new(Ctx.transitionPlacements.Select(tp => tp.Source.Name));

                _transitionLookup = Ctx.transitionPlacements.ToDictionary(tp => tp.Source.Name, tp => tp);
            }

            _waypointScenes = Lm.Waypoints.Where(w => RD.IsRoom(w.Name)).ToDictionary(w => w.Name, w => w);

            foreach (GeneralizedPlacement gp in Ctx.Vanilla.Where(gp => RD.IsTransition(gp.Location.Name)))
            {
                RandoModTransition target = new(Lm.GetTransition(gp.Item.Name))
                {
                    TransitionDef = RD.GetTransitionDef(gp.Item.Name)
                };

                RandoModTransition source = new(Lm.GetTransition(gp.Location.Name))
                {
                    TransitionDef = RD.GetTransitionDef(gp.Location.Name)
                };

                _transitionLookup.Add(gp.Location.Name, new(target, source));
            }

            if (Ctx.transitionPlacements != null)
            {
                // Add impossible transitions (because we still need info like scene name etc.)
                foreach (TransitionPlacement tp in Ctx.transitionPlacements)
                {
                    if (!_transitionLookup.ContainsKey(tp.Target.Name))
                    {
                        _transitionLookup.Add(tp.Target.Name, new(null, tp.Target));
                    }
                }
            }

            foreach (GeneralizedPlacement gp in Ctx.Vanilla.Where(gp => RD.IsTransition(gp.Location.Name)))
            {
                if (!_transitionLookup.ContainsKey(gp.Item.Name))
                {
                    RandoModTransition source = new(Lm.GetTransition(gp.Item.Name))
                    {
                        TransitionDef = RD.GetTransitionDef(gp.Item.Name)
                    };

                    _transitionLookup.Add(gp.Item.Name, new(null, source));
                }
            }

            // Get transitions sorted by scene
            _transitionsByScene = new();

            foreach (TransitionPlacement tp in _transitionLookup.Values.Where(tp => tp.Target != null))
            {
                string scene = tp.Source.TransitionDef.SceneName;

                if (!_transitionsByScene.ContainsKey(scene))
                {
                    _transitionsByScene.Add(scene, new() { tp.Source.Name });
                }
                else
                {
                    _transitionsByScene[scene].Add(tp.Source.Name);
                }
            }
        }
    }
}
