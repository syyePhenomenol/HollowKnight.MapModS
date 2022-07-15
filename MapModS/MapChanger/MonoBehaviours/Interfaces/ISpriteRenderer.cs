using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public interface ISpriteRenderer
    {
        SpriteRenderer Sr { get; }

        void SetSprite();
        void SetSpriteColor();
    }
}
