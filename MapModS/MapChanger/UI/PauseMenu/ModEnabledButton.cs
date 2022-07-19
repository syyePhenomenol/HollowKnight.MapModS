using MagicUI.Elements;

namespace MapChanger.UI
{
    public class ModEnabledButton : MainButton
    {
        public ModEnabledButton() : base("Mod Enabled", "MapChangerMod", 0, 0) { }

        public override void OnClick(Button button)
        {
            Settings.ToggleModEnabled();

            base.OnClick(button);
        }

        public override void Set()
        {
            if (Settings.MapModEnabled)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                Button.Content = $"Mod\nEnabled";
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Disabled);
                Button.Content = $"Mod\nDisabled";
            }
        }
    }
}
