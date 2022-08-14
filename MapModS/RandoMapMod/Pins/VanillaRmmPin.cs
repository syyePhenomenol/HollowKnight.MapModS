﻿using System;
using System.Collections.Generic;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using MapChanger;
using MapChanger.Defs;
using RandomizerCore;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace RandoMapMod.Pins
{
    internal sealed class VanillaRmmPin : RmmPin
    {
        private GeneralizedPlacement placement;

        private static readonly Vector4 vanillaColor = new(UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER, 1f);

        internal override HashSet<string> ItemPoolGroups => new() { LocationPoolGroup };

        private ISprite locationSprite;

        internal void Initialize(GeneralizedPlacement placement)
        {
            Initialize();

            this.placement = placement;

            SceneName = RandomizerMod.RandomizerData.Data.GetLocationDef(name)?.SceneName ?? ItemChanger.Finder.GetLocation(name)?.sceneName;

            LocationPoolGroup = SubcategoryFinder.GetLocationPoolGroup(placement.Location.Name).FriendlyName();
            locationSprite = new PinLocationSprite(LocationPoolGroup);

            if (InteropProperties.GetDefaultMapLocations(name) is (string, float, float)[] mapLocations)
            {
                MapPosition mlp = new(mapLocations);
                MapPosition = mlp;
                MapZone = mlp.MapZone;
            }
            else
            {
                RmmPinManager.MiscPins.Add(this);
            }
        }

        private protected override bool ActiveBySettings()
        {
            Settings.PoolState poolState = RandoMapMod.LS.GetPoolGroupSetting(LocationPoolGroup);

            return poolState == Settings.PoolState.On || (poolState == Settings.PoolState.Mixed && RandoMapMod.LS.VanillaOn);
        }

        protected private override bool LocationNotCleared()
        {
            return !Tracker.HasClearedLocation(name);
        }

        private protected override void UpdatePinSprite()
        {
            Sprite = locationSprite.Value;
        }

        private protected override void UpdatePinSize()
        {
            Size = pinSizes[RandoMapMod.GS.PinSize];

            if (Selected)
            {
                Size *= SELECTED_MULTIPLIER;
            }
            else
            {
                Size *= UNREACHABLE_SIZE_MULTIPLIER;
            }
        }

        private protected override void UpdatePinColor()
        {
            Color = vanillaColor;
        }

        private protected override void UpdateBorderColor()
        {
            BorderColor = vanillaColor;
        }

        internal override string GetSelectionText()
        {
            string text = base.GetSelectionText();

            text += $"\n\n{L.Localize("Status")}:";

            if (IsPersistent())
            {
                text += $" {L.Localize("Not randomized, persistent")}";
            }
            else
            {
                text += $" {L.Localize("Not randomized, unchecked")}";
            }

            text += $"\n\n{L.Localize("Logic")}: {Logic ?? "not found"}";

            return text; 
        }

        private bool IsPersistent()
        {
            return LocationPoolGroup == PoolGroup.LifebloodCocoons.FriendlyName() 
                || LocationPoolGroup == PoolGroup.SoulTotems.FriendlyName()
                || LocationPoolGroup == PoolGroup.LoreTablets.FriendlyName();
        }
    }
}
