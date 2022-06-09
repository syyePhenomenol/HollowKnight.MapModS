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

        public static bool IsTransitionRando()
        {
            return RM.RS.GenerationSettings.TransitionSettings.Mode != TM.None
                || (RM.RS.Context.transitionPlacements != null && RM.RS.Context.transitionPlacements.Any());
        }

        public static bool TransitionModeActive()
        {
            return MapModS.LS.modEnabled
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
                return _transitionsByScene[scene];
            }

            //MapModS.Instance.LogWarn("No transitions found for scene " + scene);

            return new();
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

            text += BuildTransitionStringList(visitedTransitions, "Visited", false, text != "");

            Dictionary<string, string> visitedTransitionsTo = RM.RS.TrackerData.visitedTransitions
            .Where(t => GetTransitionScene(t.Value) == scene).ToDictionary(t => t.Key, t => t.Value);

            // Display only one-way transitions in coupled rando
            if (RM.RS.GenerationSettings.TransitionSettings.Coupled)
            {
                visitedTransitionsTo = visitedTransitionsTo.Where(t => !visitedTransitions.ContainsKey(t.Value)).ToDictionary(t => t.Key, t => t.Value);
            }

            text += BuildTransitionStringList(visitedTransitionsTo, "Visited to", true, text != "");

            Dictionary<string, string> vanillaTransitions = RM.RS.Context.Vanilla
                .Where(t => RD.IsTransition(t.Location.Name)
                    && GetTransitionScene(t.Location.Name) == scene
                    && RM.RS.TrackerData.pm.Get(t.Location.Name) > 0)
                .ToDictionary(t => t.Location.Name, t => t.Item.Name);


            text += BuildTransitionStringList(vanillaTransitions, "Vanilla", false, text != "");

            Dictionary<string, string> vanillaTransitionsTo = RM.RS.Context.Vanilla
                .Where(t => RD.IsTransition(t.Location.Name)
                    && GetTransitionScene(t.Item.Name) == scene
                    && RM.RS.TrackerData.pm.Get(t.Item.Name) > 0
                    && !vanillaTransitions.ContainsKey(t.Item.Name))
                .ToDictionary(t => t.Location.Name, t => t.Item.Name);

            text += BuildTransitionStringList(vanillaTransitionsTo, "Vanilla to", true, text != "");

            return text;
        }

        public static string BuildTransitionStringList(Dictionary<string, string> transitions, string subtitle, bool to, bool addNewLines)
        {
            string text = "";

            if (transitions.Any())
            {
                if (addNewLines)
                {
                    text += "\n\n";
                }

                text += $"{Localization.Localize(subtitle)}:";

                foreach (KeyValuePair<string, string> pair in transitions)
                {
                    text += "\n";

                    if (RM.RS.TrackerDataWithoutSequenceBreaks.outOfLogicVisitedTransitions.Contains(pair.Key))
                    {
                        text += "*";
                    }

                    if (to)
                    {
                        text += pair.Key + " -> " + GetTransitionDoor(pair.Value);
                    }
                    else
                    {
                        text += GetTransitionDoor(pair.Key) + " -> " + pair.Value;
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

            if (Ctx.transitionPlacements != null)
            {
                _randomizedTransitions = new(Ctx.transitionPlacements.Select(tp => tp.Source.Name));
                _transitionLookup = Ctx.transitionPlacements.ToDictionary(tp => tp.Source.Name, tp => tp);
            }
            
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
