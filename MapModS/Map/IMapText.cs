using TMPro;
using UnityEngine;

namespace MapModS.Map
{
    internal interface IMapText
    {
        SetTextMeshProGameText STMPGT { get; }
        TextMeshPro TMP { get; }
    }
}
