using MagicUI.Elements;
using MapChanger.UI;
using RandoMapMod.Settings;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class GroupByButton : MainButton
    {
        internal static GroupByButton Instance { get; private set; }

        public GroupByButton() : base("Group By Button", "RandoMapMod", 2, 3)
        {

        }

        public override void OnClick(Button button)
        {
            RandoMapMod.LS.ToggleGroupBy();

            base.OnClick(button);
        }

        public override void Update()
        {
            base.Update();

            Button.BorderColor = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            string text = $"{L.Localize("Group by")}:\n";

            switch (RandoMapMod.LS.GroupBy)
            {
                case GroupBySetting.Location:
                    text += L.Localize("Location");
                    break;

                case GroupBySetting.Item:
                    text += L.Localize("Item");
                    break;
            }

            Button.Content = text;
            Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
        }
    }
}
