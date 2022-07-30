using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class PoolsPanelButton : MainButton
    {
        internal static PoolsPanelButton Instance { get; private set; }

        public PoolsPanelButton() : base("Pools Panel Button", "RandoMapMod", 1, 3)
        {

        }

        public override void OnClick(Button button)
        {
            PoolsPanel.Instance.Toggle();

            base.OnClick(button);
        }

        public override void Update()
        {
            base.Update();

            if (PoolsPanel.Instance.ExtraButtonsGrid.Visibility == MagicUI.Core.Visibility.Visible)
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            }

            Button.Content = $"{L.Localize("Customize")}\n{L.Localize("Pins")}";
        }
    }
}
