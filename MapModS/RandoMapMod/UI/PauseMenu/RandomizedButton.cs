using System.Linq;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Pins;
using RandoMapMod.Settings;

namespace RandoMapMod.UI
{
    internal class RandomizedButton : MainButton
    {
        public RandomizedButton() : base("Randomized Pins", "RandoMapMod", 0, 1)
        {

        }

        public override void OnClick(Button button)
        {
            RandoMapMod.LS.ToggleRandomized();

            base.OnClick(button);
        }

        public override void Update()
        {
            base.Update();

            string text = $"Randomized:\n";

            if (RandoMapMod.LS.RandomizedOn)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                text += "On";
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                text += "Off";
            }

            if (IsRandomizedCustom())
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
                text += $" (custom)";
                return;
            }

            Button.Content = text;
        }

        private static bool IsRandomizedCustom()
        {
            if (!RmmPinMaster.RandoPoolGroups.Any()) return false;

            return (!RandoMapMod.LS.RandomizedOn && RmmPinMaster.RandoPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                || (RandoMapMod.LS.RandomizedOn && RmmPinMaster.RandoPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
        }
    }
}
