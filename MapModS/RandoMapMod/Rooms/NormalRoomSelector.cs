using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MapChanger;
using MapChanger.Map;
using MapChanger.MonoBehaviours;
using RandoMapMod.Modes;

namespace RandoMapMod.Rooms
{
    internal class NormalRoomSelector : RoomSelector
    {
        internal static NormalRoomSelector Instance;

        public override float SelectionRadius { get; } = 20.0f;

        private static int benchPointer = 0;

        internal new void Initialize()
        {
            Instance = this;

            IEnumerable<string> benchMappedScenes = BenchwarpInterop.benchKeys.Values.Select(benchKey => Finder.GetMappedScene(benchKey.Item1));

            IEnumerable<RoomSprite> rooms = BuiltInObjects.MappedRooms.Values.Where(roomSprite => benchMappedScenes.Contains(roomSprite.Rsd.SceneName));

            base.Initialize(rooms);
        }

        public static Stopwatch attackHoldTimer = new();

        private void Update()
        {
            // Hold attack to benchwarp
            if (InputHandler.Instance.inputActions.attack.WasPressed)
            {
                attackHoldTimer.Restart();
            }

            if (InputHandler.Instance.inputActions.attack.WasReleased)
            {
                if (attackHoldTimer.ElapsedMilliseconds < 500)
                {
                    ToggleBench();
                    //UpdateBenchwarpText();
                }

                attackHoldTimer.Reset();
            }

            if (attackHoldTimer.ElapsedMilliseconds >= 500 && SelectedObjectKey is not NONE_SELECTED)
            {
                attackHoldTimer.Reset();
                GameManager.instance.StartCoroutine(BenchwarpInterop.DoBenchwarp(SelectedObjectKey, benchPointer));
                return;
            }
        }

        private void ToggleBench()
        {
            if (!BenchwarpInterop.Benches.ContainsKey(SelectedObjectKey)
                || benchPointer > BenchwarpInterop.Benches[SelectedObjectKey].Count - 1)
            {
                RandoMapMod.Instance.LogWarn("Invalid bench toggle");
                return;
            }

            benchPointer = (benchPointer + 1) % BenchwarpInterop.Benches[SelectedObjectKey].Count;
            RandoMapMod.Instance.LogDebug($"Toggled bench to {GetSelectedBench().benchName}");
        }

        private WorldMapBenchDef GetSelectedBench()
        {
            if (!BenchwarpInterop.Benches.ContainsKey(SelectedObjectKey)
                || benchPointer > BenchwarpInterop.Benches[SelectedObjectKey].Count - 1)
            {
                RandoMapMod.Instance.LogWarn("Invalid bench selection");
                return BenchwarpInterop.Benches.First().Value.First();
            }

            return BenchwarpInterop.Benches[SelectedObjectKey][benchPointer];
        }

        protected private override bool ActiveByCurrentMode()
        {
            return MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(NormalMode));
        }

        protected override void Deselect(ISelectable selectable)
        {
            benchPointer = 0;

            base.Deselect(selectable);
        }
    }
}
