using UnityEngine;

namespace MapModS
{
    public abstract class SpriteMapObject : MapObject, ISprite
    {
        public SpriteRenderer SR { get; set; }

        public override void Awake()
        {
            base.Awake();

            SR = gameObject.AddComponent<SpriteRenderer>();
            SR.sortingLayerName = HUD;
        }

        public override void Set()
        {
            base.Set();
            SetSprite();
            SetSpriteColor();
        }

        public abstract void SetSprite();

        public abstract void SetSpriteColor();
    }
}
