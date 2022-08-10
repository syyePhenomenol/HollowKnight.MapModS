using RandoMapMod.Modes;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class BenchwarpSelectionText : ControlPanelText
    {
        private protected override string Name => "Benchwarp Selection";

        private protected override bool ActiveCondition()
        {
            return RandoMapMod.GS.ControlPanelOn && Conditions.ItemRandoModeEnabled();
        }

        private protected override Vector4 GetColor()
        {
            if (Interop.HasBenchwarp())
            {
                return RandoMapMod.GS.BenchwarpSelectionOn ? RmmColors.GetColor(RmmColorSetting.UI_On) : RmmColors.GetColor(RmmColorSetting.UI_Neutral);
            }

            return RmmColors.GetColor(RmmColorSetting.UI_Neutral);
        }

        private protected override string GetText()
        {
            if (Interop.HasBenchwarp())
            {
                string text = $"{L.Localize("Benchwarp selection")} (Ctrl-W): ";
                return text + (RandoMapMod.GS.BenchwarpSelectionOn ? "On" : "Off");
            }

            return "Benchwarp is not installed or outdated";
        }
    }
}
