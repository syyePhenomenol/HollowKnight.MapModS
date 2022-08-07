using System;
using System.Collections.Generic;
using GlobalEnums;
using MapChanger;
using UnityEngine;

namespace RandoMapMod
{
    public enum RmmColorSetting
    {
        None,

        UI_On,
        UI_Neutral,
        UI_Custom,
        UI_Disabled,
        UI_Special,
        UI_Borders,

        UI_Compass,

        Pin_Normal,
        Pin_Previewed,
        Pin_Out_of_logic,
        Pin_Persistent,

        Map_Ancient_Basin,
        Map_City_of_Tears,
        Map_Crystal_Peak,
        Map_Deepnest,
        Map_Dirtmouth,
        Map_Fog_Canyon,
        Map_Forgotten_Crossroads,
        Map_Fungal_Wastes,
        Map_Godhome,
        Map_Greenpath,
        Map_Howling_Cliffs,
        Map_Kingdoms_Edge,
        Map_Queens_Gardens,
        Map_Resting_Grounds,
        Map_Royal_Waterways,
        Map_White_Palace,

        Map_Abyss,
        Map_Hive,
        Map_Ismas_Grove,
        Map_Mantis_Village,
        Map_Queens_Station,
        Map_Soul_Sanctum,
        Map_Watchers_Spire,

        Room_Normal,
        Room_Current,
        Room_Adjacent,
        Room_Out_of_logic,
        Room_Selected,
        Room_Benchwarp_Selected,
        Room_Debug
    }

    public static class RmmColors
    {
        public static bool HasCustomColors { get; private set; } = false;

        public static readonly Dictionary<string, RmmColorSetting> mapColors = new()
        {
            { "Ancient Basin", RmmColorSetting.Map_Ancient_Basin },
            { "City of Tears", RmmColorSetting.Map_City_of_Tears },
            { "Crystal Peak", RmmColorSetting.Map_Crystal_Peak },
            { "Deepnest", RmmColorSetting.Map_Deepnest },
            { "Town_Tutorial", RmmColorSetting.Map_Dirtmouth },
            { "Crossroads", RmmColorSetting.Map_Forgotten_Crossroads },
            { "Fog_Canyon", RmmColorSetting.Map_Fog_Canyon },
            { "Fungal Wastes", RmmColorSetting.Map_Fungal_Wastes },
            { "GODS_GLORY", RmmColorSetting.Map_Godhome },
            { "Green_Path", RmmColorSetting.Map_Greenpath },
            { "Cliffs", RmmColorSetting.Map_Howling_Cliffs },
            { "Kingdoms_Edge", RmmColorSetting.Map_Kingdoms_Edge },
            { "Queens_Gardens", RmmColorSetting.Map_Queens_Gardens },
            { "Resting_Grounds", RmmColorSetting.Map_Resting_Grounds },
            { "Waterways", RmmColorSetting.Map_Royal_Waterways },
            { "WHITE_PALACE", RmmColorSetting.Map_White_Palace },
        };

        public static readonly List<RmmColorSetting> pinColors = new()
        {
            RmmColorSetting.Pin_Normal,
            RmmColorSetting.Pin_Previewed,
            RmmColorSetting.Pin_Out_of_logic,
            RmmColorSetting.Pin_Persistent
        };

        public static readonly List<RmmColorSetting> roomColors = new()
        {
            RmmColorSetting.Room_Normal,
            RmmColorSetting.Room_Current,
            RmmColorSetting.Room_Adjacent,
            RmmColorSetting.Room_Out_of_logic,
            RmmColorSetting.Room_Selected
        };

        private static Dictionary<RmmColorSetting, Vector4> customColors = new();

        private static readonly Dictionary<RmmColorSetting, Vector4> defaultColors = new()
        {
            { RmmColorSetting.Pin_Normal, Color.white },
            { RmmColorSetting.Pin_Previewed, Color.green },
            { RmmColorSetting.Pin_Out_of_logic, Color.red },
            { RmmColorSetting.Pin_Persistent, Color.cyan },
            { RmmColorSetting.Room_Normal, new(1f, 1f, 1f, 0.3f) }, // white
            { RmmColorSetting.Room_Current, new(0, 1f, 0, 0.4f) }, // green
            { RmmColorSetting.Room_Adjacent, new(0, 1f, 1f, 0.4f) }, // cyan
            { RmmColorSetting.Room_Out_of_logic, new(1f, 0, 0, 0.3f) }, // red
            { RmmColorSetting.Room_Selected, new(1f, 1f, 0, 0.7f) }, // yellow
            { RmmColorSetting.Room_Benchwarp_Selected, new(1f, 1f, 0.2f, 1f) }, // yellow
            { RmmColorSetting.Room_Debug, new(0, 0, 1f, 0.5f) }, // blue
            { RmmColorSetting.UI_Compass, new(1f, 1f, 1f, 0.83f) }
        };

