using MagicUI.Core;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    public class ModEnabledButton : MainButton
    {
        public static ModEnabledButton Instance { get; private set; }

        public ModEnabledButton() : base("Mod Enabled", "RandoMapMod", 0, 0)
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
            RandoMapMod.LS.ModEnabled = MapChanger.Settings.MapModEnabled;

            if (MapChanger.Settings.CurrentMode().Mod is "RandoMapMod")
            {
                Button.Visibility = Visibility.Visible;
            }
            else
            {
                Button.Visibility = Visibility.Hidden;
            }

            if (RandoMapMod.LS.ModEnabled)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                Button.Content = $"{L.Localize("Mod")}\n{L.Localize("Enabled")}";
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Disabled);
                Button.Content = $"{L.Localize("Mod")}\n{L.Localize("Disabled")}";
            }
        }
    }
}
