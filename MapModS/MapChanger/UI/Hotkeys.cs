using MagicUI.Core;
using MapChanger.Map;
using UnityEngine;

namespace MapChanger.UI
{
    internal class Hotkeys : IMainHooks
    {
        private static LayoutRoot layout;

        public void OnEnterGame()
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

        public void OnQuitToMenu()
        {
            layout?.Destroy();
            layout = null;
        }
    }
}