        public static void Load()
        {
            Dictionary<string, float[]> customColorsRaw;

            try
            {
                 customColorsRaw = JsonUtil.DeserializeFromExternalFile<Dictionary<string, float[]>>("colors.json");
            }
            catch (Exception)
            {
                RandoMapMod.Instance.LogError("Invalid colors.json file. Using default colors");
                return;
            }

            customColors = new();

            if (customColorsRaw != null)
            {
                foreach (string colorSettingRaw in customColorsRaw.Keys)
                {
                    if (!Enum.TryParse(colorSettingRaw, out RmmColorSetting colorSetting)) continue;
                    
                    if (customColors.ContainsKey(colorSetting)) continue;

                    float[] rgba = customColorsRaw[colorSettingRaw];

                    if (rgba == null || rgba.Length < 4) continue;

                    Vector4 color = new(rgba[0] / 256f, rgba[1] / 256f, rgba[2] / 256f, rgba[3]);

                    customColors.Add(colorSetting, color);
                }

                MapChangerMod.Instance.Log("Custom colors loaded");
                HasCustomColors = true;
            }
            else
            {
                MapChangerMod.Instance.Log("No colors.json found. Using default colors");
            }
        }

        public static Vector4 GetColor(RmmColorSetting rmmColor)
        {
            if (customColors != null && customColors.ContainsKey(rmmColor))
            {
                return customColors[rmmColor];
            }

            if (defaultColors.ContainsKey(rmmColor))
            {
                return defaultColors[rmmColor];
            }

            if (Enum.TryParse(rmmColor.ToString(), out ColorSetting mcColor))
            {
                return Colors.GetColor(mcColor);
            }

            return Vector4.negativeInfinity;
        }

        public static Vector4 GetColor(ColorSetting mcColor)
        {
            if (Enum.TryParse(mcColor.ToString(), out RmmColorSetting rmmColor))
            {
                return GetColor(rmmColor);
            }

            return Vector4.negativeInfinity;
        }

        public static Vector4 GetColorFromMapZone(MapZone mapZone)
        {
            return mapZone switch
            {
                MapZone.ABYSS => GetColor(RmmColorSetting.Map_Ancient_Basin),
                MapZone.CITY => GetColor(RmmColorSetting.Map_City_of_Tears),
                MapZone.CLIFFS => GetColor(RmmColorSetting.Map_Howling_Cliffs),
                MapZone.CROSSROADS => GetColor(RmmColorSetting.Map_Forgotten_Crossroads),
                MapZone.MINES => GetColor(RmmColorSetting.Map_Crystal_Peak),
                MapZone.DEEPNEST => GetColor(RmmColorSetting.Map_Deepnest),
                MapZone.TOWN => GetColor(RmmColorSetting.Map_Dirtmouth),
                MapZone.FOG_CANYON => GetColor(RmmColorSetting.Map_Fog_Canyon),
                MapZone.WASTES => GetColor(RmmColorSetting.Map_Fungal_Wastes),
                MapZone.GREEN_PATH => GetColor(RmmColorSetting.Map_Greenpath),
                MapZone.OUTSKIRTS => GetColor(RmmColorSetting.Map_Kingdoms_Edge),
                MapZone.ROYAL_GARDENS => GetColor(RmmColorSetting.Map_Queens_Gardens),
                MapZone.RESTING_GROUNDS => GetColor(RmmColorSetting.Map_Resting_Grounds),
                MapZone.WATERWAYS => GetColor(RmmColorSetting.Map_Royal_Waterways),
                MapZone.WHITE_PALACE => GetColor(RmmColorSetting.Map_White_Palace),
                MapZone.GODS_GLORY => GetColor(RmmColorSetting.Map_Godhome),
                _ => Color.white
            };
        }
    }
}
