using UnityEngine;

namespace MapChanger.Objects
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
