using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using ItemChanger.Tags;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using RandoMapMod.Defs;
using UnityEngine;
using RM = RandomizerMod.RandomizerMod;

namespace RandoMapMod.Pins
{
    internal sealed class RandomizedRmmPin : RmmPin, IPeriodicUpdater
    {
        private AbstractPlacement placement;
        private RandoPlacementState placementState;

        private IEnumerable<AbstractItem> remainingItems;
        private int itemIndex = 0;

        private Dictionary<AbstractItem, string> itemPoolGroups;
        internal override HashSet<string> ItemPoolGroups => new(itemPoolGroups.Values);

        // Interop properties
        internal Sprite LocationSprite { get; private set; } = null;
        internal Dictionary<AbstractItem, Sprite> ItemSprites { get; private set; } = new();
        internal string[] HighlightScenes { get; private set; } = { };

        public float UpdateWaitSeconds { get; } = 1f;

        public IEnumerator PeriodicUpdate()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);
                UpdatePinSprite();
            }
        }

        internal void Initialize(AbstractPlacement placement)
        {
            this.placement = placement;

            LocationPoolGroup = SupplementalMetadata.OfPlacementAndLocations(placement).Get(InjectedProps.LocationPoolGroup);
            if (LocationPoolGroup == null)
            {
                LocationPoolGroup = PoolGroup.Other.FriendlyName();
            }

            itemPoolGroups = new();
            foreach (AbstractItem item in placement.Items)
            {
                itemPoolGroups[item] = SupplementalMetadata.Of(item).Get(InjectedProps.ItemPoolGroup);
                if (itemPoolGroups[item] == null)
                {
                    itemPoolGroups[item] = PoolGroup.Other.FriendlyName();
                }
            }

            Sprite locationSprite = null;
            if (placement.GetTags<InteropTag>().Any(tag => tag.TryGetProperty("LocationPinSprite", out locationSprite)))
            {
                LocationSprite = locationSprite;
            }

            foreach (AbstractItem item in placement.Items)
            {
                Sprite itemSprite = null;
                if (item.GetTags<InteropTag>().Any(tag => tag.TryGetProperty("ItemPinSprite", out itemSprite)))
                {
                    ItemSprites[item] = itemSprite;
                }
            }

            string[] highlightScenes = { };
            if (placement.GetTags<InteropTag>().Any(tag => tag.TryGetProperty("HighlightScenes", out highlightScenes)))
            {
                HighlightScenes = highlightScenes;
                // Place over panel
                Initialize();
                return;
            }

            MapLocation[] mapLocations = { };
            if (placement.GetTags<InteropTag>().Any(tag => tag.TryGetProperty("MapLocations", out mapLocations)))
            {
                Initialize(mapLocations);
                return;
            }

            Initialize(MapChanger.Finder.GetLocation(placement.Name).MapLocations);
        }

        private protected override bool LocationNotCleared()
        {
            return placementState != RandoPlacementState.Cleared
                && (placementState != RandoPlacementState.ClearedPersistent || RandoMapMod.GS.PersistentOn);
        }

        private protected override void OnEnable()
        {
            UpdatePlacementState();
            UpdatePinSprite();
            UpdateBorderColor();

            StartCoroutine(PeriodicUpdate());

            base.OnEnable();
        }

        private protected override void UpdatePinSize()
        {
            Size = pinSizes[RandoMapMod.GS.PinSize];

            if (Selected)
            {
                Size *= SELECTED_MULTIPLIER;
            }
            else if (placementState is RandoPlacementState.UncheckedUnreachable)
            {
                Size *= UNREACHABLE_SIZE_MULTIPLIER;
            }
        }

        private protected override void UpdatePinColor()
        {
            Vector4 color = UnityEngine.Color.white;

            if (placementState == RandoPlacementState.UncheckedUnreachable && !Selected)
            {
                Color = new Vector4(color.x * UNREACHABLE_COLOR_MULTIPLIER, color.y * UNREACHABLE_COLOR_MULTIPLIER, color.z * UNREACHABLE_COLOR_MULTIPLIER, color.w);
                return;
            }

            Color = color;
        }

        private protected override void UpdateBorderColor()
        {
            Vector4 color = placementState switch
            {
                RandoPlacementState.OutOfLogicReachable => Colors.GetColor(ColorSetting.Pin_Out_of_logic),
                RandoPlacementState.Previewed => Colors.GetColor(ColorSetting.Pin_Previewed),
                RandoPlacementState.ClearedPersistent => Colors.GetColor(ColorSetting.Pin_Persistent),
                _ => Colors.GetColor(ColorSetting.Pin_Normal),
            };

            if (placementState == RandoPlacementState.UncheckedUnreachable && !Selected)
            {
                BorderColor = new Vector4(color.x * UNREACHABLE_COLOR_MULTIPLIER, color.y * UNREACHABLE_COLOR_MULTIPLIER, color.z * UNREACHABLE_COLOR_MULTIPLIER, color.w);
            }
            else
            {
                BorderColor = color;
            }
        }

        private void UpdatePlacementState()
        {
            if (RM.RS.TrackerData.clearedLocations.Contains(name))
            {
                if (placement.IsPersistent())
                {
                    placementState = RandoPlacementState.ClearedPersistent;
                }
                else
                {
                    placementState = RandoPlacementState.Cleared;
                }
            }
            else if (placement.CanPreview() && placement.IsPreviewed())
            {
                placementState = RandoPlacementState.Previewed;
            }
            else if (RM.RS.TrackerDataWithoutSequenceBreaks.uncheckedReachableLocations.Contains(name))
            {
                placementState = RandoPlacementState.UncheckedReachable;
            }
            else if (RM.RS.TrackerData.uncheckedReachableLocations.Contains(name))
            {
                placementState = RandoPlacementState.OutOfLogicReachable;
            }
            else
            {
                placementState = RandoPlacementState.UncheckedUnreachable;
            }

            itemIndex = -1;

            if (RandoMapMod.GS.PersistentOn)
            {
                remainingItems = placement.Items.Where(item => !item.WasEverObtained() || item.IsPersistent());
            }
            else
            {
                remainingItems = placement.Items.Where(item => !item.WasEverObtained());
            }
        }

        private void UpdatePinSprite()
        {
            if (RandoMapMod.LS.SpoilerOn)
            {
                Sprite = GetItemSprite();
            }
            else
            {
                Sprite = placementState switch
                {
                    RandoPlacementState.UncheckedUnreachable
                    or RandoPlacementState.UncheckedReachable
                    or RandoPlacementState.OutOfLogicReachable => GetLocationSprite(),
                    RandoPlacementState.Previewed
                    or RandoPlacementState.ClearedPersistent => GetItemSprite(),
                    _ => GetLocationSprite(),
                };
            }

            Sprite GetLocationSprite()
            {
                if (LocationSprite is not null) return LocationSprite;
                return PinSprites.GetSpriteFromPoolGroup(LocationPoolGroup);
            }
            Sprite GetItemSprite()
            {
                itemIndex = (itemIndex + 1) % remainingItems.Count();
                AbstractItem item = remainingItems.ElementAt(itemIndex);

                if (ItemSprites.TryGetValue(item, out Sprite itemSprite)) return itemSprite;
                return PinSprites.GetSpriteFromPoolGroup(itemPoolGroups[item], true);
            }
        }

        internal override void GetLookupText()
        {
            throw new NotImplementedException();
        }
    }
}
