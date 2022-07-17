using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class Selector : MapObject, IPeriodicUpdater, ISpriteRenderer, IToggleable
    {
        internal event Action<string> OnSet;

        public const string NONE_SELECTED = "None selected";
        
        private const float DEFAULT_SIZE = 0.3f;
        private const float MAP_FRONT_Z = -23f;
        private const string SELECTOR = "selector";

        public Dictionary<string, List<ISelectable>> Objects { get; set; } = new();
        public Vector2 TargetPosition { get; set; } = Vector2.zero;
        public float UpdateWaitSeconds { get; set; } = 0.1f;

        public SpriteRenderer Sr => GetComponent<SpriteRenderer>();

        internal string SelectedObjectKey { get; private set; } = NONE_SELECTED;

        private bool isActive = true;

        // Coroutines stop when the GameObject is set inactive!
        public IEnumerator PeriodicUpdate()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

                if (!isActive) continue;

                if (TryGetKeyOfObjectClosestToTarget(out string newKey))
                {
                    if (Objects.ContainsKey(SelectedObjectKey))
                    {
                        foreach (ISelectable selectable in Objects[SelectedObjectKey])
                        {
                            selectable.Deselect();
                        }
                    }
                    if (Objects.ContainsKey(newKey))
                    {
                        foreach (ISelectable selectable in Objects[newKey])
                        {
                            selectable.Select();
                        }
                    }
                    SelectedObjectKey = newKey;
                    Set();
                }
            }

            bool TryGetKeyOfObjectClosestToTarget(out string newKey)
            {
                newKey = NONE_SELECTED;
                double minDistance = double.PositiveInfinity;

                foreach (List<ISelectable> selectables in Objects.Values)
                {
                    foreach (ISelectable selectable in selectables)
                    {
                        if (!selectable.CanUsePosition()) continue;

                        (string key, Vector2 position) = selectable.GetKeyAndPosition();

                        double distanceX = HorizontalDistanceToTarget(position);
                        if (distanceX > minDistance) continue;

                        double distanceY = VerticalDistanceToTarget(position);
                        if (distanceY > minDistance) continue;

                        double euclidDistance = EuclideanDistanceToTarget(distanceX, distanceY);
                        if (euclidDistance < minDistance)
                        {
                            newKey = key;
                            minDistance = euclidDistance;
                        }
                    }
                }

                return newKey != SelectedObjectKey;

                double HorizontalDistanceToTarget(Vector2 position)
                {
                    return Math.Abs(position.x - TargetPosition.x);
                }

                double VerticalDistanceToTarget(Vector2 position)
                {
                    return Math.Abs(position.y - TargetPosition.y);
                }

                double EuclideanDistanceToTarget(double distanceX, double distanceY)
                {
                    return Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2);
                }
            }
        }

        public void Awake()
        {
            base.Initialize();

            gameObject.AddComponent<SpriteRenderer>();
            Sr.sprite = SpriteManager.GetSprite(SELECTOR);
            Sr.sortingLayerName = HUD;

            transform.localScale = Vector3.one * DEFAULT_SIZE;
            transform.localPosition = new Vector3(0, 0, MAP_FRONT_Z);

            Events.AfterOpenWorldMap += OnOpenWorldMap;
            Events.BeforeCloseMap += OnCloseMap;
            Events.BeforeQuitToMenu += OnQuitToMenu;
        }

        private void OnOpenWorldMap(GameMap gameMap)
        {
            Set();
        }

        private void OnCloseMap(GameMap gameMap)
        {
            Set();
        }

        /// <summary>
        /// As this GameObject doesn't have a parent, we need to manually destroy it
        /// </summary>
        private void OnQuitToMenu()
        {
            Events.AfterOpenWorldMap -= OnOpenWorldMap;
            Events.BeforeCloseMap -= OnCloseMap;
            Events.BeforeQuitToMenu -= OnQuitToMenu;
            gameObject.DestroyAll();
        }

        public override void Set()
        {
            try { OnSet?.Invoke(SelectedObjectKey); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }

            if (Settings.MapModEnabled && States.WorldMapOpen && isActive)
            {
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    StartCoroutine(PeriodicUpdate());
                }
            }
            else
            {
                gameObject.SetActive(false);

                if (SelectedObjectKey != NONE_SELECTED)
                {
                    foreach (ISelectable selectable in Objects[SelectedObjectKey])
                    {
                        selectable.Deselect();
                    }

                    SelectedObjectKey = NONE_SELECTED;
                }
            }

            SetSpriteColor();
        }

        public void SetSprite()
        {
            
        }

        public void SetSpriteColor()
        {
            Sr.color = new Vector4(1f, 1f, 1f, 0.5f);

            //if (selectedObjectKey is not NONE_SELECTED)
            //{
            //    Sr.color = Colors.GetColor(ColorSetting.Room_Selected);
            //}
            //else
            //{
            //    Sr.color = Colors.GetColor(ColorSetting.Room_Normal);
            //}
        }

        public void SetActive(bool value)
        {
            isActive = value;
            Set();
        }

        public void Toggle()
        {
            isActive = !isActive;
            Set();
        }
    }
}
