using System;
using System.Collections.Generic;
using System.Linq;
using MapChanger.MonoBehaviours;
using Modding;

namespace MapChanger
{
    public class Settings : IMainHooks
    {
        public static event Action OnSettingChanged;
        public static bool MapModEnabled { get; private set; } = false;

        private static List<MapMode> Modes = new ();

        private static int modeIndex = 0;

        public void OnEnterGame()
        {

        }

        public void OnQuitToMenu()
        {
            Modes = new();
        }

        internal static void AddModes(MapMode[] modes)
        {
            foreach (MapMode mode in modes)
            {
                if (Modes.Any(existingMode => existingMode.ModeKey == mode.ModeKey))
                {
                    MapChangerMod.Instance.LogDebug($"A mode with the same key has already been added! {mode.ModeKey}");
                    continue;
                }

                Modes.Add(mode);
            }
        }

        public static void ToggleModEnabled()
        {
            MapModEnabled = !MapModEnabled;

            SettingChanged();
        }

        public static void SetModEnabled(bool enabled)
        {
            if (MapModEnabled != enabled)
            {
                ToggleModEnabled();
            }
        }

        public static void ToggleMode()
        {
            if (!MapModEnabled) return;

            modeIndex = (modeIndex + 1) % Modes.Count;

            SettingChanged();

            MapChangerMod.Instance.LogDebug($"Mode set to {CurrentMode().ModeKey}");
        }

        public static void SetMode(string mod, string modeName)
        {
            if (!MapModEnabled) return;

            for (int i = 0; i < Modes.Count; i++)
            {
                if (Modes[i].ModeKey == (mod, modeName))
                {
                    modeIndex = i;
                }
            }

            SettingChanged();

            MapChangerMod.Instance.LogDebug($"Mode set to {CurrentMode().ModeKey}");
        }

        public static MapMode CurrentMode()
        {
            if (!Modes.Any())
            {
                return new();
            }
            if (modeIndex > Modes.Count)
            {
                MapChangerMod.Instance.LogWarn("Mode index overflow");
                modeIndex = 0;
            }
            return Modes[modeIndex];
        }

        private static void SettingChanged()
        {
            try { OnSettingChanged?.Invoke(); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }
    }
}
