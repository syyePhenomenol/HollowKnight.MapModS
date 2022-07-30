using MagicUI.Core;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;

namespace VanillaMapMod
{
    public class ModeButton : MainButton
    {
        public static ModeButton Instance { get; private set; }

        public ModeButton() : base("Mode", "VanillaMapMod", 1, 0)
        {
            Instance = this;
        }

        public override void OnClick(Button button)
        {
            MapChanger.Settings.ToggleMode();

            base.OnClick(button);
        }

        public override void Update()
        {
            base.Update();

            if (MapChanger.Settings.CurrentMode().Mod is "VanillaMapMod")
            {
                VanillaMapMod.LS.SetMode(MapChanger.Settings.CurrentMode().ModeName);
            }

            string text = "Mode\n";

            switch (VanillaMapMod.LS.Mode)
            {
                case Settings.VMMMode.Normal:
                    Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                    text += "Normal";
                    break;

                case Settings.VMMMode.Full_Map:
                    Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                    text += "Full Map";
                    break;
            }

            Button.Content = text;
        }
    }
}
