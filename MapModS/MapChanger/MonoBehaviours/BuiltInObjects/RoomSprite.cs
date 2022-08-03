using System;
using System.Collections.Generic;
using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class RoomSprite : ColoredMapObject, ISelectable
    {
        public RoomSpriteDef Rsd { get; private set; }

        private SpriteRenderer sr;
        public override Vector4 Color
        {
            get => sr.color;
            set
            {
                sr.color = value;
            }
        }

        private bool selected = false;
        public bool Selected
        {
            get => selected;
            set
            {
                if (Selected != value)
                {
                    selected = value;
                    UpdateColor();
                }
            }
        }

        internal void Initialize(RoomSpriteDef rsd)
        {
            Rsd = rsd;

            ActiveModifiers.Add(IsActive);
            
            sr = GetComponentInChildren<SpriteRenderer>();
            OrigColor = sr.color;

            if (!Finder.IsScene(Rsd.SceneName))
            {
                MapChangerMod.Instance.LogDebug($"Not a scene: {Rsd.SceneName}");
            }

            MapObjectUpdater.Add(this);
        }

        private bool IsActive()
        {
            if (Settings.MapModEnabled && Settings.CurrentMode().RoomSpriteActiveOverride is not null)
            {
                try { return Settings.CurrentMode().RoomSpriteActiveOverride.Invoke(this); }
                catch (Exception e) { MapChangerMod.Instance.LogError(e); }
            }

            return Settings.CurrentMode().FullMap
                || PlayerData.instance.GetVariable<List<string>>("scenesMapped").Contains(Rsd.SceneName)
                || Finder.IsMinimalMapScene(transform.name);
        }

        public override void UpdateColor()
        {
            if (!Settings.MapModEnabled)
            {
                ResetColor();
                return;
            }

            try { Settings.CurrentMode().OnRoomUpdateColor?.Invoke(this); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }

        public bool CanSelect()
        {
            return gameObject.activeSelf;
        }

        public (string, Vector2) GetKeyAndPosition()
        {
            return (Rsd.SceneName, transform.localPosition);
        }
    }
}
