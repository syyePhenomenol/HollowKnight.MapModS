using GlobalEnums;
using MapModS.Data;

namespace MapModS.Settings
{
    public static class SettingsUtil
    {
        public static bool GetMapSetting(MapZone mapZone)
        {
            return mapZone switch
            {
                MapZone.ABYSS => PlayerData.instance.GetBool("mapAbyss"),
                MapZone.CITY => PlayerData.instance.GetBool("mapCity"),
                MapZone.CLIFFS => PlayerData.instance.GetBool("mapCliffs"),
                MapZone.CROSSROADS => PlayerData.instance.GetBool("mapCrossroads"),
                MapZone.MINES => PlayerData.instance.GetBool("mapMines"),
                MapZone.DEEPNEST => PlayerData.instance.GetBool("mapDeepnest"),
                MapZone.TOWN => PlayerData.instance.GetBool("mapDirtmouth"),
                MapZone.FOG_CANYON => PlayerData.instance.GetBool("mapFogCanyon"),
                MapZone.WASTES => PlayerData.instance.GetBool("mapFungalWastes"),
                MapZone.GREEN_PATH => PlayerData.instance.GetBool("mapGreenpath"),
                MapZone.OUTSKIRTS => PlayerData.instance.GetBool("mapOutskirts"),
                MapZone.ROYAL_GARDENS => PlayerData.instance.GetBool("mapRoyalGardens"),
                MapZone.RESTING_GROUNDS => PlayerData.instance.GetBool("mapRestingGrounds"),
                MapZone.WATERWAYS => PlayerData.instance.GetBool("mapWaterways"),
                MapZone.WHITE_PALACE => PlayerData.instance.GetBool("AdditionalMapsGotWpMap"),
                MapZone.GODS_GLORY => PlayerData.instance.GetBool("AdditionalMapsGotGhMap"),
                _ => false,
            };
        }

        public static bool IsFSMMapState(string name)
        {
            return name switch
            {
                "Abyss"
                or "Ancient Basin"
                or "City"
                or "Cliffs"
                or "Crossroads"
                or "Deepnest"
                or "Fog Canyon"
                or "Fungal Wastes"
                or "Fungus"
                or "Greenpath"
                or "Hive"
                or "Mines"
                or "Outskirts"
                or "Resting Grounds"
                or "Royal Gardens"
                or "Waterways"
                or "WHITE_PALACE"
                or "GODS_GLORY" => true,
                _ => false,
            };
        }

        public static bool GetPreviewSetting(PreviewGroup previewGroup)
        {
            return previewGroup switch
            {
                PreviewGroup.Colosseum => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.ColosseumPreview,
                PreviewGroup.KingFragment => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.KingFragmentPreview,
                PreviewGroup.FlowerQuest => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.FlowerQuestPreview,
                PreviewGroup.GreyPrinceZote => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.GreyPrinceZotePreview,
                PreviewGroup.WhisperingRoot => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.WhisperingRootPreview,
                PreviewGroup.Dreamer => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.DreamerPreview,
                PreviewGroup.AbyssShriek => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.AbyssShriekPreview,
                PreviewGroup.VoidHeart => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.VoidHeartPreview,
                PreviewGroup.Godtuner => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.GodtunerPreview,
                PreviewGroup.LoreTablet => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.LoreTabletPreview,
                PreviewGroup.BasinFountain => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.BasinFountainPreview,
                PreviewGroup.Nailmaster => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.NailmasterPreview,
                PreviewGroup.Stag => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.StagPreview,
                PreviewGroup.Map => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.MapPreview,
                PreviewGroup.Divine => RandomizerMod.RandomizerMod.RS.GenerationSettings.LongLocationSettings.DivinePreview,
                _ => false,
            };
        }
    }
}