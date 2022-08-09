using MapChanger;
using MapChanger.MonoBehaviours;
using RandoMapMod.Transition;
using UnityEngine;

namespace RandoMapMod.Modes
{
    internal class ItemRandoMode : RmmMapMode
    {
        public override bool InitializeToThis()
        {
            if (TransitionData.IsTransitionRando()) return false;

            if (RandoMapMod.GS.OverrideDefaultMode)
            {
                return ModeName == RandoMapMod.GS.ItemRandoModeOverride.ToString().ToCleanName();
            }
            else
            {
                return ModeName == Settings.RmmMode.Full_Map.ToString().ToCleanName();
            }
        }

        public override Vector4? RoomColorOverride(RoomSprite roomSprite)
        {
            return roomSprite.Selected ? RmmColors.GetColor(RmmColorSetting.Room_Benchwarp_Selected) : RmmColors.GetColor(roomSprite.Rsd.ColorSetting);
        }

        public override bool? RoomCanSelectOverride(RoomSprite roomSprite)
        {
            return roomSprite.gameObject.activeInHierarchy && BenchwarpInterop.Benches.ContainsKey(roomSprite.Rsd.SceneName);
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

    internal class FullMapMode : ItemRandoMode
    {
        public override string Mod => RandoMapMod.MOD;
        public override string ModeName => Settings.RmmMode.Full_Map.ToString().ToCleanName();
    }

    internal class AllPinsMode : ItemRandoMode
    {
        public override string Mod => RandoMapMod.MOD;
        public override string ModeName => Settings.RmmMode.All_Pins.ToString().ToCleanName();
        public override bool FullMap => false;
    }

    internal class PinsOverMapMode : ItemRandoMode
    {
        public override string Mod => RandoMapMod.MOD;
        public override string ModeName => Settings.RmmMode.Pins_Over_Map.ToString().ToCleanName();
        public override bool FullMap => false;
    }
}
