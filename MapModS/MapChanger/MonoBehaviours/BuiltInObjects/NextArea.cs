using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class NextArea : MapObject
    {
        public MiscObjectDef MiscObjectDef { get; private set; }

        private TextMeshPro tmp;
        private SpriteRenderer sr;
        public Vector4 OrigColor { get; private set; }
        public Vector4 Color
        {
            get => tmp.color;
            set
            {
                tmp.color = value;
                sr.color = value;
            }
        }

        private MapNextAreaDisplay Mnad => GetComponent<MapNextAreaDisplay>();

        internal void Initialize(MiscObjectDef miscObjectDef)
        {
            MiscObjectDef = miscObjectDef;

            ActiveModifiers.Add(IsActive);

            tmp = transform.FindChildInHierarchy("Area Name")?.GetComponent<TextMeshPro>();
            sr = transform.FindChildInHierarchy("Map_Arrow")?.GetComponent<SpriteRenderer>();

            if (tmp is null || sr is null)
            {
                MapChangerMod.Instance.LogWarn($"Missing component references! {transform.name}");
                Destroy(this);
                return;
            }

            OrigColor = tmp.color;
        }

        private bool IsActive()
        {
            return States.QuickMapOpen
                && !Settings.CurrentMode().DisableNextArea
                && (Mnad.visitedString is "" || PlayerData.instance.GetBool(Mnad.visitedString));
        }
    }
}
