using System.Linq;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Pins;
using RandoMapMod.Settings;

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

            string text = $"Vanilla:\n";

            if (RandoMapMod.LS.VanillaOn)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                text += "On";
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                text += "Off";
            }

            if (IsVanillaCustom())
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
                text += $" (custom)";
                return;
            }

            Button.Content = text;
        }

        private static bool IsVanillaCustom()
        {
            if (!RmmPinMaster.VanillaPoolGroups.Any()) return false;

            return (!RandoMapMod.LS.VanillaOn && RmmPinMaster.VanillaPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                || (RandoMapMod.LS.VanillaOn && RmmPinMaster.VanillaPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
        }
    }
}
