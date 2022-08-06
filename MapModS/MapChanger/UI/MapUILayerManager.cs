using System.Collections.Generic;
using GlobalEnums;

namespace MapChanger.UI
{
    internal class MapUILayerManager : HookModule
    {
        private static readonly List<MapUILayer> mapLayers = new();

        public override void OnEnterGame()
        {
            AddMapLayer(new GlobalHotkeys());

            Events.AfterOpenWorldMap += OnOpenWorldMap;
            Events.AfterOpenQuickMap += OnOpenQuickMap;
            Events.BeforeCloseMap += OnCloseMap;
        }

        public override void OnQuitToMenu()
        {
            Events.AfterOpenWorldMap += OnOpenWorldMap;
            Events.AfterOpenQuickMap += OnOpenQuickMap;
            Events.BeforeCloseMap += OnCloseMap;

            RemoveMapLayers();
        }

        internal static void AddMapLayer(MapUILayer layer)
        {
            mapLayers.Add(layer);
            layer.Build();
        }

        internal static void RemoveMapLayers()
        {
            foreach (MapUILayer layer in mapLayers)
            {
                layer.Destroy();
            }

            mapLayers.Clear();
        }

        private static void OnOpenWorldMap(GameMap obj)
        {
            Update();
        }

        private static void OnOpenQuickMap(GameMap gameMap, MapZone mapZone)
        {
            Update();
        }

        private static void OnCloseMap(GameMap obj)
        {
            Update();
        }

        public static void Update()
        {
            foreach (MapUILayer layer in mapLayers)
            {
                layer.Update();
            }
        }
    }
}
