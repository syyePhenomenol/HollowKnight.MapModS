using MagicUI.Core;
using UnityEngine;

namespace MapChanger.UI
{
    internal class Hotkeys : HookModule
    {
        private static LayoutRoot layout;

        internal override void OnEnterGame()
        {
            if (layout == null)
            {
                layout = new(true, "Hotkeys");

                layout.ListenForHotkey(KeyCode.M, () =>
                {
                    Settings.ToggleModEnabled();
                }, ModifierKeys.Ctrl);

                layout.ListenForHotkey(KeyCode.T, () =>
                {
                    Settings.ToggleMode();
                    //BuiltInObjects.RoomSelector.Toggle();
                }, ModifierKeys.Ctrl);
                layout.ListenForHotkey(KeyCode.E, () =>
                {
                    //BuiltInObjects.ExportDefs();
                }, ModifierKeys.Ctrl);
                //layout.ListenForHotkey(KeyCode.D, () =>
                //{
                //    BuiltInObjects.ImportDefs();
                //}, ModifierKeys.Ctrl);
            }
        }

        internal override void OnQuitToMenu()
        {
            layout?.Destroy();
            layout = null;
        }
    }
}
