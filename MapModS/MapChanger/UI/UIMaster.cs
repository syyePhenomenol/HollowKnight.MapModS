using System.Collections.Generic;
using GlobalEnums;

namespace MapChanger.UI
{
    internal class UIMaster : HookModule
    {
        private static readonly List<UILayer> mapLayers = new();

        internal override void OnEnterGame()
        {
            PauseMenu.OnEnterGame();

            Events.AfterOpenWorldMap += OnOpenWorldMap;
            Events.AfterOpenQuickMap += OnOpenQuickMap;
            Events.BeforeCloseMap += OnCloseMap;
        }

        internal override void OnQuitToMenu()
        {
            PauseMenu.OnQuitToMenu();
            Events.AfterOpenWorldMap += OnOpenWorldMap;
            Events.AfterOpenQuickMap += OnOpenQuickMap;
            Events.BeforeCloseMap += OnCloseMap;

            RemoveMapLayers();
        }

        internal static void AddMapLayer(UILayer layer)
        {
            mapLayers.Add(layer);
            layer.Build();
        }

        internal static void RemoveMapLayers()
        {
            foreach (UILayer layer in mapLayers)
            {
                layer.Destroy();
            }

            mapLayers.Clear();
        }

        private static void OnOpenWorldMap(GameMap obj)
        {
            UpdateMapLayers();
        }

        private static void OnOpenQuickMap(GameMap gameMap, MapZone mapZone)
        {
            UpdateMapLayers();
        }

        private static void OnCloseMap(GameMap obj)
        {
            UpdateMapLayers();
        }

        public static void UpdateMapLayers()
        {
            foreach (UILayer layer in mapLayers)
            {
                layer.Update();
            }
        }
    }
}
