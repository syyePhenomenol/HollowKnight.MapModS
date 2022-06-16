using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapModS.Data
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
        Room_Normal,
        Room_Current,
        Room_Adjacent,
        Room_Out_of_logic,
        Room_Selected,
        Room_Benchwarp_Selected,
        Room_Debug
    }

    internal class Colors
    {
        private static readonly Dictionary<ColorSetting, Vector4> customColors = new();

        private static readonly Dictionary<ColorSetting, Vector4> defaultColors = new()
        {
            { ColorSetting.UI_On, Color.green},
            { ColorSetting.UI_Neutral, Color.white },
            { ColorSetting.UI_Custom, Color.yellow },
            { ColorSetting.UI_Disabled, Color.red },
            { ColorSetting.UI_Special, Color.cyan },
            { ColorSetting.UI_Borders, Color.white },
            { ColorSetting.UI_Compass, new(1f, 1f, 1f, 0.83f) },
            { ColorSetting.Room_Normal, new(1f, 1f, 1f, 0.3f) }, // white
            { ColorSetting.Room_Current, new(0, 1f, 0, 0.4f) }, // green
            { ColorSetting.Room_Adjacent, new(0, 1f, 1f, 0.4f) }, // cyan
            { ColorSetting.Room_Out_of_logic, new(1f, 0, 0, 0.3f) }, // red
            { ColorSetting.Room_Selected, new(1f, 1f, 0, 0.7f) }, // yellow
            { ColorSetting.Room_Benchwarp_Selected, new(1f, 1f, 0.2f, 1f) }, // yellow
            { ColorSetting.Room_Debug, new(0, 0, 1f, 0.5f) } // blue
        };

        public static readonly List<ColorSetting> roomColors = new()
        {
            ColorSetting.Room_Normal,
            ColorSetting.Room_Current,
            ColorSetting.Room_Adjacent,
            ColorSetting.Room_Out_of_logic,
            ColorSetting.Room_Selected
        };

        public static void LoadCustomColors()
        {
            Dictionary<string, float[]> customColorsRaw = JsonUtil.DeserializeFromExternalFile<Dictionary<string, float[]>>("colors.json");

            if (customColors != null)
            {
                MapModS.Instance.Log("Custom colors loaded");

                foreach (string colorSettingRaw in customColorsRaw.Keys)
                {
                    if (!Enum.TryParse(colorSettingRaw, out ColorSetting colorSetting)) continue;

                    float[] rgba = customColorsRaw[colorSettingRaw];

                    if (rgba == null || rgba.Length < 4) continue;

                    Vector4 vec = new(rgba[0] / 256f, rgba[1] / 256f, rgba[2] / 256f, rgba[3]);

                    customColors.Add(colorSetting, vec);
                }
            }
            else
            {
                MapModS.Instance.Log("colors.json missing or invalid. Using default colors");
            }
        }

        public static Vector4 GetColor(ColorSetting colorSetting)
        {
            if (customColors != null && customColors.ContainsKey(colorSetting))
            {
                return customColors[colorSetting];
            }

            return defaultColors[colorSetting];
        }
    }
}
