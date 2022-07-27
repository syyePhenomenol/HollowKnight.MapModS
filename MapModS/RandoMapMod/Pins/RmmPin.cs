﻿using System;
using System.Collections.Generic;
using GlobalEnums;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using RandoMapMod.Settings;
using UnityEngine;

namespace RandoMapMod.Pins
{
    internal abstract class RmmPin: BorderedPin, ISelectable
    {
        private const float SMALL_SCALE = 0.31f;
        private const float MEDIUM_SCALE = 0.37f;
        private const float LARGE_SCALE = 0.44f;

        private protected const float UNREACHABLE_SIZE_MULTIPLIER = 0.7f;
        private protected const float UNREACHABLE_COLOR_MULTIPLIER = 0.5f;

        private protected const float SELECTED_MULTIPLIER = 1.5f;

        private protected static readonly Dictionary<PinSize, float> pinSizes = new()
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
                    UpdatePinColor();
                    UpdateBorderColor();
                }
            }
        }

        internal string LocationPoolGroup { get; private protected set; }
        internal abstract HashSet<string> ItemPoolGroups { get; }
        internal MapZone MapZone { get; private set; } = MapZone.NONE;

        public void Initialize(MapLocation[] mapLocations)
        {
            MapLocationPosition mlp = new(mapLocations);
            MapPosition = mlp;
            MapZone = mlp.MapZone;

            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();

            ActiveModifiers.AddRange
            (
                new Func<bool>[]
                {
                    CorrectMapOpen,
                    ActiveByCurrentMode,
                    ActiveByPoolSetting,
                    LocationNotCleared
                }
            );

            BorderSprite = SpriteManager.GetSprite("pinBorder");
            BorderPlacement = BorderPlacement.InFront;
        }

        public bool CanSelect()
        {
            return Sr.isVisible;
        }

        public (string, Vector2) GetKeyAndPosition()
        {
            return (name, transform.position);
        }

        private protected virtual void OnEnable()
        {
            UpdatePinSprite();
            UpdatePinSize();
            UpdatePinColor();
            UpdateBorderColor();
        }

        protected private abstract void UpdatePinSprite();

        protected private abstract void UpdatePinSize();

        protected private abstract void UpdatePinColor();

        protected private abstract void UpdateBorderColor();

        protected private bool CorrectMapOpen()
        {
            return States.WorldMapOpen || (States.QuickMapOpen && States.CurrentMapZone == MapZone);
        }

        protected private bool ActiveByCurrentMode()
        {
            return MapChanger.Settings.CurrentMode().Mod is "RandoMapMod";
        }

        protected private abstract bool ActiveByPoolSetting();

        protected private abstract bool LocationNotCleared();

        internal abstract void GetLookupText();
    }
}