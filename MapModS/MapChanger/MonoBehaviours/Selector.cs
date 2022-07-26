using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    /// <summary>
    /// A MapObject with a reticle for selecting MapObjects on the world map.
    /// </summary>
    public abstract class Selector : MapObject, IPeriodicUpdater
    {
        public const string NONE_SELECTED = "None selected";

        protected const float MAP_FRONT_Z = -30f;
        protected const float DEFAULT_SIZE = 0.3f;
        protected const float DEFAULT_SELECTION_RADIUS = 0.7f;
        protected static readonly Vector4 DEFAULT_COLOR = new(1f, 1f, 1f, 0.6f);
        protected const string SELECTOR_SPRITE = "selector";

        public Dictionary<string, List<ISelectable>> Objects { get; } = new();
        public virtual Vector2 TargetPosition { get; } = Vector2.zero;
        public virtual float UpdateWaitSeconds { get; } = 0.1f;
        public virtual float SelectionRadius { get; } = DEFAULT_SELECTION_RADIUS;

        protected SpriteRenderer Sr { get; private set; }

        private string selectedObjectKey = NONE_SELECTED;
        public string SelectedObjectKey
        {
            get => selectedObjectKey;
            private set
            {
                if (selectedObjectKey != value)
                {
                    if (selectedObjectKey is not NONE_SELECTED)
                    {
                        Deselect(selectedObjectKey);
                    }

                    if (value is not NONE_SELECTED)
                    {
                        Select(value);
                    }

                    selectedObjectKey = value;
                }
            }
        }

        private void Select(string objectKey)
        {
            if (Objects.ContainsKey(objectKey))
            {
                foreach (ISelectable selectable in Objects[objectKey])
                {
                    Select(selectable);
                }
            }
        }

        private void Deselect(string objectKey)
        {
            if (Objects.ContainsKey(objectKey))
            {
                foreach (ISelectable selectable in Objects[objectKey])
                {
                    Deselect(selectable);
                }
            }
        }

        // Coroutines stop when the GameObject is set inactive!
        public IEnumerator PeriodicUpdate()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

                double minDistance = SelectionRadius;
                string newKey = NONE_SELECTED;

                foreach (List<ISelectable> selectables in Objects.Values)
                {
                    foreach (ISelectable selectable in selectables)
                    {
                        if (!selectable.CanSelect()) continue;

                        (string key, Vector2 position) = selectable.GetKeyAndPosition();

                        double distanceX = Math.Abs(position.x - TargetPosition.x);
                        if (distanceX > minDistance) continue;

                        double distanceY = Math.Abs(position.y - TargetPosition.y);
                        if (distanceY > minDistance) continue;

                        double euclidDistance = Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2);
                        if (euclidDistance < minDistance)
                        {
                            newKey = key;
                            minDistance = euclidDistance;
                        }
                    }
                }

                SelectedObjectKey = newKey;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            gameObject.SetActive(false);
            DontDestroyOnLoad(this);

            ActiveModifiers.Add(WorldMapOpen);

            Sr = gameObject.AddComponent<SpriteRenderer>();
            Sr.sprite = SpriteManager.GetSprite(SELECTOR_SPRITE);
            Sr.color = DEFAULT_COLOR;
            Sr.sortingLayerName = HUD;

            transform.localScale = Vector3.one * DEFAULT_SIZE;
            transform.localPosition = new Vector3(0, 0, MAP_FRONT_Z);

            MapObjectUpdater.Add(this);
        }

        private bool WorldMapOpen()
        {
            return States.WorldMapOpen;
        }

        public void OnEnable()
        {
            StartCoroutine(PeriodicUpdate());
        }

        public void OnDisable()
        {
            SelectedObjectKey = NONE_SELECTED;
        }

        protected abstract void Select(ISelectable selectable);

        protected abstract void Deselect(ISelectable selectable);
    }
}
