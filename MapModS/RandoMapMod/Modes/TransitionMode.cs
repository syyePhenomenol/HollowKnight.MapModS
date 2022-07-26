using System;
using MapChanger;
using MapChanger.MonoBehaviours;

namespace RandoMapMod.Modes
{
    internal abstract class TransitionMode : MapMode
    {
        public override bool ForceHasMap => true;
        public override bool ForceHasQuill => true;
        public override OverrideType VanillaPins => OverrideType.ForceOff;
        public override OverrideType MapMarkers => OverrideType.ForceOff;
        public override bool ImmediateMapUpdate => true;
        public override bool FullMap => true;
        public override bool DisableAreaNames => true;
        public override bool DisableNextArea => true;
        public override bool EnableExtraRoomNames => true;
        public override Func<RoomSprite, bool> OnRoomSpriteSet => SetColor;

        private bool SetColor(RoomSprite roomSprite)
        {
            return true;
        }
    }
}
