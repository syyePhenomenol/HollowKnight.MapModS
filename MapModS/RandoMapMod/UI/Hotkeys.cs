using MagicUI.Core;
using MapChanger.UI;
using UnityEngine;

namespace RandoMapMod.UI
{
    internal class Hotkeys : MapUILayer
    {
        public override void BuildLayout()
        {
            Root.ListenForHotkey(KeyCode.B, () =>
            {
            }, ModifierKeys.Ctrl);

            Root.ListenForHotkey(KeyCode.C, () =>
            {
            }, ModifierKeys.Ctrl);

            Root.ListenForHotkey(KeyCode.H, () =>
            {
            }, ModifierKeys.Ctrl);

            Root.ListenForHotkey(KeyCode.Alpha1, () =>
            {
                RandoMapMod.LS.ToggleSpoilers();
                UpdatePins();
            }, ModifierKeys.Ctrl);

            Root.ListenForHotkey(KeyCode.Alpha2, () =>
            {
                RandoMapMod.LS.ToggleRandomized();
                UpdatePins();
            }, ModifierKeys.Ctrl);

            Root.ListenForHotkey(KeyCode.Alpha3, () =>
            {
                RandoMapMod.LS.ToggleVanilla();
                UpdatePins();
            }, ModifierKeys.Ctrl);

            Root.ListenForHotkey(KeyCode.Alpha4, () =>
            {
                RandoMapMod.GS.TogglePinStyle();
                UpdatePins();
            }, ModifierKeys.Ctrl);

            Root.ListenForHotkey(KeyCode.Alpha5, () =>
            {
                RandoMapMod.GS.TogglePinSize();
                UpdatePins();
            }, ModifierKeys.Ctrl);
        }

        public override bool Condition()
        {
            return MapChanger.Settings.CurrentMode().Mod is "RandoMapMod";
        }

        private void UpdatePins()
        {
            PauseMenu.Update();
            Pins.RmmPinManager.Update();
            MapUILayerManager.Update();
        }
    }
}
