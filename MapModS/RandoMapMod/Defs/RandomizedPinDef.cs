using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using MapChanger;
using UnityEngine;
using RM = RandomizerMod.RandomizerMod;

namespace RandoMapMod.Defs
{
    public class RandomizedPinDef : RMMPinDef
    {
        private readonly AbstractPlacement placement;
        public readonly Dictionary<AbstractItem, string> itemPoolGroups;
        private RandoPlacementState placementState;

        internal override HashSet<string> ItemPoolGroups => new(itemPoolGroups.Values);

        private IEnumerable<AbstractItem> remainingItems;
        private int itemIndex = 0;

        internal RandomizedPinDef(AbstractPlacement placement) : base(MapChanger.Finder.GetLocation(placement.Name))
        {
            if (MapPosition is null)
            {
                MapChangerMod.Instance.LogWarn($"No valid MapPositionDef found for RandoPinDef! {placement.Name}");
            }

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
        }

        public override void Update()
        {
            if (RM.RS.TrackerData.clearedLocations.Contains(Name))
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
            else if (RM.RS.TrackerDataWithoutSequenceBreaks.uncheckedReachableLocations.Contains(Name))
            {
                placementState = RandoPlacementState.UncheckedReachable;
            }
            else if (RM.RS.TrackerData.uncheckedReachableLocations.Contains(Name))
            {
                placementState = RandoPlacementState.OutOfLogicReachable;
            }
            else
            {
                placementState = RandoPlacementState.UncheckedUnreachable;
            }

            UpdateRemainingItems();

            Active = RandoMapMod.LS.GroupBy switch
            {
                Settings.GroupBySetting.Location => RandoMapMod.LS.GetPoolGroupSetting(LocationPoolGroup) == Settings.PoolState.On || RandoMapMod.LS.RandomizedOn,
                Settings.GroupBySetting.Item => itemPoolGroups.Values.Any(poolGroup => RandoMapMod.LS.GetPoolGroupSetting(poolGroup) == Settings.PoolState.On || RandoMapMod.LS.RandomizedOn),
                _ => true
            }
            && placementState != RandoPlacementState.Cleared
            && (placementState != RandoPlacementState.ClearedPersistent || RandoMapMod.GS.PersistentOn);
            base.Update();
        }

        public override Sprite GetSprite()
        {
            if (RandoMapMod.LS.SpoilerOn)
            {
                return GetItemSprite();
            }
            else
            {
                return placementState switch
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
                return PinSprites.GetSpriteFromPoolGroup(LocationPoolGroup);
            }
            Sprite GetItemSprite()
            {
                return PinSprites.GetSpriteFromPoolGroup(GetNextItemPoolGroup(), true);
            }
        }

        public override Vector4 GetSpriteColor()
        {
            Vector4 color = Color.white;

            if (placementState == RandoPlacementState.UncheckedUnreachable)
            {
                return new Vector4(color.x * UNREACHABLE_COLOR_SCALE, color.y * UNREACHABLE_COLOR_SCALE, color.z * UNREACHABLE_COLOR_SCALE, color.w);
            }
            return color;
        }

        public override Vector4 GetBorderColor()
        {
            //if (Selected)
            //{
            //    // do something
            //}

            // Need to grey out properly

            Vector4 color = placementState switch
            {
                RandoPlacementState.OutOfLogicReachable => Colors.GetColor(ColorSetting.Pin_Out_of_logic),
                RandoPlacementState.Previewed => Colors.GetColor(ColorSetting.Pin_Previewed),
                RandoPlacementState.ClearedPersistent => Colors.GetColor(ColorSetting.Pin_Persistent),
                _ => Colors.GetColor(ColorSetting.Pin_Normal),
            };

            if (placementState == RandoPlacementState.UncheckedUnreachable)
            {
                return new Vector4(color.x * UNREACHABLE_COLOR_SCALE, color.y * UNREACHABLE_COLOR_SCALE, color.z * UNREACHABLE_COLOR_SCALE, color.w);
            }
            return color;
        }

        public override float GetScale()
        {
            float scale = GetPinSizeScale();

            //if (Selected)
            //{
            //    scale *= SELECTED_SIZE_SCALE;
            //}
            //else
            //{
            if (placementState == RandoPlacementState.UncheckedUnreachable)
            {
                scale *= UNREACHABLE_SIZE_SCALE;
            }
            else
            {
                scale *= REACHABLE_SIZE_SCALE;
            }

            return scale;
        }

        private void UpdateRemainingItems()
        {
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

        private string GetNextItemPoolGroup()
        {
            itemIndex = (itemIndex + 1) % remainingItems.Count();
            return itemPoolGroups[remainingItems.ElementAt(itemIndex)];
        }

        internal override string[] GetPreviewText()
        {
            return placement.PreviewText();
        }
    }
}
