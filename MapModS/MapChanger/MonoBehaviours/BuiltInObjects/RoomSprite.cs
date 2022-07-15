using System;
using System.Collections.Generic;
using GlobalEnums;
using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class RoomSprite : MapObject, ISpriteRenderer, IMapRoom, ISelectable
    {
        /// <returns>
        /// Whether or not to disable the normal setting behaviour afterwards.
        /// </returns>
        internal static event Func<RoomSprite, bool> OnSet;
        public string SceneName => rsd.SceneName;
        public MapZone MapZone => rsd.MapZone;
        public SpriteRenderer Sr => GetComponent<SpriteRenderer>();
        public Vector4 OrigColor { get; set; }

        private RoomSpriteDef rsd;
        private bool canSelect = false;
        public bool Selected { get; private set; } = false;

        public bool CanUsePosition()
        {
            return gameObject.activeSelf && canSelect;
        }

        public void Select()
        {
            MapChangerMod.Instance.LogDebug($"Currently selected: {transform.name}");
            Selected = true;
            Set();
        }

        public void Deselect()
        {
            MapChangerMod.Instance.LogDebug($"Currently deselected: {transform.name}");
            Selected = false;
            Set();
        }

        public (string, Vector2) GetKeyAndPosition()
        {
            return (SceneName, transform.position);
        }

        internal void Initialize(RoomSpriteDef rsd)
        {
            this.rsd = rsd;
            canSelect = Finder.IsScene(transform.name) || Finder.IsScene(transform.parent.name);
            OrigColor = Sr.color;

            if (!Finder.IsScene(SceneName))
            {
                MapChangerMod.Instance.LogDebug($"Not a scene: {SceneName}");
            }

            SetSprite();
            SetSpriteColor();
        }

        public override void Set()
        {
            try { if (OnSet.Invoke(this)) return; }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }

            gameObject.SetActive(Settings.CurrentMode().ForceFullMap
                || PlayerData.instance.GetVariable<List<string>>("scenesMapped").Contains(SceneName)
                || Finder.IsMinimalMapScene(transform.name));

            SetSprite();
            SetSpriteColor();
        }

        public void SetSprite()
        {

        }

        public void SetSpriteColor()
        {
            if (Settings.MapModEnabled)
            {
                if (Selected)
                {
                    Sr.color = Colors.GetColor(ColorSetting.Room_Selected);
                    return;
                }
                else if (Settings.CurrentMode().EnableCustomColors && Colors.TryGetCustomColor(rsd.ColorSetting, out Vector4 color))
                {
                    Sr.color = color;
                    return;
                }
            }
            Sr.color = OrigColor;
        }
    }
}
