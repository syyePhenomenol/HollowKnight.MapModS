﻿using ConnectionMetadataInjector.Util;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;

namespace VanillaMapMod
{
    internal class PoolButton : ExtraButton
    {
        internal PoolGroup PoolGroup { get; init; }

        public PoolButton(PoolGroup poolGroup) : base(poolGroup.FriendlyName(), "VanillaMapMod")
        {
            PoolGroup = poolGroup;
        }

        public override void OnClick(Button button)
        {
            VanillaMapMod.LS.TogglePoolGroupSetting(PoolGroup);

            base.OnClick(button);
        }

        public override void Update()
        {
            Button.Content = PoolGroup.FriendlyName().Replace(" ", "\n")
                + "\n" + VmmPins.GetPoolGroupCounter(PoolGroup);

            if (VanillaMapMod.LS.GetPoolGroupSetting(PoolGroup))
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_On);
            }
            else
            {
                Button.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            }
        }
    }
}
