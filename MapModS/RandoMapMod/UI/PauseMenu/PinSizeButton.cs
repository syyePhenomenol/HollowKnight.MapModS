using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Settings;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class PinSizeButton : MainButton
    {
        public static PinSizeButton Instance { get; private set; }

        public PinSizeButton() : base("Pin Size", "RandoMapMod", 1, 2)
        {
            Instance = this;
        }

        public override void OnClick(Button button)
        {
            RandoMapMod.GS.TogglePinSize();

            base.OnClick(button);
        }

        public override void Update()
        {
            base.Update();

            string text = $"{L.Localize("Pin Size")}:\n";

            switch (RandoMapMod.GS.PinSize)
            {
                case PinSize.Small:
                    text += L.Localize("small");
                    break;

                case PinSize.Medium:
                    text += L.Localize("medium");
                    break;

                case PinSize.Large:
                    text += L.Localize("large");
                    break;
            }

            Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            Button.Content = text;
        }
    }
}
