using MapChanger;

namespace VanillaMapMod
{
    internal class NormalMode : MapMode
    {
        public override string Mod => "VanillaMapMod";
        public override string ModeName => "Normal";
        public override OverrideType MapMarkers => OverrideType.ForceOff;
        public override bool ImmediateMapUpdate => true;
    }
}
