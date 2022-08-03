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
        public override Action<AreaName> OnAreaNameUpdateColor => (areaName) => { areaName.Color = RmmColors.GetColor(areaName.MiscObjectDef.ColorSetting).ToOpaque(); };
        public override Action<NextArea> OnNextAreaUpdateColor => (nextArea) => { nextArea.Color = RmmColors.GetColor(nextArea.MiscObjectDef.ColorSetting).ToOpaque(); };
        public override Action<QuickMapTitle, MapZone> OnQuickMapTitleUpdateColor => (qmt, mapZone) => { qmt.Color = RmmColors.GetColorFromMapZone(mapZone).ToOpaque(); };
    }
}
