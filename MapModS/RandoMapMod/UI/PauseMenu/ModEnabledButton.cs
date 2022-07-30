using MagicUI.Core;
using MagicUI.Elements;
using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class ModEnabledButton : MainButton
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

            Button.BorderColor = RmmColors.GetColor(RmmColorSetting.UI_Borders);

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
                Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_On);
                Button.Content = $"{L.Localize("Mod")}\n{L.Localize("Enabled")}";
            }
            else
            {
                Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Disabled);
                Button.Content = $"{L.Localize("Mod")}\n{L.Localize("Disabled")}";
            }
        }
    }
}
