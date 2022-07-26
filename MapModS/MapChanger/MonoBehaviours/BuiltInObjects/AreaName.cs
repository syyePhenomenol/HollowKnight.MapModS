using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class AreaName : MapObject
    {
        public MiscObjectDef MiscObjectDef { get; private set; }

        private TextMeshPro tmp;
        public Vector4 OrigColor { get; private set; }
        public Vector4 Color
        {
            get => tmp.color;
            set
            {
                tmp.color = value;
            }
        }

        internal void Initialize(MiscObjectDef miscObjectDef)
        {
            ActiveModifiers.Add(AreaNamesEnabled);

            MiscObjectDef = miscObjectDef;

            tmp = GetComponent<TextMeshPro>();

            if (tmp is null)
            {
                MapChangerMod.Instance.LogWarn($"Missing component references! {transform.name}");
                Destroy(this);
                return;
            }

            OrigColor = tmp.color;
        }

        private bool AreaNamesEnabled()
        {
            return !Settings.CurrentMode().DisableAreaNames;
        }
    }
}
