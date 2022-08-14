using MagicUI.Core;
using MagicUI.Elements;
using MapChanger.UI;
using RandoMapMod.Modes;
using RandoMapMod.Rooms;
using RandoMapMod.Transition;

namespace RandoMapMod.UI
{
    internal class InstructionText: MapUILayer
    {
        internal static InstructionText Instance;

        //private static TextObject instruction;
        private static TextObject routeSummary;

        protected override bool Condition()
        {
            //return Conditions.RandoMapModEnabled() && MapChanger.States.WorldMapOpen;
            return Conditions.TransitionRandoModeEnabled() && MapChanger.States.WorldMapOpen;
        }

        public override void BuildLayout()
        {
            Instance = this;

            //instruction = UIExtensions.TextFromEdge(Root, "Instruction", false);
            routeSummary = UIExtensions.TextFromEdge(Root, "Route Summary", true);
        }

        public override void Update()
        {
            //string text = "";

            //if (Conditions.ItemRandoModeEnabled())
            //{
            //    if (Interop.HasBenchwarp() && RandoMapMod.GS.BenchwarpSelectionOn)
            //    {
            //        text += BenchwarpRoomSelector.Instance.GetInstructionText();
            //    }
            //}

            routeSummary.Text = RouteTracker.GetSummaryText();

            //if (Conditions.TransitionRandoModeEnabled())
            //{
            //    //text += RouteTracker.GetInstructionText();

            //    routeSummary.Visibility = Visibility.Visible;
            //    routeSummary.Text = RouteTracker.GetSummaryText();
            //}
            //else
            //{
            //    routeSummary.Visibility = Visibility.Hidden;
            //}

            //instruction.Text = text;
        }
    }
}
