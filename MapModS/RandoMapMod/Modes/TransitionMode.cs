using MapChanger;
using MapChanger.MonoBehaviours;
using RandoMapMod.Transition;
using UnityEngine;

namespace RandoMapMod.Modes
{
    internal abstract class TransitionMode : RmmMapMode
    {
        public override bool DisableAreaNames => true;
        public override bool DisableNextArea => true;

        public override bool? RoomActiveOverride(RoomSprite roomSprite)
        {
            return TransitionTracker.GetRoomActive(roomSprite.Rsd.SceneName);
        }

        public override Vector4? RoomColorOverride(RoomSprite roomSprite)
        {
            return roomSprite.Selected ? RmmColors.GetColor(RmmColorSetting.Room_Selected) : TransitionTracker.GetRoomColor(roomSprite.Rsd.SceneName);
        }

        public override Vector4? QuickMapTitleColorOverride(QuickMapTitle qmt)
        {
            return RmmColors.GetColor(ColorSetting.UI_Neutral);
        }
    }

    internal class TransitionNormalMode : TransitionMode
    {
        public override string Mod => "RandoMapMod";
        public override string ModeName => "Transition Normal";
    }

    internal class TransitionVisitedOnlyMode : TransitionMode
    {
        public override string Mod => "RandoMapMod";
        public override string ModeName => "Transition Visited Only";
    }

    internal class TransitionAllRoomsMode : TransitionMode
    {
        public override string Mod => "RandoMapMod";
        public override string ModeName => "Transition All Rooms";
    }
}
