using GlobalEnums;
using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    /// <summary>
    /// Shows a group of MapObjects when the map opens,
    /// and hides the group when the map closes.
    /// </summary>
    public class MapObjectGroup : MapObject
    {
        public List<MapObject> MapObjects = new();

        public void Awake()
        {
            transform.localScale = Vector3.one;
            gameObject.SetActive(false);

            Events.BeforeOpenWorldMap += OnOpenWorldMap;
            Events.BeforeOpenQuickMap += OnOpenQuickMap;
            Events.BeforeCloseMap += OnCloseMap;
        }

        private void OnOpenWorldMap(GameMap gameMap)
        {
            gameObject.SetActive(true);
            Set();
        }

        private void OnOpenQuickMap(GameMap gameMap, MapZone mapZone)
        {
            gameObject.SetActive(true);
            Set();
        }

        private void OnCloseMap(GameMap gameMap)
        {
            gameObject.SetActive(false);
        }

        public override void Set()
        {
            foreach (MapObject mapObject in MapObjects)
            {
                mapObject.Set();
            }
        }

        public void OnDestroy()
        {
            MapChangerMod.Instance.LogDebug($"OnDestroy: {name}");

            Events.BeforeOpenWorldMap -= OnOpenWorldMap;
            Events.BeforeOpenQuickMap -= OnOpenQuickMap;
            Events.BeforeCloseMap -= OnCloseMap;
        }
    }
}
