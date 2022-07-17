using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    /// <summary>
    /// Base class for all objects in MapChanger.
    /// </summary>
    public abstract class MapObject : MonoBehaviour
    {
        private protected const int UI_LAYER = 5;
        private protected const string HUD = "HUD";

        public virtual void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            transform.localScale = Vector3.one;
            gameObject.layer = UI_LAYER;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Call this method to update the state of the object.
        /// </summary>
        public abstract void Set();
    }
}
