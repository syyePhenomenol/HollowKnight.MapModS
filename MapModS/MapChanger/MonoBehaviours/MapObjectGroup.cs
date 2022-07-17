using GlobalEnums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    /// <summary>
    /// Shows a group of MapObjects when the map opens,
    /// and hides the group when the map closes.
    /// </summary>
    public class MapObjectGroup : MapObject
    {
        private const float OFFSETZ_BASE = -1.4f;
        private const float OFFSETZ_RANGE = 0.4f;

        public List<MapObject> MapObjects = new();

        public void StaggerZ()
        {
            IEnumerable<MapObject> MapObjectsSorted = MapObjects.OrderBy(mapObj => mapObj.transform.position.x).ThenBy(mapObj => mapObj.transform.position.y);

            for (int i = 0; i < MapObjectsSorted.Count(); i++)
            {
                Transform transform = MapObjectsSorted.ElementAt(i).transform;
                transform.localPosition = new(transform.localPosition.x, transform.localPosition.y, OFFSETZ_BASE + (float)i / MapObjects.Count() * OFFSETZ_RANGE);
            }
        }

        public void Awake()
        {
            base.Initialize();

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
