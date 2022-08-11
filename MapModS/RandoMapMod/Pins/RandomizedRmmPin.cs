using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConnectionMetadataInjector;
using ItemChanger;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using RandoMapMod.Defs;
using UnityEngine;
using L = RandomizerMod.Localization;
using RM = RandomizerMod.RandomizerMod;
using SD = ConnectionMetadataInjector.SupplementalMetadata;

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

        private ISprite locationSprite;
        private float locationSpriteScale = 1f;
        private Dictionary<AbstractItem, ISprite> itemSprites;
        private Dictionary<AbstractItem, float> itemSpriteScales;

        private bool showItemSprite = false;

        internal string[] HighlightScenes { get; private set; }

        public float UpdateWaitSeconds { get; } = 1f;

        public IEnumerator PeriodicUpdate()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

                itemIndex = (itemIndex + 1) % remainingItems.Count();
                UpdatePinSprite();
                UpdatePinSize();
            }
        }

        internal void Initialize(AbstractPlacement placement)
        {
            this.placement = placement;

            SceneName = placement.RandoModLocation()?.LocationDef?.SceneName ?? ItemChanger.Finder.GetLocation(name)?.sceneName;

            LocationPoolGroup = SD.OfPlacementAndLocations(placement).Get(InjectedProps.LocationPoolGroup);
            locationSprite = SD.OfPlacementAndLocations(placement).Get(InteropProperties.LocationPinSprite);
            
            if (SD.OfPlacementAndLocations(placement).Get(InteropProperties.LocationPinSpriteSize) is Vector2Int locationSize)
            {
                locationSpriteScale = SpriteManager.DEFAULT_PIN_SPRITE_SIZE / locationSize.Average();
            }

            itemPoolGroups = new();
            itemSprites = new();
            itemSpriteScales = new();
            foreach (AbstractItem item in placement.Items)
            {
                itemPoolGroups[item] = SD.Of(item).Get(InjectedProps.ItemPoolGroup);
                itemSprites[item] = SD.Of(item).Get(InteropProperties.ItemPinSprite);

                if (SD.Of(item).Get(InteropProperties.ItemPinSpriteSize) is Vector2Int itemSize)
                {
                    itemSpriteScales[item] = SpriteManager.DEFAULT_PIN_SPRITE_SIZE / itemSize.Average();
                }
                else
                {
                    itemSpriteScales[item] = 1f;
                }
            }

            HighlightScenes = SD.OfPlacementAndLocations(placement).Get(InteropProperties.HighlightScenes);

            //HighlightScenes = new string[] { "Town", "White_Palace_01", "Fungus1_35", "Ruins2_04" };

            if (SD.OfPlacementAndLocations(placement).Get(InteropProperties.WorldMapLocations) is (string, float, float)[] worldMapLocations)
            {
                WorldMapPosition wmp = new(worldMapLocations);
                MapPosition = wmp;
                MapZone = wmp.MapZone;
            }
            else if (SD.OfPlacementAndLocations(placement).Get(InteropProperties.AbsMapLocation) is Vector2 absMapLocation)
            {
                MapPosition = new AbsMapPosition(absMapLocation);
            }
            else if (SD.OfPlacementAndLocations(placement).Get(InteropProperties.MapLocations) is (string, float, float)[] mapLocations)
            {
                MapPosition mlp = new(mapLocations);
                MapPosition = mlp;
                MapZone = mlp.MapZone;
            }
            else
            {
                PlaceToMiscGrid();
            }

            Initialize();
        }

        protected private override bool ActiveByPoolSetting()
        {
            if (RandoMapMod.LS.GroupBy == Settings.GroupBySetting.Item)
            {
                foreach (string poolGroup in remainingItems.Select(item => itemPoolGroups[item]))
                {
                    Settings.PoolState poolState = RandoMapMod.LS.GetPoolGroupSetting(poolGroup);

                    if (poolState == Settings.PoolState.On || (poolState == Settings.PoolState.Mixed && RandoMapMod.LS.RandomizedOn))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                Settings.PoolState poolState = RandoMapMod.LS.GetPoolGroupSetting(LocationPoolGroup);

                return poolState == Settings.PoolState.On || (poolState == Settings.PoolState.Mixed && RandoMapMod.LS.RandomizedOn);
            }
        }

        protected private override bool LocationNotCleared()
        {
            return placementState != RandoPlacementState.Cleared
                && (placementState != RandoPlacementState.ClearedPersistent || RandoMapMod.GS.PersistentOn);
        }

        public override void OnMainUpdate(bool active)
        {
            if (!active) return;

            itemIndex = 0;

            if (RandoMapMod.GS.PersistentOn)
            {
                remainingItems = placement.Items.Where(item => !item.WasEverObtained() || item.IsPersistent());
            }
            else
            {
                remainingItems = placement.Items.Where(item => !item.WasEverObtained());
            }

            if (RandoMapMod.LS.SpoilerOn)
            {
                showItemSprite = true;
            }
            else
            {
                showItemSprite = placementState switch
                {
                    RandoPlacementState.UncheckedUnreachable
                    or RandoPlacementState.UncheckedReachable
                    or RandoPlacementState.OutOfLogicReachable => false,
                    RandoPlacementState.Previewed
                    or RandoPlacementState.ClearedPersistent => true,
                    _ => false,
                };
            }

            StopAllCoroutines();

            if (showItemSprite)
            {
                StartCoroutine(PeriodicUpdate());
            }

            base.OnMainUpdate(active);
        }

        protected private override void UpdatePinSprite()
        {
            if (showItemSprite)
            {
                Sprite = itemSprites[remainingItems.ElementAt(itemIndex)].Value;
            }
            else
            {
                Sprite = locationSprite.Value;
            }
        }

        protected private override void UpdatePinSize()
        {
            float size = pinSizes[RandoMapMod.GS.PinSize];

            if (showItemSprite)
            {
                size *= itemSpriteScales[remainingItems.ElementAt(itemIndex)];
            }
            else
            {
                size *= locationSpriteScale;
            }

            if (Selected)
            {
                size *= SELECTED_MULTIPLIER;
            }
            else if (placementState is RandoPlacementState.UncheckedUnreachable)
            {
                size *= UNREACHABLE_SIZE_MULTIPLIER;
            }

            Size = size;
        }

        protected private override void UpdatePinColor()
        {
            Vector4 color = UnityEngine.Color.white;

            if (placementState == RandoPlacementState.UncheckedUnreachable)
            {
                Color = new Vector4(color.x * UNREACHABLE_COLOR_MULTIPLIER, color.y * UNREACHABLE_COLOR_MULTIPLIER, color.z * UNREACHABLE_COLOR_MULTIPLIER, color.w);
                return;
            }

            Color = color;
        }

        protected private override void UpdateBorderColor()
        {
            Vector4 color = placementState switch
            {
                RandoPlacementState.OutOfLogicReachable => RmmColors.GetColor(RmmColorSetting.Pin_Out_of_logic),
                RandoPlacementState.Previewed => RmmColors.GetColor(RmmColorSetting.Pin_Previewed),
                RandoPlacementState.ClearedPersistent => RmmColors.GetColor(RmmColorSetting.Pin_Persistent),
                _ => RmmColors.GetColor(RmmColorSetting.Pin_Normal),
            };

            if (placementState == RandoPlacementState.UncheckedUnreachable)
            {
                BorderColor = new Vector4(color.x * UNREACHABLE_COLOR_MULTIPLIER, color.y * UNREACHABLE_COLOR_MULTIPLIER, color.z * UNREACHABLE_COLOR_MULTIPLIER, color.w);
            }
            else
            {
                BorderColor = color;
            }
        }

        internal void UpdatePlacementState()
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
        }

        internal override string GetLookupText()
        {
            string text = base.GetLookupText();

            if (HighlightScenes is not null)
            {
                text += $"\n\n{L.Localize("Rooms")}:";

                foreach (string scene in HighlightScenes)
                {
                    text += $" {scene},";
                }

                text = text.Substring(0, text.Length - 1);
            }

            text += $"\n\n{L.Localize("Status")}:";

            text += placementState switch
            {
                RandoPlacementState.UncheckedUnreachable => $" {L.Localize("Randomized, unchecked, unreachable")}",
                RandoPlacementState.UncheckedReachable => $" {L.Localize("Randomized, unchecked, reachable")}",
                RandoPlacementState.OutOfLogicReachable => $" {L.Localize("Randomized, unchecked, reachable through sequence break")}",
                RandoPlacementState.Previewed => $" {L.Localize("Randomized, previewed")}",
                RandoPlacementState.Cleared => $" {L.Localize("Cleared")}",
                RandoPlacementState.ClearedPersistent => $" {L.Localize("Randomized, cleared, persistent")}",
                _ => ""
            };

            text += $"\n\n{L.Localize("Logic")}: {Logic?? "not found"}";

            if (RM.RS.TrackerData.previewedLocations.Contains(name) && placement.TryGetPreviewText(out string[] previewText))
            {
                text += $"\n\n{L.Localize("Previewed item(s)")}:";

                foreach (string preview in previewText)
                {
                    text += $" {ToCleanPreviewText(preview)},";
                }

                text = text.Substring(0, text.Length - 1);
            }

            if (RandoMapMod.LS.SpoilerOn
                && !(RM.RS.TrackerData.previewedLocations.Contains(name) && placement.CanPreview()))
            {
                text += $"\n\n{L.Localize("Spoiler item(s)")}:";

                foreach (AbstractItem item in placement.Items)
                {
                    text += $" {Utils.ToCleanName(item.name)},";
                }

                text = text.Substring(0, text.Length - 1);
            }

            return text;

            static string ToCleanPreviewText(string text)
            {
                return text.Replace("Pay ", "")
                    .Replace("Once you own ", "")
                    .Replace(", I'll gladly sell it to you.", "")
                    .Replace("Requires ", "");
            }
        }
    }
}