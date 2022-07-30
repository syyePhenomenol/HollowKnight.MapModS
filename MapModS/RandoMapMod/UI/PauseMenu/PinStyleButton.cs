using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Settings;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class PinStyleButton : MainButton
    {
        public static PinStyleButton Instance { get; private set; }

        public PinStyleButton() : base("Pin Style", "RandoMapMod", 1, 1)
        {
            Instance = this;
        }

        public override void OnClick(Button button)
        {
            RandoMapMod.GS.TogglePinStyle();

            base.OnClick(button);
        }

        public override void Update()
        {
            base.Update();

            string text = $"{L.Localize("Pin Style")}:\n";

            switch (RandoMapMod.GS.PinStyle)
            {
                case PinStyle.Normal:
                    text += L.Localize("normal");
                    break;

                case PinStyle.Q_Marks_1:
                    text += $"{L.Localize("q marks")} 1";
                    break;

                case PinStyle.Q_Marks_2:
                    text += $"{L.Localize("q marks")} 2";
                    break;

                case PinStyle.Q_Marks_3:
                    text += $"{L.Localize("q marks")} 3";
                    break;
            }

            Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            Button.Content = text;
        }
    }
}
