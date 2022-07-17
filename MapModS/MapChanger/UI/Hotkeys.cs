using MagicUI.Core;
using MapChanger.Map;
using UnityEngine;

namespace MapChanger.UI
{
    internal class Hotkeys : HookModule
    {
        private static LayoutRoot layout;

        internal override void Hook()
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
                    BuiltInObjects.ExportDefs();
                }, ModifierKeys.Ctrl);
                //layout.ListenForHotkey(KeyCode.D, () =>
                //{
                //    BuiltInObjects.ImportDefs();
                //}, ModifierKeys.Ctrl);
            }
        }

        internal override void Unhook()
        {
            layout?.Destroy();
            layout = null;
        }
    }
}
