using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public interface ISelectable
    {
        bool CanUsePosition();
        (string, Vector2) GetKeyAndPosition();
        void Select();
        void Deselect();

        bool Selected { get; }
    }
}
