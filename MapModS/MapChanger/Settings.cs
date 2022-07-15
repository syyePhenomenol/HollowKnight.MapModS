using System;
using System.Collections.Generic;
using MapChanger.MonoBehaviours;

namespace MapChanger
{
    public record MapModeSetting
    {
        public string ModeName { get; init; } = "Default";
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
        public Func<RoomSprite, bool> OnRoomSpriteSet { get; init; } = (roomSprite) => { return false; };
        public Func<RoomText, bool> OnRoomTextSet { get; init; } = (roomText) => { return false; };
    }

    public static class Settings
    {
        public static bool MapModEnabled { get; set; }

        public static List<MapModeSetting> Modes;

        private static int modeIndex = 0;

        internal static void Initialize()
        {
            Modes = new() { new MapModeSetting() };
            RoomSprite.OnSet += CurrentMode().OnRoomSpriteSet;
            RoomText.OnSet += CurrentMode().OnRoomTextSet;
        }

        public static void ToggleMode()
        {
            RoomSprite.OnSet -= CurrentMode().OnRoomSpriteSet;
            RoomText.OnSet -= CurrentMode().OnRoomTextSet;

            modeIndex = (modeIndex + 1) % Modes.Count;

            RoomSprite.OnSet += CurrentMode().OnRoomSpriteSet;
            RoomText.OnSet += CurrentMode().OnRoomTextSet;

            MapChangerMod.Instance.LogDebug($"Mode set to {CurrentMode().ModeName}");

            if (modeIndex is 0)
            {
                MapChangerMod.Instance.LogDebug($"MapMod Disabled");
                MapModEnabled = false;
            }
            else
            {
                MapChangerMod.Instance.LogDebug($"MapMod Enabled");
                MapModEnabled = true;
            }
        }

        public static MapModeSetting CurrentMode()
        {
            if (modeIndex > Modes.Count)
            {
                MapChangerMod.Instance.LogWarn("Mode index overflow");
                modeIndex = 0;
            }
            return Modes[modeIndex];
        }
    }
}
