using MagicUI.Elements;
using MapChanger.UI;
using RandoMapMod.Modes;
using RandoMapMod.Transition;

namespace RandoMapMod.UI
{
    internal class WorldMapRouteText: MapUILayer
    {
        internal static WorldMapRouteText Instance;

        private static TextObject instruction;
        private static TextObject routeSummary;

        protected override bool Condition()
        {
            return Conditions.TransitionRandoModeEnabled()
                && MapChanger.States.WorldMapOpen;
        }

        public override void BuildLayout()
        {
            Instance = this;

            instruction = UIExtensions.TextFromEdge(Root, "Benchwarp Text", false);
            routeSummary = UIExtensions.TextFromEdge(Root, "Route Summary", true);
        }

        public override void Update()
        {
            UpdateInstructions();
            UpdateRouteSummary();
        }

        internal static void UpdateInstructions()
        {
            instruction.Visibility = RandoMapMod.GS.RoomSelectionOn ? MagicUI.Core.Visibility.Visible : MagicUI.Core.Visibility.Hidden;
            
            instruction.Text = RouteTracker.GetInstructionText();
        }

        internal static void UpdateRouteSummary()
        {
            routeSummary.Text = RouteTracker.GetSummaryText();
        }
    }
}
