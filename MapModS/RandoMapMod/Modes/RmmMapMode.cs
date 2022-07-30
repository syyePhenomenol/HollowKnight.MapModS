using System;
using GlobalEnums;
using MapChanger;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace RandoMapMod.Modes
{
    internal class RmmMapMode : MapMode
    {
        public override bool ForceHasMap => true;
        public override bool ForceHasQuill => true;
        public override OverrideType VanillaPins => OverrideType.ForceOff;
        public override OverrideType MapMarkers => OverrideType.ForceOff;
        public override bool ImmediateMapUpdate => true;
        public override bool FullMap => true;

        public override Action<RoomSprite> OnRoomUpdateColor => (roomSprite) => { roomSprite.Color = RmmColors.GetColor(roomSprite.Rsd.ColorSetting); };
        public override Action<AreaName> OnAreaNameUpdateColor => (areaName) => { areaName.Color = ToOpaque(RmmColors.GetColor(areaName.MiscObjectDef.ColorSetting)); };
        public override Action<NextArea> OnNextAreaUpdateColor => (nextArea) => { nextArea.Color = ToOpaque(RmmColors.GetColor(nextArea.MiscObjectDef.ColorSetting)); };
        public override Action<QuickMapTitle, MapZone> OnQuickMapTitleUpdateColor => (qmt, mapZone) => { qmt.Color = ToOpaque(RmmColors.GetColorFromMapZone(mapZone)); };

        private Vector4 ToOpaque(Vector4 color)
        {
            return new(color.x, color.y, color.z, 1f);
        }
    }
}
