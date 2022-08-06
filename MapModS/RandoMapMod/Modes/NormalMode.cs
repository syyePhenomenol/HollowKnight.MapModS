using System;
using GlobalEnums;
using MapChanger;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace RandoMapMod.Modes
{
    internal class NormalMode : RmmMapMode
    {
        public override Action<RoomSprite> OnRoomUpdateColor => (roomSprite) =>
        {
            if (roomSprite.Selected)
            {
                roomSprite.Color = RmmColors.GetColor(RmmColorSetting.Room_Benchwarp_Selected);
            }
            else
            {
                roomSprite.Color = RmmColors.GetColor(roomSprite.Rsd.ColorSetting);
            }
        };
        public override Func<RoomSprite, bool> RoomSpriteCanSelectOverride => (roomSprite) =>
        {
            return roomSprite.gameObject.activeSelf && BenchwarpInterop.Benches.ContainsKey(roomSprite.Rsd.SceneName);
        };
        public override Action<AreaName> OnAreaNameUpdateColor => (areaName) => { areaName.Color = RmmColors.GetColor(areaName.MiscObjectDef.ColorSetting).ToOpaque(); };
        public override Action<NextArea> OnNextAreaUpdateColor => (nextArea) => { nextArea.Color = RmmColors.GetColor(nextArea.MiscObjectDef.ColorSetting).ToOpaque(); };
        public override Action<QuickMapTitle, MapZone> OnQuickMapTitleUpdateColor => (qmt, mapZone) => { qmt.Color = RmmColors.GetColorFromMapZone(mapZone).ToOpaque(); };
        
    }

    internal class FullMapMode : NormalMode
    {
        public override string Mod => "RandoMapMod";
        public override string ModeName => "Full Map";
    }

    internal class AllPinsMode : NormalMode
    {
        public override string Mod => "RandoMapMod";
        public override string ModeName => "All Pins";
        public override bool FullMap => false;
    }

    internal class PinsOverMapMode : NormalMode
    {
        public override string Mod => "RandoMapMod";
        public override string ModeName => "Pins Over Map";
        public override bool FullMap => false;
    }
}
