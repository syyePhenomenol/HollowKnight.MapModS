﻿using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Settings;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class PersistentButton : MainButton
    {
        public PersistentButton() : base("Persistent Button", "RandoMapMod", 2, 2)
        {

        }

        internal override void Make()
        {
            base.Make();

            Button.Borderless = true;
        }

        protected override void OnClick()
        {
            RandoMapMod.GS.TogglePersistent();
        }

        public override void Update()
        {
            base.Update();

            Button.Visibility = PoolsPanel.Instance.ExtraButtonsGrid.Visibility;

            Button.BorderColor = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            string text = $"{L.Localize("Persistent\nitems")}: ";

            if (RandoMapMod.GS.PersistentOn)
            {
                text += L.Localize("On");
                Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_On);
            }
            else
            {
                text += L.Localize("Off");
                Button.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
            }

            Button.Content = text;
        }
    }
}
