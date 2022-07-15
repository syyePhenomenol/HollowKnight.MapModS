using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public enum BorderPlacement
    {
        Behind,
        InFront
    }

    internal interface IBorder
    {
        SpriteRenderer BorderSR { get; set; }
        BorderPlacement BorderPlacement { get; set; }
        void SetBorderPosition();
        void SetBorderSprite();
        void SetBorderColor();
    }
}
