using ConnectionMetadataInjector;
using ItemChanger;
using MapModS.Data;
using MapModS.Map;
using RandomizerMod.RC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CMI = ConnectionMetadataInjector.ConnectionMetadataInjector;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Pins
{
    public class RandoPinDef : RandomizerModPinDef
    {
        private readonly AbstractPlacement placement;
        private readonly Dictionary<AbstractItem, string> itemPoolGroups;
        private RandoPlacementState placementState;

        internal override HashSet<string> ItemPoolGroups => new(itemPoolGroups.Values);

        private IEnumerable<AbstractItem> remainingItems;
        private int itemIndex = 0;

        internal RandoPinDef(AbstractPlacement placement, RandoModLocation rml, VanillaPinDef vpd = null) : base()
        {
            this.placement = placement;

            Name = rml.Name;
            Scene = rml.LocationDef.SceneName;

            SetMapData();

            if (vpd != null)
            {
                OffsetX = vpd.OffsetX;
                OffsetY = vpd.OffsetY;
            }
            else
            {
                OffsetX = 0f;
                OffsetY = 0f;
            }

            LocationPoolGroup = SupplementalMetadata.OfPlacementAndLocations(placement).Get(CMI.LocationPoolGroup);
            if (LocationPoolGroup == null)
            {
                LocationPoolGroup = PoolGroupNames.UNKNOWN;
            }

            itemPoolGroups = new();
            foreach (AbstractItem item in placement.Items)
            {
                itemPoolGroups[item] = SupplementalMetadata.Of(item).Get(CMI.ItemPoolGroup);
                if (itemPoolGroups[item] == null)
                {
                    itemPoolGroups[item] = PoolGroupNames.UNKNOWN;
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

            Active = MapModS.LS.GroupBy switch
            {
                Settings.GroupBySetting.Location => MapModS.LS.GetPoolGroupSetting(LocationPoolGroup) == Settings.PoolState.On || MapModS.LS.RandomizedOn,
                Settings.GroupBySetting.Item => itemPoolGroups.Values.Any(poolGroup => MapModS.LS.GetPoolGroupSetting(poolGroup) == Settings.PoolState.On || MapModS.LS.RandomizedOn),
                _ => true
            }
            && placementState != RandoPlacementState.Cleared
            && (placementState != RandoPlacementState.ClearedPersistent || MapModS.GS.PersistentOn);
            base.Update();
        }

        public override Sprite GetMainSprite()
        {
            if (MapModS.LS.SpoilerOn)
            {
                return GetItemSprite();
            }
            else
            {
                switch (placementState)
                {
                    case RandoPlacementState.UncheckedUnreachable:
                    case RandoPlacementState.UncheckedReachable:
                    case RandoPlacementState.OutOfLogicReachable:
                        return GetLocationSprite();
                    case RandoPlacementState.Previewed:
                    case RandoPlacementState.ClearedPersistent:
                        return GetItemSprite();
                    default:
                        return GetLocationSprite();
                }
            }

            Sprite GetLocationSprite()
            {
                return SpriteManager.GetSpriteFromPoolGroup(LocationPoolGroup);
            }
            Sprite GetItemSprite()
            {
                return SpriteManager.GetSpriteFromPoolGroup(GetNextItemPoolGroup(), true);
            }
        }

        public override Vector4 GetMainColor()
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

        public override Vector3 GetScale()
        {
            Vector2 scale = Vector2.one;

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
            //}

            scale *= GetPinSizeScale();

            return new Vector3(scale.x, scale.y, 1f);
        }

        private void UpdateRemainingItems()
        {
            itemIndex = -1;

            if (MapModS.GS.PersistentOn)
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
