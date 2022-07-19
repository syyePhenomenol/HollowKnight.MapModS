using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    public class ModeButton : MainButton
    {
        public ModeButton() : base("Mode", "MapChangerMod", 1, 0) { }

        public override void OnClick(Button button)
        {
            Settings.ToggleMode();

            base.OnClick(button);
        }

        public override void Set()
        {
            if (Settings.MapModEnabled)
            {
                Button.Visibility = Visibility.Visible;
            }
            else
            {
                Button.Visibility = Visibility.Hidden;
            }

            string text = "Mode\n";
            text += Settings.CurrentMode().ModeName;
            Button.Content = text;
        }
    }
}
