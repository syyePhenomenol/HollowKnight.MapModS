using System.Collections.Generic;
using MapChanger;
using RM = RandomizerMod.RandomizerMod;

namespace RandoMapMod.Transition
{
    internal class TransitionTracker : IMainHooks
    {
        internal static HashSet<string> InLogicScenes { get; private set; }
        internal static HashSet<string> OutOfLogicScenes { get; private set; }
        internal static HashSet<string> VisitedAdjacentScenes { get; private set; }
        internal static HashSet<string> UncheckedReachableScenes { get; private set; }

        public void OnEnterGame()
        {
            Update();

            RandomizerMod.IC.TrackerUpdate.OnFinishedUpdate += OnFinishedUpdate;
        }

        public void OnQuitToMenu()
        {
            RandomizerMod.IC.TrackerUpdate.OnFinishedUpdate -= OnFinishedUpdate;
        }

        private void OnFinishedUpdate()
        {
            Update();
        }

        internal static void Update()
        {
            InLogicScenes = new();
            OutOfLogicScenes = new();
            VisitedAdjacentScenes = new();
            UncheckedReachableScenes = new();

            RandomizerCore.Logic.ProgressionManager pm = RM.RS.TrackerData.pm;

            // Get in-logic, out-of-logic, and adjacent visited scenes
            foreach (KeyValuePair<string, RandomizerCore.Logic.LogicTransition> t in RM.RS.TrackerData.lm.TransitionLookup)
            {
                string scene = TransitionData.GetScene(t.Key);

                if (pm.Has(t.Value.term.Id))
                {
                    InLogicScenes.Add(scene);
                }
                else if (PlayerData.instance.scenesVisited.Contains(scene))
                {
                    OutOfLogicScenes.Add(scene);
                }
            }

            VisitedAdjacentScenes = Pathfinder.GetAdjacentReachableScenes(Utils.CurrentScene());

            // Manuallly add Godhome/Tram scenes
            if (pm.lm.TermLookup.ContainsKey("Warp-Godhome_to_Junk_Pit") && pm.Get("Warp-Godhome_to_Junk_Pit") > 0)
            {
                InLogicScenes.Add("GG_Atrium");
            }

            if (pm.lm.TermLookup.ContainsKey("Warp-Junk_Pit_to_Godhome") && pm.Get("Warp-Junk_Pit_to_Godhome") > 0)
            {
                InLogicScenes.Add("GG_Waterways");
            }

            if (pm.lm.TermLookup.ContainsKey("GG_Workshop") && pm.Get("GG_Workshop") > 0)
            {
                InLogicScenes.Add("GG_Workshop");
            }

            if (pm.Get("Upper_Tram") > 0)
            {
                InLogicScenes.Add("Room_Tram_RG");
            }

            if (pm.Get("Lower_Tram") > 0)
            {
                InLogicScenes.Add("Room_Tram");
            }

            // Get scenes where there are unchecked reachable transitions
            foreach (string transition in RM.RS.TrackerData.uncheckedReachableTransitions)
            {
                UncheckedReachableScenes.Add(TransitionData.GetScene(transition));
            }
        }
    }
}
