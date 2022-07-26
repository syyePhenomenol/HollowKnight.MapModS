using MapChanger;

namespace RandoMapMod.Modes
{
    internal class FullMapMode : MapMode
    {
        public override string Mod => "RandoMapMod";
        public override string ModeName => "Full Map";
        public override bool ForceHasMap => true;
        public override bool ForceHasQuill => true;
        public override OverrideType VanillaPins => OverrideType.ForceOff;
        public override OverrideType MapMarkers => OverrideType.ForceOff;
        public override bool ImmediateMapUpdate => true;
        public override bool FullMap => true;
    }
}
