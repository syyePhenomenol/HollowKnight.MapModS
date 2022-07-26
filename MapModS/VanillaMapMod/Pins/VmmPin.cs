using System;
using System.Collections.Generic;
using ConnectionMetadataInjector.Util;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using UnityEngine;
using VanillaMapMod.Settings;

namespace VanillaMapMod
{
    internal class VmmPin : Pin, ISelectable
    {
        private const float SMALL_SCALE = 0.31f;
        private const float MEDIUM_SCALE = 0.37f;
        private const float LARGE_SCALE = 0.44f;

        private const float SELECTED_MULTIPLIER = 1.5f;

        private static readonly Dictionary<PinSize, float> pinSizes = new()
        {
            { PinSize.Small, SMALL_SCALE },
            { PinSize.Medium, MEDIUM_SCALE },
            { PinSize.Large, LARGE_SCALE }
        };

        private bool selected = false;
        public bool Selected
        {
            get => selected;
            set
            {
                if (Selected != value)
                {
                    selected = value;
                    UpdatePinSize();
                }
            }
        }

        internal MapLocationPosition Mlp { get; private set; }

        internal void Initialize(MapLocationDef mld, VmmPinGroup parent)
        {
            base.Initialize();

            ActiveModifiers.AddRange
            (
                new Func<bool>[]
                {
                    CorrectMapOpen,
                    ActiveByCurrentMode,
                    LocationNotCleared
                }
            );

            Parent = parent;
            Mlp = new MapLocationPosition(mld.MapLocations);
            MapPosition = Mlp;
            Sprite = SpriteManager.GetSpriteFromPoolGroup(parent.PoolGroup.FriendlyName());
        }

        private void OnEnable()
        {
            UpdatePinSize();
        }

        internal void UpdatePinSize()
        {
            Size = pinSizes[VanillaMapMod.GS.PinSize];

            if (selected)
            {
                Size *= SELECTED_MULTIPLIER;
            }
        }

        public bool CanSelect()
        {
            return Sr.isVisible;
        }

        public (string, Vector2) GetKeyAndPosition()
        {
            return (name, transform.position);
        }

        private bool CorrectMapOpen()
        {
            return States.WorldMapOpen || (States.QuickMapOpen && States.CurrentMapZone == Mlp.MapZone);
        }

        private bool ActiveByCurrentMode()
        {
            return (MapChanger.Settings.CurrentMode() is NormalMode && Utils.HasMapSetting(Mlp.MapZone))
                    || MapChanger.Settings.CurrentMode() is FullMapMode;
        }

        private bool LocationNotCleared()
        {
            return !Tracker.HasClearedLocation(name);
        }
    }
}
