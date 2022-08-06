using System;
using GlobalEnums;
using MapChanger;
using MapChanger.MonoBehaviours;
using RandoMapMod.Transition;

namespace RandoMapMod.Modes
{
    internal abstract class TransitionMode : RmmMapMode
    {
        public override bool DisableAreaNames => true;
        public override bool DisableNextArea => true;
        public override Func<RoomSprite, bool> RoomSpriteActiveOverride => (roomSprite) =>
        {
            return TransitionTracker.GetRoomActive(roomSprite.Rsd.SceneName);
        };
        public override Action<RoomSprite> OnRoomUpdateColor => (roomSprite) => 
        {
            if (roomSprite.Selected)
            {
                roomSprite.Color = RmmColors.GetColor(RmmColorSetting.Room_Selected);
            }
            else
            {
                roomSprite.Color = TransitionTracker.GetRoomColor(roomSprite.Rsd.SceneName);
            }
        };
        public override Action<AreaName> OnAreaNameUpdateColor => (areaName) => { areaName.ResetColor(); };
        public override Action<NextArea> OnNextAreaUpdateColor => (nextArea) => { nextArea.ResetColor(); };
        public override Action<QuickMapTitle, MapZone> OnQuickMapTitleUpdateColor => (qmt, mapZone) => { qmt.Color = RmmColors.GetColor(ColorSetting.UI_Neutral); };
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
