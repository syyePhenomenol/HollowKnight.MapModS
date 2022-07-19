using System.Collections.Generic;
using ConnectionMetadataInjector.Util;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using UnityEngine;
using VanillaMapMod.Settings;

namespace VanillaMapMod
{
    internal class VMMPin : Pin, ISelectable
    {
        private const float SMALL_SCALE = 0.31f;
        private const float MEDIUM_SCALE = 0.37f;
        private const float LARGE_SCALE = 0.44f;

        private static readonly Dictionary<PinSize, float> pinSizes = new()
        {
            { PinSize.Small, SMALL_SCALE },
            { PinSize.Medium, MEDIUM_SCALE },
            { PinSize.Large, LARGE_SCALE }
        };

        public override IMapPosition MapPosition => Location;

        internal MapLocationDef Location { get; private set; }

        internal PoolGroup PoolGroup { get; private set; }

        public bool Active { get; set; } = false;

        public bool Selected { get; private set; } = false;

        public bool Obtained { get; private set; } = false;

        public void Initialize(MapLocationDef mld)
        {
            Location = mld;

            PoolGroup = SubcategoryFinder.GetLocationPoolGroup(Location.Name);

            base.Initialize();
        }

        public override void SetSprite()
        {
            Sr.sprite = SpriteManager.GetSpriteFromPoolGroup(PoolGroup.FriendlyName());
        }

        public override void SetSpriteColor()
        {
        }

        public override void SetScale()
        {
            float scale = pinSizes[VanillaMapMod.GS.PinSize];
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        public override void Set()
        {
            Obtained = Tracker.HasClearedLocation(Location.Name);

            gameObject.SetActive(
                MapChanger.Settings.MapModEnabled
                && (MapChanger.Settings.CurrentMode().ModeKey is ("VanillaMapMod", "Normal") 
                    || MapChanger.Settings.CurrentMode().ModeKey is ("VanillaMapMod", "Full Map"))
                && Active && !Obtained
                && (States.WorldMapOpen || (States.QuickMapOpen && States.CurrentMapZone == Location.MapZone)));
                

            SetSprite();
            SetScale();
        }

        public bool CanUsePosition()
        {
            return true;
        }

        public (string, Vector2) GetKeyAndPosition()
        {
            return (Location.Name, transform.position);
        }

        public void Select()
        {
            Selected = true;
            Set();
            MapChangerMod.Instance.LogDebug($"Currently selected: {Location.Name}");
        }

        public void Deselect()
        {
            Selected = false;
            Set();
            MapChangerMod.Instance.LogDebug($"Currently deselected: {Location.Name}");
        }
    }
}
