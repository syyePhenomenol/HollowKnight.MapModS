using MagicUI.Elements;
using MapChanger.UI;
using RandoMapMod.Settings;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class ModeButton : MainButton
    {
        public static ModeButton Instance { get; private set; }

        public ModeButton() : base("Mode", "RandoMapMod", 1, 0)
        {
            Instance = this;
        }

        protected override void OnClick()
        {
            MapChanger.Settings.ToggleMode();
        }

        public override void Update()
        {
            base.Update();

            Button.BorderColor = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            string text = $"{L.Localize("Mode")}:\n";

            switch (RandoMapMod.LS.Mode)
            {
                case RMMMode.Full_Map:
                    Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_On);
                    text += L.Localize("Full Map");
                    break;

                case RMMMode.All_Pins:
                    Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
                    text += L.Localize("All Pins");
                    break;

                case RMMMode.Pins_Over_Map:
                    Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
                    text += L.Localize("Pins Over Map");
                    break;

                case RMMMode.Transition_Normal:
                    Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Special);
                    text += L.Localize("Transition") + " 1";
                    break;

                case RMMMode.Transition_Visited_Only:
                    Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Special);
                    text += L.Localize("Transition") + " 2";
                    break;

                case RMMMode.Transition_All_Rooms:
                    Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Special);
                    text += L.Localize("Transition") + " 3";
                    break;
            }

            Button.Content = text;
        }
    }
}
