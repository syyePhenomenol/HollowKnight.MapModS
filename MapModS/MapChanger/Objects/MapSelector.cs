using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.Objects
{
    public class MapSelector : MonoBehaviour, IPeriodicUpdater
    {
        public const string NONE_SELECTED = "None selected";
        public Dictionary<string, ISelectable> Objects { get; init; }

        public virtual Vector2 TargetPosition
        {
            get
            {
                return Vector2.zero;
            }
        }

        public virtual float UpdateWaitSeconds { get; set; } = 0.1f;

        private string selectedObjectName;

        public IEnumerator PeriodicUpdate()
        {
            yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

        }

        public virtual string GetObjectClosestToTarget()
        {
            return NONE_SELECTED;
        }

        public void Awake()
        {
            selectedObjectName = NONE_SELECTED;

            Events.OnOpenWorldMap += OnOpenWorldMap;
            Events.OnCloseMap += OnCloseMap;
        }

        public void OnOpenWorldMap(GameMap gameMap)
        {
            StopAllCoroutines();
            StartCoroutine(PeriodicUpdate());
        }

        public void OnCloseMap(GameMap gameMap)
        {
            StopAllCoroutines();

            if (selectedObjectName != NONE_SELECTED)
            {
                Objects[selectedObjectName].Deselect();
            }
            else
            {
                selectedObjectName = NONE_SELECTED;
            }
        }

        public void OnDestroy()
        {
            Events.OnOpenWorldMap -= OnOpenWorldMap;
            Events.OnCloseMap -= OnCloseMap;
        }
    }
}
