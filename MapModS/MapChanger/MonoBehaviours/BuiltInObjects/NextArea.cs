using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    internal class NextArea : MapObject, ISpriteRenderer, ITextMeshPro
    {
        private MiscObjectDef mod;
        private Vector4 tmpOrigColor;
        private Vector4 srOrigColor;

        public TextMeshPro Tmp { get; private set; }
        public SpriteRenderer Sr { get; private set; }
        private MapNextAreaDisplay Mnad => GetComponent<MapNextAreaDisplay>();

        internal void Initialize(MiscObjectDef mod)
        {
            this.mod = mod;

            Tmp = transform.FindChildInHierarchy("Area Name")?.GetComponent<TextMeshPro>();
            Sr = transform.FindChildInHierarchy("Map_Arrow")?.GetComponent<SpriteRenderer>();

            if (Tmp is null || Sr is null)
            {
                MapChangerMod.Instance.LogWarn($"Missing component references! {transform.name}");
                Destroy(this);
                return;
            }
  
            gameObject.SetActive(false);
            Tmp.gameObject.SetActive(true);
            Sr.gameObject.SetActive(true);

            tmpOrigColor = Tmp.color;
            srOrigColor = Sr.color;
        }

        public override void Set()
        {
            gameObject.SetActive(States.QuickMapOpen
                && !Settings.CurrentMode().DisableNextArea
                && (Mnad.visitedString is ""
                    || PlayerData.instance.GetBool(Mnad.visitedString)));

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
                Sr.color = color;
            }
            else
            {
                Tmp.color = tmpOrigColor;
                Sr.color = srOrigColor;
            }
        }

        public void SetSprite()
        {

        }

        public void SetSpriteColor()
        {

        }
    }
}
