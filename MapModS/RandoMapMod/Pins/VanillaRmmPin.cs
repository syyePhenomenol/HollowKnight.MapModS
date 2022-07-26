using System;
using System.Collections.Generic;
using ConnectionMetadataInjector.Util;
using MapChanger;
using MapChanger.Defs;
using RandomizerCore;
using UnityEngine;

namespace RandoMapMod.Pins
{
    internal sealed class VanillaRmmPin : RmmPin
    {
        private static readonly Vector4 vanillaColor = new(UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER, 1f);

        internal override HashSet<string> ItemPoolGroups => new() { LocationPoolGroup };

        internal void Initialize(GeneralizedPlacement placement)
        {
            LocationPoolGroup = SubcategoryFinder.GetLocationPoolGroup(placement.Location.Name).FriendlyName();

            if (Finder.TryGetLocation(placement.Location.Name, out MapLocationDef mld))
            {
                Initialize(mld.MapLocations);
                return;
            }

            RandoMapMod.Instance.LogWarn($"No MapLocationDef found for vanilla placement {name}");
            Initialize();
        }

        private protected override bool ActiveByPoolSetting()
        {
            return RandoMapMod.LS.GetPoolGroupSetting(LocationPoolGroup) == Settings.PoolState.On || RandoMapMod.LS.VanillaOn;
        }

        private protected override bool LocationNotCleared()
        {
            return !Tracker.HasClearedLocation(name);
        }

        private protected override void UpdatePinSprite()
        {
            Sprite = SpriteManager.GetSpriteFromPoolGroup(LocationPoolGroup);
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

        internal override void GetLookupText()
        {
            throw new NotImplementedException();
        }
    }
}
