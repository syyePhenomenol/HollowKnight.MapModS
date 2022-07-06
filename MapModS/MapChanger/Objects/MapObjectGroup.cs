using GlobalEnums;
using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.Objects
{
    public class MapObjectGroup : MonoBehaviour
    {
        public List<MapObject> MapObjects = new();

        public void Awake()
        {
            transform.localScale = Vector3.one;
            gameObject.SetActive(false);

            Events.OnOpenWorldMap += OnOpenWorldMap;
            Events.OnOpenQuickMap += OnOpenQuickMap;
            Events.OnCloseMap += OnCloseMap;
        }

        private void OnOpenWorldMap(GameMap gameMap)
        {
            gameObject.SetActive(true);
            SetPins();
        }

        private void OnOpenQuickMap(GameMap gameMap, MapZone mapZone)
        {
            gameObject.SetActive(true);
            SetPins();
        }

        private void OnCloseMap(GameMap gameMap)
        {
            gameObject.SetActive(false);
        }

        public void SetPins()
        {
            foreach (MapObject mapObject in MapObjects)
            {
                mapObject.Set();
            }
        }
    }
}
