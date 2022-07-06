using GlobalEnums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapChanger
{
    public enum ColorSetting
    {
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

    public static class Colors
    {
        public static readonly Dictionary<string, ColorSetting> mapColors = new()
        {
            { "Ancient Basin", ColorSetting.Map_Ancient_Basin },
            { "City of Tears", ColorSetting.Map_City_of_Tears },
            { "Crystal Peak", ColorSetting.Map_Crystal_Peak },
            { "Deepnest", ColorSetting.Map_Deepnest },
            { "Town_Tutorial", ColorSetting.Map_Dirtmouth },
            { "Crossroads", ColorSetting.Map_Forgotten_Crossroads },
            { "Fog_Canyon", ColorSetting.Map_Fog_Canyon },
            { "Fungal Wastes", ColorSetting.Map_Fungal_Wastes },
            { "GODS_GLORY", ColorSetting.Map_Godhome },
            { "Green_Path", ColorSetting.Map_Greenpath },
            { "Cliffs", ColorSetting.Map_Howling_Cliffs },
            { "Kingdoms_Edge", ColorSetting.Map_Kingdoms_Edge },
            { "Queens_Gardens", ColorSetting.Map_Queens_Gardens },
            { "Resting_Grounds", ColorSetting.Map_Resting_Grounds },
            { "Waterways", ColorSetting.Map_Royal_Waterways },
            { "WHITE_PALACE", ColorSetting.Map_White_Palace },
        };

        // When iterating through map objects, these are the first objects with unique sub area colors
        public static readonly Dictionary<string, ColorSetting> subAreaMapColorKeys = new()
        {
            { "Abyss_01", ColorSetting.Map_Ancient_Basin },
            { "Waterways_13", ColorSetting.Map_Ismas_Grove },
            { "Abyss_02", ColorSetting.Map_Ancient_Basin },
            { "Hive_01", ColorSetting.Map_Hive },
            { "Fungus2_01", ColorSetting.Map_Queens_Station },
            { "Fungus2_12", ColorSetting.Map_Mantis_Village },
            { "Fungus2_13", ColorSetting.Map_Mantis_Village },
            { "Tutorial_01", ColorSetting.Map_Howling_Cliffs },
            { "Abyss_06_Core", ColorSetting.Map_Abyss },
            { "Abyss_06_Core_b", ColorSetting.Map_Abyss },
            { "Ruins1_09", ColorSetting.Map_Soul_Sanctum },
            { "Ruins1_23", ColorSetting.Map_Soul_Sanctum },
            { "Ruins2_01", ColorSetting.Map_Watchers_Spire },

        };

        // Dynamically get pairings of sub area objects/color setting. Key is (parent name, original color)
        public static Dictionary<(string, Vector4), ColorSetting> subAreaMapColors = new();

        public static readonly List<ColorSetting> pinColors = new()
        {
            ColorSetting.Pin_Normal,
            ColorSetting.Pin_Previewed,
            ColorSetting.Pin_Out_of_logic,
            ColorSetting.Pin_Persistent
        };

        public static readonly List<ColorSetting> roomColors = new()
        {
            ColorSetting.Room_Normal,
            ColorSetting.Room_Current,
            ColorSetting.Room_Adjacent,
            ColorSetting.Room_Out_of_logic,
            ColorSetting.Room_Selected
        };

        private static Dictionary<ColorSetting, Vector4> customColors = new();

        private static readonly Dictionary<ColorSetting, Vector4> defaultColors = new()
        {
            { ColorSetting.UI_On, Color.green},
            { ColorSetting.UI_Neutral, Color.white },
            { ColorSetting.UI_Custom, Color.yellow },
            { ColorSetting.UI_Disabled, Color.red },
            { ColorSetting.UI_Special, Color.cyan },
            { ColorSetting.UI_Borders, Color.white },
            { ColorSetting.Pin_Normal, Color.white },
            { ColorSetting.Pin_Previewed, Color.green },
            { ColorSetting.Pin_Out_of_logic, Color.red },
            { ColorSetting.Pin_Persistent, Color.cyan },
            { ColorSetting.UI_Compass, new(1f, 1f, 1f, 0.83f) },
            { ColorSetting.Room_Normal, new(1f, 1f, 1f, 0.3f) }, // white
            { ColorSetting.Room_Current, new(0, 1f, 0, 0.4f) }, // green
            { ColorSetting.Room_Adjacent, new(0, 1f, 1f, 0.4f) }, // cyan
            { ColorSetting.Room_Out_of_logic, new(1f, 0, 0, 0.3f) }, // red
            { ColorSetting.Room_Selected, new(1f, 1f, 0, 0.7f) }, // yellow
            { ColorSetting.Room_Benchwarp_Selected, new(1f, 1f, 0.2f, 1f) }, // yellow
            { ColorSetting.Room_Debug, new(0, 0, 1f, 0.5f) } // blue
        };

        public static void LoadCustomColors()
        {
            Dictionary<string, float[]> customColorsRaw;

            try
            {
                 customColorsRaw = JsonUtil.DeserializeFromExternalFile<Dictionary<string, float[]>>("colors.json");
            }
            catch (Exception)
            {
                MapChangerMod.Instance.LogError("Invalid colors.json file. Using default colors");
                return;
            }

            customColors = new();

            if (customColorsRaw != null)
            {
                foreach (string colorSettingRaw in customColorsRaw.Keys)
                {
                    if (!Enum.TryParse(colorSettingRaw, out ColorSetting colorSetting)) continue;
                    
                    if (customColors.ContainsKey(colorSetting)) continue;

                    float[] rgba = customColorsRaw[colorSettingRaw];

                    if (rgba == null || rgba.Length < 4) continue;

                    Vector4 color = new(rgba[0] / 256f, rgba[1] / 256f, rgba[2] / 256f, rgba[3]);

                    customColors.Add(colorSetting, color);
                }

                MapChangerMod.Instance.Log("Custom colors loaded");
            }
            else
            {
                MapChangerMod.Instance.Log("No colors.json found. Using default colors");
            }
        }

        public static Vector4 GetColor(ColorSetting colorSetting)
        {
            if (customColors != null && customColors.ContainsKey(colorSetting))
            {
                return customColors[colorSetting];
            }

            if (defaultColors.ContainsKey(colorSetting))
            {
                return defaultColors[colorSetting];
            }

            return Vector4.negativeInfinity;
        }

        public static Vector4 GetColorFromMapZone(MapZone mapZone)
        {
            Vector4 color = mapZone switch
            {
                MapZone.ABYSS => GetColor(ColorSetting.Map_Ancient_Basin),
                MapZone.CITY => GetColor(ColorSetting.Map_City_of_Tears),
                MapZone.CLIFFS => GetColor(ColorSetting.Map_Howling_Cliffs),
                MapZone.CROSSROADS => GetColor(ColorSetting.Map_Forgotten_Crossroads),
                MapZone.MINES => GetColor(ColorSetting.Map_Crystal_Peak),
                MapZone.DEEPNEST => GetColor(ColorSetting.Map_Deepnest),
                MapZone.TOWN => GetColor(ColorSetting.Map_Dirtmouth),
                MapZone.FOG_CANYON => GetColor(ColorSetting.Map_Fog_Canyon),
                MapZone.WASTES => GetColor(ColorSetting.Map_Fungal_Wastes),
                MapZone.GREEN_PATH => GetColor(ColorSetting.Map_Greenpath),
                MapZone.OUTSKIRTS => GetColor(ColorSetting.Map_Kingdoms_Edge),
                MapZone.ROYAL_GARDENS => GetColor(ColorSetting.Map_Queens_Gardens),
                MapZone.RESTING_GROUNDS => GetColor(ColorSetting.Map_Resting_Grounds),
                MapZone.WATERWAYS => GetColor(ColorSetting.Map_Royal_Waterways),
                MapZone.WHITE_PALACE => GetColor(ColorSetting.Map_White_Palace),
                MapZone.GODS_GLORY => GetColor(ColorSetting.Map_Godhome),
                _ => Color.white
            };

            color.w = 1f;
            return color;
        }

        public static Vector4 GetColorFromMapZone(string mapZoneString)
        {
            if (Enum.TryParse(mapZoneString, out MapZone mapZone))
            {
                return GetColorFromMapZone(mapZone);
            }
            return Color.white;
        }
    }
}
