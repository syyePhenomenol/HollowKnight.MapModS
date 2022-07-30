using System.Linq;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Pins;
using RandoMapMod.Settings;
using L = RandomizerMod.Localization;

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

            string text = $"{L.Localize("Randomized")}:\n";

            if (RandoMapMod.LS.RandomizedOn)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                text += L.Localize("on");
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                text += L.Localize("off");
            }

            if (IsRandomizedCustom())
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
                text += $" ({L.Localize("custom")})";
            }

            Button.Content = text;
        }

        private static bool IsRandomizedCustom()
        {
            if (RandoMapMod.LS.GroupBy == GroupBySetting.Item)
            {
                if (!RmmPinMaster.RandoItemPoolGroups.Any()) return false;

                return (!RandoMapMod.LS.RandomizedOn && RmmPinMaster.RandoItemPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                || (RandoMapMod.LS.RandomizedOn && RmmPinMaster.RandoItemPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
            }
            else
            {
                if (!RmmPinMaster.RandoLocationPoolGroups.Any()) return false;

                return (!RandoMapMod.LS.RandomizedOn && RmmPinMaster.RandoLocationPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                || (RandoMapMod.LS.RandomizedOn && RmmPinMaster.RandoLocationPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
            }
        }
    }
}
