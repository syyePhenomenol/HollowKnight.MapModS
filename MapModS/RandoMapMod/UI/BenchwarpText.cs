using MagicUI.Elements;
using MapChanger.UI;
using RandoMapMod.Modes;
using RandoMapMod.Rooms;

namespace RandoMapMod.UI
{
    internal class BenchwarpText: MapUILayer
    {
        internal static BenchwarpText Instance;

        private static TextObject benchwarpText;

        protected override bool Condition()
        {
            return Conditions.NormalModeEnabled()
                && RandoMapMod.GS.BenchwarpWorldMap
                && MapChanger.States.WorldMapOpen;
        }

        public override void BuildLayout()
        {
            Instance = this;

            if (!Interop.HasBenchwarp()) return;

            benchwarpText = UIExtensions.TextFromEdge(Root, "Benchwarp Text", false);
        }

        internal void Update(string text)
        {
            if (!Interop.HasBenchwarp()) return;

            benchwarpText.Text = text;
        }
    }
}
