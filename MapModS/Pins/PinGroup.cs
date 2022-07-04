using GlobalEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapModS.Pins
{
    public abstract class PinGroup<T1, T2> : MonoBehaviour where T1 : Pin where T2 : AbstractPinDef
    {
        private const float OFFSETZ_BASE = -0.6f;
        private const float OFFSETZ_RANGE = 0.1f;

        public static PinGroup<T1, T2> Instance { get; private set; }

        public List<T1> Pins;

        public void Awake()
        {
            Instance = this;
            transform.localScale = Vector3.one;

            Pins = new();

            IEnumerable<T2> pinDefs = GetPinDefs();
            for (int i = 0; i < pinDefs.Count(); i++)
            {
                float offsetZ = OFFSETZ_BASE + (float)i / pinDefs.Count() * OFFSETZ_RANGE;
                T1 pin = MapModS.MakeMonoBehaviour<T1>(gameObject, pinDefs.ElementAt(i).Name);
                pin.Initialize(pinDefs.ElementAt(i), offsetZ);
                Pins.Add(pin);
            }

            gameObject.SetActive(false);

            Events.OnOpenWorldMap += OnOpenWorldMap;
            Events.OnOpenQuickMap += OnOpenQuickMap;
            Events.OnCloseMap += OnCloseMap;
        }

        public abstract IEnumerable<T2> GetPinDefs();

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
            foreach (T1 pin in Pins)
            {
                pin.Set();
            }
        }

        public void OnDestroy()
        {
            Events.OnOpenWorldMap -= OnOpenWorldMap;
            Events.OnOpenQuickMap -= OnOpenQuickMap;
            Events.OnCloseMap -= OnCloseMap;
        }
    }
}
