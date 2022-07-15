using MagicUI.Core;
using MapChanger.Map;
using UnityEngine;

namespace MapChanger.UI
{
    internal class Hotkeys : HookModule
    {
        private static LayoutRoot layout;

        public override void Hook()
        {
            if (layout == null)
            {
                layout = new(true, "Hotkeys");

                layout.ListenForHotkey(KeyCode.M, () =>
                {
                    Settings.ToggleMode();
                }, ModifierKeys.Ctrl);

                layout.ListenForHotkey(KeyCode.E, () =>
                {
                    BuiltInObjects.ExportRoomTextDefs();
                }, ModifierKeys.Ctrl);
            }
        }

        public override void Unhook()
        {
            layout?.Destroy();
            layout = null;
        }
    }
}
