using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Settings;

namespace RandoMapMod.UI
{
    internal class PinSizeButton : MainButton
    {
        public static PinSizeButton Instance { get; private set; }

        public PinSizeButton() : base("Pin Size", "RandoMapMod", 1, 1)
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

            string text = $"Pin Size\n";

            switch (RandoMapMod.GS.PinSize)
            {
                case PinSize.Small:
                    text += "small";
                    break;

                case PinSize.Medium:
                    text += "medium";
                    break;

                case PinSize.Large:
                    text += "large";
                    break;
            }

            Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            Button.Content = text;
        }
    }
}
