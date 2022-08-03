using MagicUI.Core;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;

namespace VanillaMapMod
{
    public class ModEnabledButton : MainButton
    {
        public static ModEnabledButton Instance { get; private set; }

        public ModEnabledButton() : base("Mod Enabled", "VanillaMapMod", 0, 0)
        {
            Instance = this;
        }

        public override void OnClick(Button button)
        {
            MapChanger.Settings.ToggleModEnabled();

            base.OnClick(button);
        }

        public override void Update()
        {
            VanillaMapMod.LS.ModEnabled = MapChanger.Settings.MapModEnabled;

            if (MapChanger.Settings.CurrentMode().Mod is "VanillaMapMod")
            {
                Button.Visibility = Visibility.Visible;
            }
            else
            {
                Button.Visibility = Visibility.Hidden;
            }

            if (VanillaMapMod.LS.ModEnabled)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                Button.Content = "Map Mod\nEnabled";
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Disabled);
                Button.Content = "Map Mod\nDisabled";
            }
        }
    }
}
