using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    internal class AreaName : MapObject, ITextMeshPro
    {
        private MiscObjectDef mod;
        private Vector4 OrigColor;

        public TextMeshPro Tmp => GetComponent<TextMeshPro>();

        internal void Initialize(MiscObjectDef mod)
        {
            this.mod = mod;

            if (Tmp is null)
            {
                MapChangerMod.Instance.LogWarn($"Missing component references! {transform.name}");
                Destroy(this);
                return;
            }
  
            gameObject.SetActive(false);

            OrigColor = Tmp.color;
        }

        public override void Set()
        {
            gameObject.SetActive(!Settings.CurrentMode().DisableAreaNames);

            SetTextColor();
        }

        public void SetText()
        {

        }

        public void SetTextColor()
        {
            if (Settings.MapModEnabled && Settings.CurrentMode().EnableCustomColors && Colors.TryGetCustomColor(mod.ColorSetting, out Vector4 color))
            {
                Tmp.color = color;
            }
            else
            {
                Tmp.color = OrigColor;
            }
        }
    }
}
