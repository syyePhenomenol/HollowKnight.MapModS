using MapChanger.UI;

namespace RandoMapMod.UI
{
    internal class RmmTitle : Title
    {
        public RmmTitle() : base("RandoMapMod")
        {
        }

        public override void Update()
        {
            base.Update();

            TitleText.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
        }
    }
}
