using System;
using System.Collections.Generic;
using System.Linq;
using MapChanger.MonoBehaviours;
using MapChanger.UI;

namespace MapChanger
{
    public record MapModeSetting
    {
        public (string, string) ModeKey => (Mod, ModeName);
        public string Mod { get; init; } = "MapChangerMod";
        public string ModeName { get; init; } = "Disabled";
        public bool ForceHasMap { get; init; } = false;
        public bool ForceHasQuill { get; init; } = false;
        /// <summary>
        /// Determines if the map immediately gets filled in when visiting a new scene with Quill.
        /// </summary>
        public bool ImmediateMapUpdate { get; init; } = false;
        public bool ForceFullMap { get; init; } = false;
        public bool DisableAreaNames { get; init; } = false;
        public bool DisableNextArea { get; init; } = false;
        public bool DisableVanillaPins { get; init; } = false;
        public bool EnableCustomColors { get; init; } = false;
        public bool EnableExtraRoomNames { get; init; } = false;
        public Func<RoomSprite, bool> OnRoomSpriteSet { get; init; }
        public Func<RoomText, bool> OnRoomTextSet { get; init; }
    }

    public static class Settings
    {
        public static bool MapModEnabled { get; private set; } = false;

        private static List<MapModeSetting> Modes;

        private static int modeIndex = 0;

        internal static void Initialize()
        {
            Modes = new();
        }

        public static void AddModes(MapModeSetting[] modes)
        {
            foreach (MapModeSetting mode in modes)
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

            UIManager.instance.checkpointSprite.Show();
            UIManager.instance.checkpointSprite.Hide();

            if (MapModEnabled)
            {
                MapChangerMod.Instance.LogDebug($"MapMod Enabled");
            }
            else
            {
                MapChangerMod.Instance.LogDebug($"MapMod Disabled");
            }
        }

        public static void ToggleMode()
        {
            if (!MapModEnabled) return;

            modeIndex = (modeIndex + 1) % Modes.Count;

            MapChangerMod.Instance.LogDebug($"Mode set to {CurrentMode().ModeName}");
        }

        public static MapModeSetting CurrentMode()
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
    }
}
