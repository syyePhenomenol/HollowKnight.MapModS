using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.Objects
{
    public abstract class MapObject : MonoBehaviour
    {
        private protected const int UI_LAYER = 5;
        private protected const string HUD = "HUD";

        public abstract IMapPosition MapPosition { get; }
        public Transform GameMap => transform.parent.transform.parent;

        public virtual void Initialize()
        {
            gameObject.layer = UI_LAYER;
            SetPosition();
            SetScale();
        }

        /// <summary>
        /// Call this method to update the state of the object.
        /// </summary>
        public abstract void Set();

        public virtual void SetPosition()
        {
            // TODO: Change to Finder dictionary lookup
            if (TryGetRoomPos(MapPosition.MappedScene, out Vector2 vec))
            {
                transform.SetPositionX(vec.x);
                transform.SetPositionY(vec.y);
            }
            else
            {
                MapChangerMod.Instance.LogWarn($"{MapPosition.MappedScene} not found on the map!");
            }
        }

        // TODO: change to Finder private function
        private bool TryGetRoomPos(string roomName, out Vector2 pos)
        {
            foreach (Transform areaObj in GameMap)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (roomObj.gameObject.name == roomName)
                    {
                        Vector3 roomVec = roomObj.transform.localPosition;
                        pos = areaObj.transform.localPosition + roomVec;
                        return true;
                    }
                }
            }

            pos = Vector2.zero;
            return false;
        }

        public abstract void SetScale();
    }
}
