using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using VanillaMapMod.Settings;

namespace VanillaMapMod
{
    internal class PinSizeButton : MainButton
    {
        public PinSizeButton() : base("Pin Size", "VanillaMapMod", 0, 1)
        {

        }

        public override void OnClick(Button button)
        {
            VanillaMapMod.GS.TogglePinSize();

            base.OnClick(button);
        }

        public override void Set()
        {
            base.Set();

            string text = $"Pin Size\n";

            switch (VanillaMapMod.GS.PinSize)
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
