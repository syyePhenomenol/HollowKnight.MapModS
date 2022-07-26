using System.Linq;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;

namespace VanillaMapMod
{
    internal class ModPinsButton : MainButton
    {
        public ModPinsButton() : base("Mod Pins", "VanillaMapMod", 0, 2)
        {

        }

        public override void OnClick(Button button)
        {
            VanillaMapMod.LS.ToggleAllPools();

            base.OnClick(button);
        }

        public override void Set()
        {
            base.Set();

            string text = $"Mod Pins:\n";

            if (VanillaMapMod.LS.PoolSettings.Values.All(value => value))
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                text += "On";
            }
            else if (VanillaMapMod.LS.PoolSettings.Values.All(value => !value))
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                text += "Off";
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
                text += "Custom";
            }
            
            Button.Content = text;
        }
    }
}
