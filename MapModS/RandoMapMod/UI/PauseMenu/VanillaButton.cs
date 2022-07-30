using System.Linq;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Pins;
using RandoMapMod.Settings;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class VanillaButton : MainButton
    {
        public VanillaButton() : base("Vanilla Pins", "RandoMapMod", 0, 2)
        {

        }

        public override void OnClick(Button button)
        {
            RandoMapMod.LS.ToggleVanilla();

            base.OnClick(button);
        }

        public override void Update()
        {
            base.Update();

            string text = $"{L.Localize("Vanilla")}:\n";

            if (RandoMapMod.LS.VanillaOn)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                text += L.Localize("on");
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                text += L.Localize("off");
            }

            if (IsVanillaCustom())
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
                text += $" ({L.Localize("custom")})";
            }

            Button.Content = text;
        }

        private static bool IsVanillaCustom()
        {
            if (RandoMapMod.LS.GroupBy == GroupBySetting.Item)
            {
                if (!RmmPinMaster.VanillaItemPoolGroups.Any()) return false;

                return (!RandoMapMod.LS.VanillaOn && RmmPinMaster.VanillaItemPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                || (RandoMapMod.LS.VanillaOn && RmmPinMaster.VanillaItemPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
            }
            else
            {
                if (!RmmPinMaster.RandoLocationPoolGroups.Any()) return false;

                return (!RandoMapMod.LS.VanillaOn && RmmPinMaster.VanillaLocationPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                || (RandoMapMod.LS.VanillaOn && RmmPinMaster.VanillaLocationPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
            }
        }
    }
}
