using MagicUI.Core;
using MapChanger.UI;
using RandoMapMod.Modes;
using RandoMapMod.Pins;
using RandoMapMod.Rooms;
using RandoMapMod.Transition;
using UnityEngine;

namespace RandoMapMod.UI
{
    internal class Hotkeys : MapUILayer
    {
        public override void BuildLayout()
        {
            Root.ListenForHotkey(KeyCode.H, () =>
            {
                RandoMapMod.GS.ToggleControlPanel();
                MapUILayerManager.Update();
            }, ModifierKeys.Ctrl);

            Root.ListenForHotkey(KeyCode.K, () =>
            {
                RandoMapMod.GS.ToggleMapKey();
                MapUILayerManager.Update();
            }, ModifierKeys.Ctrl);

            if (Interop.HasBenchwarp())
            {
                Root.ListenForHotkey(KeyCode.W, () =>
                {
                    RandoMapMod.GS.ToggleBenchwarpWorldMap();
                    NormalRoomSelector.Instance.MainUpdate();
                    MapUILayerManager.Update();
                }, ModifierKeys.Ctrl, () => Conditions.NormalModeEnabled());

                Root.ListenForHotkey(KeyCode.B, () =>
                {
                    RandoMapMod.GS.ToggleAllowBenchWarpSearch();
                    RouteTracker.ResetRoute();
                    MapUILayerManager.Update();
                    RouteCompass.Update();
                }, ModifierKeys.Ctrl, () => Conditions.TransitionModeEnabled());
            }

            Root.ListenForHotkey(KeyCode.U, () =>
            {
                RandoMapMod.GS.ToggleUncheckedPanel();
                MapUILayerManager.Update();
            }, ModifierKeys.Ctrl, () => RandoMapMod.LS.ModEnabled);

            Root.ListenForHotkey(KeyCode.R, () =>
            {
                RandoMapMod.GS.ToggleRouteTextInGame();
                MapUILayerManager.Update();
                //TransitionPersistent.UpdateAll();
            }, ModifierKeys.Ctrl, () => RandoMapMod.LS.ModEnabled);

            Root.ListenForHotkey(KeyCode.E, () =>
            {
                RandoMapMod.GS.ToggleWhenOffRoute();
                MapUILayerManager.Update();
            }, ModifierKeys.Ctrl, () => RandoMapMod.LS.ModEnabled);

            Root.ListenForHotkey(KeyCode.C, () =>
            {
                RandoMapMod.GS.ToggleRouteCompassEnabled();
                MapUILayerManager.Update();
            }, ModifierKeys.Ctrl, () => Conditions.TransitionModeEnabled());

            Root.ListenForHotkey(KeyCode.L, () =>
            {
                RandoMapMod.GS.ToggleLookup();
                RmmPinSelector.Instance.MainUpdate();
                MapUILayerManager.Update();
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

        protected override bool Condition()
        {
            return Conditions.RandoMapModEnabled();
        }

        private void UpdatePins()
        {
            PauseMenu.Update();
            RmmPinManager.Update();
            MapUILayerManager.Update();
        }
    }
}
