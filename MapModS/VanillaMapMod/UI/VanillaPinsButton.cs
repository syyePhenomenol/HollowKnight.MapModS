using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;

namespace VanillaMapMod
{
    internal class VanillaPinsButton : MainButton
    {
        public VanillaPinsButton() : base("Vanilla Toggle", "VanillaMapMod", 0, 1)
        {

        }

        public override void OnClick(Button button)
        {
            VanillaMapMod.LS.ToggleVanillaPins();

            base.OnClick(button);
        }

        public override void Set()
        {
            base.Set();

            string text = $"Vanilla Pins:\n";

            if (VanillaMapMod.LS.VanillaPinsOn)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                text += "On";
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                text += "Off";
            }
            
            Button.Content = text;
        }
    }
}
