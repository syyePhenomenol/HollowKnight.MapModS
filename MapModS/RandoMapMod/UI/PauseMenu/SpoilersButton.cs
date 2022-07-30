using MagicUI.Elements;
using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class SpoilersButton : MainButton
    {
        public SpoilersButton() : base("Spoilers", "RandoMapMod", 0, 3)
        {

        }

        public override void OnClick(Button button)
        {
            RandoMapMod.LS.ToggleSpoilers();

            base.OnClick(button);
        }

        public override void Update()
        {
            base.Update();

            Button.BorderColor = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            if (RandoMapMod.LS.SpoilerOn)
            {
                Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_On);
                Button.Content = $"{L.Localize("Spoilers")}:\n{L.Localize("on")}";
            }
            else
            {
                Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
                Button.Content = $"{L.Localize("Spoilers")}:\n{L.Localize("off")}";
            }
        }
    }
}
