using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    /// <summary>
    /// Base class for all map objects in MapChanger. If your derived object is a root object,
    /// you probably want to initialize with DontDestroyOnLoad(), and to manually destroy it
    /// at some point.
    /// </summary>
    public abstract class MapObject : MonoBehaviour
    {
        /// <summary>
        /// A list of conditions to determine whether or not the GameObject should be set active or not.
        /// </summary>
        public readonly List<Func<bool>> ActiveModifiers = new();

        private protected const int UI_LAYER = 5;
        private protected const string HUD = "HUD";

        private MapObject parent;
        /// <summary>
        /// When UpdateActive of the parent MapObject is called, it will call this MapObject's
        /// UpdateActive.
        /// </summary>
        public MapObject Parent
        {
            get => parent;
            set
            {
                if (parent != value && this != value)
                {
                    parent = value;
                    gameObject.transform.parent = parent.transform;
                }
            }
        }

        /// <summary>
        /// Returns a list of children MapObjects.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<MapObject> GetChildren()
        {
            List<MapObject> mapObjects = new();

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out MapObject mapObject))
                {
                    mapObjects.Add(mapObject);
                }
            }

            return mapObjects.AsReadOnly();
        }

        public virtual void Initialize()
        {
            gameObject.layer = UI_LAYER;
            gameObject.SetActive(false);

            transform.localScale = Vector3.one;

            ActiveModifiers.Add(() => { return Settings.MapModEnabled; });
        } 

        /// <summary>
        /// Sets the gameObject as active/inactive based on the conjunction of a list of conditions.
        /// Also calls UpdateActive on its children.
        /// </summary>
        protected internal void UpdateActive()
        {
            bool value = true;

            foreach (Func<bool> activeModifier in ActiveModifiers)
            {
                try
                {
                    value &= activeModifier.Invoke();
                }
                catch (Exception e)
                {
                    MapChangerMod.Instance.LogError(e);
                }
            }

            MapChangerMod.Instance.LogDebug($"UpdateActive: {name}, {value}");

            gameObject.SetActive(value);

            foreach (MapObject mapObject in GetChildren())
            {
                mapObject.UpdateActive();
            }
        }
    }
}
