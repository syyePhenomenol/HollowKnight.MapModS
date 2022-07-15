using TMPro;

namespace MapChanger.MonoBehaviours
{
    internal interface ITextMeshPro
    {
        TextMeshPro Tmp { get; }
        void SetText();
        void SetTextColor();
    }
}
