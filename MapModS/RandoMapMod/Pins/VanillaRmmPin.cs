using System;
using System.Collections.Generic;
using ConnectionMetadataInjector.Util;
using MapChanger;
using RandomizerCore;
using UnityEngine;

namespace RandoMapMod.Pins
{
    internal sealed class VanillaRmmPin : RmmPin
    {
        private GeneralizedPlacement placement;

        private static readonly Vector4 vanillaColor = new(UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER, 1f);

        internal override HashSet<string> ItemPoolGroups => new() { LocationPoolGroup };

        internal void Initialize(GeneralizedPlacement placement)
        {
            this.placement = placement;

            SceneName = RandomizerMod.RandomizerData.Data.GetLocationDef(name)?.SceneName ?? ItemChanger.Finder.GetLocation(name)?.sceneName;

            LocationPoolGroup = SubcategoryFinder.GetLocationPoolGroup(placement.Location.Name).FriendlyName();

            Initialize(InteropProperties.GetDefaultMapLocations(name));
        }

        private protected override bool ActiveByPoolSetting()
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
            if (RandoMapMod.LS.SpoilerOn)
            {
                Sprite = PinSprites.GetItemSprite(LocationPoolGroup).Value;
            }
            else
            {
                Sprite = PinSprites.GetLocationSprite(LocationPoolGroup).Value;
            }
            
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
