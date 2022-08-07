using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using MapChanger.UI;
using Modding;
using UnityEngine;

namespace MapChanger
{
    public class Settings : HookModule
    {
        public static event Action OnSettingChanged;
        public static bool MapModEnabled { get; private set; } = false;

        private static List<MapMode> Modes = new();

        private static int modeIndex = 0;

        public override void OnEnterGame()
        {

        }

        public override void OnQuitToMenu()
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

            UIManager.instance.checkpointSprite.Show();
            UIManager.instance.checkpointSprite.Hide();

            SettingChanged();
        }

        public static void SetModEnabled(bool value)
        {
            if (MapModEnabled != value)
            {
                MapModEnabled = value;
            }
        }

        public static void InitializeMode(string mod, string modeName)
        {
            for (int i = 0; i < Modes.Count; i++)
            {
                if (Modes[i].ModeKey == (mod, modeName))
                {
                    modeIndex = i;
                }
            }

            MapChangerMod.Instance.LogDebug($"Mode initialized to {CurrentMode().ModeKey}");
        }

        public static void ToggleMode()
        {
            if (!MapModEnabled) return;

            modeIndex = (modeIndex + 1) % Modes.Count;

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
            MapUILayerUpdater.Update();
            MapObjectUpdater.Update();

            if (States.WorldMapOpen)
            {
                GameManager.instance.StartCoroutine(CloseAndOpenWorldMap());
            }

            if (States.QuickMapOpen)
            {
                SetQuickMapButton(false);
            }

            try { OnSettingChanged?.Invoke(); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }

        private static IEnumerator CloseAndOpenWorldMap()
        {
            SetQuickMapButton(true);
            yield return new WaitForSeconds(0.3f);
            GameManager.instance.inventoryFSM.SendEvent("OPEN INVENTORY MAP");
        }

        private static void SetQuickMapButton(bool value)
        {
            InputHandler.Instance.inputActions.quickMap.CommitWithState(value, ReflectionHelper.GetField<OneAxisInputControl, ulong>(InputHandler.Instance.inputActions.quickMap, "pendingTick") + 1, 0);
        }
    }
}
