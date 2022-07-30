using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Settings;

namespace RandoMapMod.UI
{
    internal class PoolButton : ExtraButton
    {
        internal string PoolGroup { get; init; }

        public PoolButton(string poolGroup) : base(poolGroup, "RandoMapMod")
        {
            PoolGroup = poolGroup;
        }

        public override void OnClick(Button button)
        {
            RandoMapMod.LS.TogglePoolGroupSetting(PoolGroup);

            base.OnClick(button);
        }

        public override void Update()
        {
            Button.Content = PoolGroup.Replace(" ", "\n");
            //+ "\n" + RmmPinMaster.GetPoolGroupCounter(PoolGroup);

            Button.ContentColor = RandoMapMod.LS.GetPoolGroupSetting(PoolGroup) switch
            {
                PoolState.On => RmmColors.GetColor(RmmColorSetting.UI_On),
                PoolState.Off => RmmColors.GetColor(RmmColorSetting.UI_Neutral),
                PoolState.Mixed => RmmColors.GetColor(RmmColorSetting.UI_Custom),
                _ => RmmColors.GetColor(RmmColorSetting.UI_On)
            };
        }
    }
}
