using MagicUI.Core;
using MagicUI.Elements;
using MapChanger.UI;
using RandoMapMod.Modes;
using RandoMapMod.Transition;

namespace RandoMapMod.UI
{
    internal class RouteSummaryText: MapUILayer
    {
        internal static RouteSummaryText Instance;

        //private static TextObject instruction;
        private static TextObject routeSummary;

        protected override bool Condition()
        {
            return Conditions.TransitionRandoModeEnabled()
                && MapChanger.States.WorldMapOpen;
        }

        public override void BuildLayout()
        {
            Instance = this;

            //instruction = UIExtensions.TextFromEdge(Root, "Benchwarp Text", false);
            routeSummary = UIExtensions.TextFromEdge(Root, "Route Summary", true);
        }

        public override void Update()
        {
            routeSummary.Text = RouteTracker.GetSummaryText();

            //UpdateInstructions();
            //UpdateRouteSummary();
        }

        //internal static void UpdateInstructions()
        //{
        //    instruction.Text = RouteTracker.GetInstructionText();

        //    instruction.Visibility = RandoMapMod.GS.RoomSelectionOn ? Visibility.Visible : Visibility.Hidden;
        //}

        //internal static void UpdateRouteSummary()
        //{
        //    routeSummary.Text = RouteTracker.GetSummaryText();
        //}
    }
}
