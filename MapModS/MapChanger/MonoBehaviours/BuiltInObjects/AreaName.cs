using System;
using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class AreaName : ColoredMapObject
    {
        public MiscObjectDef MiscObjectDef { get; private set; }

        private TextMeshPro tmp;
        public override Vector4 Color
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

        public override void UpdateColor()
        {
            if (!Settings.MapModEnabled)
            {
                ResetColor();
                return;
            }

            try { Settings.CurrentMode().OnAreaNameUpdateColor?.Invoke(this); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }
    }
}
