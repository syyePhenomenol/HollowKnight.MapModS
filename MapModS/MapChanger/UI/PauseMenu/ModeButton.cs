using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    public class ModeButton : MainButton
    {
        public static ModeButton Instance { get; private set; }

        public ModeButton() : base("Mode", "MapChangerMod", 1, 0)
        {
            Instance = this;
        }

        public override void OnClick(Button button)
        {
            Settings.ToggleMode();

            base.OnClick(button);
        }

        public override void Update()
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
