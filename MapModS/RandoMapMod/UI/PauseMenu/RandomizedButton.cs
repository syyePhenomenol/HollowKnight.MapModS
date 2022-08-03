using System.Linq;
using MagicUI.Elements;
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

            Button.BorderColor = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            string text = $"{L.Localize("Randomized")}:\n";

            if (RandoMapMod.LS.RandomizedOn)
            {
                Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_On);
                text += L.Localize("on");
            }
            else
            {
                Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
                text += L.Localize("off");
            }

            if (IsRandomizedCustom())
            {
                Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Custom);
                text += $" ({L.Localize("custom")})";
            }

            Button.Content = text;
        }

        internal static bool IsRandomizedCustom()
        {
            if (RandoMapMod.LS.GroupBy == GroupBySetting.Item)
            {
                if (!RmmPins.RandoItemPoolGroups.Any()) return false;

                return (!RandoMapMod.LS.RandomizedOn && RmmPins.RandoItemPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                || (RandoMapMod.LS.RandomizedOn && RmmPins.RandoItemPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
            }
            else
            {
                if (!RmmPins.RandoLocationPoolGroups.Any()) return false;

                return (!RandoMapMod.LS.RandomizedOn && RmmPins.RandoLocationPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                || (RandoMapMod.LS.RandomizedOn && RmmPins.RandoLocationPoolGroups.Any(group => RandoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
            }
        }
    }
}
