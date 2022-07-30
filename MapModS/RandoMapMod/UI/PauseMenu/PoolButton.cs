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
                PoolState.On => Colors.GetColor(ColorSetting.UI_On),
                PoolState.Off => Colors.GetColor(ColorSetting.UI_Neutral),
                PoolState.Mixed => Colors.GetColor(ColorSetting.UI_Custom),
                _ => Colors.GetColor(ColorSetting.UI_On)
            };
        }
    }
}
