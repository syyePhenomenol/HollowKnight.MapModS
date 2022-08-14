using System.Collections.Generic;
using ItemChanger;
using MapChanger;
using MapChanger.Defs;
using RandoMapMod.Modes;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace RandoMapMod.Pins
{
    internal sealed class BenchPin : RmmPin
    {
        private static readonly Vector4 unvisitedColor = new(UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER, 1f);
        private static readonly Vector4 visitedColor = UnityEngine.Color.white;

        internal override HashSet<string> ItemPoolGroups => new() { "Benches" };

        private static readonly ISprite benchSprite = new PinLocationSprite("Benches");

        internal string BenchName { get; private set; }

        internal void Initialize(string benchName)
        {
            Initialize();

            BenchName = benchName;

            SceneName = BenchwarpInterop.GetBenchKey(benchName).Item1;

            LocationPoolGroup = "Benches";

            if (InteropProperties.GetDefaultMapLocations(benchName) is (string, float, float)[] mapLocations)
            {
                MapPosition mapPosition = new(mapLocations);
                MapPosition = mapPosition;
                MapZone = mapPosition.MapZone;
            }
            else if (benchName is BenchwarpInterop.BENCH_WARP_START)
            {
                StartDef start = ItemChanger.Internal.Ref.Settings.Start;

                if (MapChanger.Finder.IsMappedScene(SceneName))
                {
                    WorldMapPosition wmp = new(new (string, float, float)[] { (SceneName, start.X, start.Y) });
                    MapPosition = wmp;
                    MapZone = wmp.MapZone;
                }
                else
                {
                    string mappedScene = MapChanger.Finder.GetMappedScene(SceneName);
                    MapPosition mapPosition = new(new (string, float, float)[] { (mappedScene, 0, 0) });
                    MapPosition = mapPosition;
                    MapZone = mapPosition.MapZone;
                }
            }
            else
            {
                RmmPinManager.MiscPins.Add(this);
            }
        }

        private protected override bool ActiveBySettings()
        {
            return Conditions.ItemRandoModeEnabled() && RandoMapMod.GS.ShowBenchwarpPins && (!BenchRandoInterop.HasBenchPlacements() || IsVisited());
        }

        protected private override bool LocationNotCleared()
        {
            return true;
        }

        private protected override void UpdatePinSprite()
        {
            Sprite = benchSprite.Value;
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
            if (IsVisited())
            {
                Color = visitedColor;
            }
            else
            {
                Color = unvisitedColor;
            }
        }

        private protected override void UpdateBorderColor()
        {
            Vector4 color;

            if (IsVisited())
            {
                color = RmmColors.GetColor(RmmColorSetting.Pin_Cleared);
            }
            else
            {
                color = RmmColors.GetColor(RmmColorSetting.Pin_Normal);
                color.x *= UNREACHABLE_COLOR_MULTIPLIER;
                color.y *= UNREACHABLE_COLOR_MULTIPLIER;
                color.z *= UNREACHABLE_COLOR_MULTIPLIER;
            }

            BorderColor = color;
        }

        internal override string GetSelectionText()
        {
            string text = $"{BenchName.ToCleanName()}";

            if (SceneName is not null)
            {
                text += $"\n\n{L.Localize("Room")}: {SceneName}";
            }

            text += $"\n\n{L.Localize("Status")}:";

            if (IsVisited())
            {
                text += $" {L.Localize("Visited")}";
            }
            else
            {
                text += $" {L.Localize("Not visited")}";
            }

            if (IsVisited())
            {
                List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.attack.Bindings);

                text += $"\n\n{L.Localize("Hold")} {Utils.GetBindingsText(bindings)} {L.Localize("to benchwarp")}.";
            }

            return text; 
        }

        private bool IsVisited()
        {
            return BenchwarpInterop.IsVisitedBench(BenchName);
        }
    }
}
