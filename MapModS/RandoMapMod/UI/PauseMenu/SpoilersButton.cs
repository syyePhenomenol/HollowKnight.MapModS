using MagicUI.Elements;
using MapChanger;
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

            if (RandoMapMod.LS.SpoilerOn)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                Button.Content = $"{L.Localize("Spoilers")}:\n{L.Localize("on")}";
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                Button.Content = $"{L.Localize("Spoilers")}:\n{L.Localize("off")}";
            }
        }
    }
}
