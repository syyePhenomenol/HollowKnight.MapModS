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
    public class MapObject : MonoBehaviour
    {
        /// <summary>
        /// A list of conditions to determine whether or not the GameObject should be set active or inactive.
        /// </summary>
        public readonly List<Func<bool>> ActiveModifiers = new();

        private protected const int UI_LAYER = 5;
        private protected const string HUD = "HUD";

        private readonly List<MapObject> children = new();
        public ReadOnlyCollection<MapObject> Children => children.AsReadOnly();

        public void AddChild(MapObject child)
        {
            if (children.Contains(child)) return;

            children.Add(child);
            child.transform.parent = transform;
        }

        public virtual void Initialize()
        {
            gameObject.layer = UI_LAYER;

            ActiveModifiers.Add(() => { return Settings.MapModEnabled; });
        } 

        public void MainUpdate()
        {
            BeforeMainUpdate();

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

            //MapChangerMod.Instance.LogDebug($"UpdateActive: {name}, {value}");

            gameObject.SetActive(value);

            AfterMainUpdate();

            foreach (MapObject mapObject in children)
            {
                mapObject.MainUpdate();
            }
        }

        public virtual void BeforeMainUpdate() { }

        public virtual void AfterMainUpdate() { }
    }
}
