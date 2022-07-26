using System.Collections.Generic;

namespace MapChanger.UI
{
    internal class MapUI : IMainHooks
    {
        internal static MapUI Instance { get; private set; }

        private static readonly List<UILayer> layers = new();

        public void OnEnterGame()
        {
            Instance = this;
            //    Events.AfterOpenWorldMap += OnOpenWorldMap;
            //    Events.AfterOpenQuickMap += OnOpenQuickMap;
            //    Events.BeforeCloseMap += OnCloseMap;
        }

        public void OnQuitToMenu()
        {
        //    Events.AfterOpenWorldMap -= OnOpenWorldMap;
        //    Events.AfterOpenQuickMap -= OnOpenQuickMap;
        //    Events.BeforeCloseMap -= OnCloseMap;

            RemoveLayers();
        }

        internal static void AddLayer(UILayer layer)
        {
            layers.Add(layer);
            layer.Build();
        }

        internal static void RemoveLayers()
        {
            foreach (UILayer layer in layers)
            {
                layer.Destroy();
            }

            layers.Clear();
        }

        //private static void OnOpenWorldMap(GameMap obj)
        //{
        //    Set();
        //}

        //private static void OnOpenQuickMap(GameMap gameMap, MapZone mapZone)
        //{
        //    Set();
        //}

        //private static void OnCloseMap(GameMap obj)
        //{
        //    Set();
        //}

        public static void Set()
        {
            foreach (UILayer layer in layers)
            {
                layer.Set();
            }
        }
    }
}
