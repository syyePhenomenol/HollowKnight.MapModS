using System;
using GlobalEnums;
using MapChanger;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace RandoMapMod.Modes
{
    internal class NormalMode : RmmMapMode
    {
        public override Vector4? RoomColorOverride(RoomSprite roomSprite)
        {
            return roomSprite.Selected ? RmmColors.GetColor(RmmColorSetting.Room_Benchwarp_Selected) : RmmColors.GetColor(roomSprite.Rsd.ColorSetting);
        }

        public override bool? RoomCanSelectOverride(RoomSprite roomSprite)
        {
            return roomSprite.gameObject.activeSelf && BenchwarpInterop.Benches.ContainsKey(roomSprite.Rsd.SceneName);
        }

        public override Vector4? AreaNameColorOverride(AreaName areaName)
        {
            return RmmColors.GetColor(areaName.MiscObjectDef.ColorSetting).ToOpaque();
        }

        public override Vector4? NextAreaColorOverride(NextArea nextArea)
        {
            return RmmColors.GetColor(nextArea.MiscObjectDef.ColorSetting).ToOpaque();
        }

        public override Vector4? QuickMapTitleColorOverride(QuickMapTitle qmt)
        {
            return RmmColors.GetColorFromMapZone(Finder.GetCurrentMapZone()).ToOpaque();
        }
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
