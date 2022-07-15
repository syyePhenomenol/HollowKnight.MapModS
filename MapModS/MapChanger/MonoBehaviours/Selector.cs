using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class Selector : MapObject, IPeriodicUpdater, ISpriteRenderer
    {
        public event Action<string> OnSelectionChanged;

        public const string NONE_SELECTED = "None selected";
        public Dictionary<string, List<ISelectable>> Objects { get; set; } = new();
        public Vector2 TargetPosition { get; set; } = Vector2.zero;
        public float UpdateWaitSeconds { get; set; } = 0.1f;

        public SpriteRenderer Sr => GetComponent<SpriteRenderer>();

        private string selectedObjectKey = NONE_SELECTED;

        public IEnumerator PeriodicUpdate()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

                if (!gameObject.activeSelf) continue;

                if (TryGetKeyOfObjectClosestToTarget(out string newKey))
                {
                    if (Objects.ContainsKey(selectedObjectKey))
                    {
                        foreach (ISelectable selectable in Objects[selectedObjectKey])
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
                    selectedObjectKey = newKey;
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

                return newKey != selectedObjectKey;

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
            gameObject.layer = 5;

            gameObject.AddComponent<SpriteRenderer>();
            Sr.sprite = SpriteManager.GetSprite("selector");
            Sr.sortingLayerName = HUD;

            transform.localScale = Vector3.one * 0.3f;
            transform.localPosition = new Vector3(0, 0, -23f);

            gameObject.SetActive(false);

            Events.AfterOpenWorldMap += OnOpenWorldMap;
            Events.BeforeCloseMap += OnCloseMap;
        }

        private void OnOpenWorldMap(GameMap gameMap)
        {
            StopAllCoroutines();
            Set();
            StartCoroutine(PeriodicUpdate());
        }

        private void OnCloseMap(GameMap gameMap)
        {
            StopAllCoroutines();
            Set();
        }

        public void OnDestroy()
        {
            Events.AfterOpenWorldMap -= OnOpenWorldMap;
            Events.BeforeCloseMap -= OnCloseMap;
        }

        public override void Set()
        {
            try { OnSelectionChanged?.Invoke(selectedObjectKey); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }

            if (Settings.MapModEnabled && States.WorldMapOpen)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);

                if (selectedObjectKey != NONE_SELECTED)
                {
                    foreach (ISelectable selectable in Objects[selectedObjectKey])
                    {
                        selectable.Deselect();
                    }

                    selectedObjectKey = NONE_SELECTED;
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
    }
}
