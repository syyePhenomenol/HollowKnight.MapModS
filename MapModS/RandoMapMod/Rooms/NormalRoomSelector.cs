using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MapChanger;
using MapChanger.Map;
using MapChanger.MonoBehaviours;
using RandoMapMod.Modes;
using RandoMapMod.UI;
using L = RandomizerMod.Localization;

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

            ActiveModifiers.Add(ActiveByToggle);
        }

        private static readonly Stopwatch attackHoldTimer = new();
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
                }

                attackHoldTimer.Reset();
            }

            if (attackHoldTimer.ElapsedMilliseconds >= 500 && SelectedObjectKey is not NONE_SELECTED)
            {
                attackHoldTimer.Reset();
                GameManager.instance.StartCoroutine(BenchwarpInterop.DoBenchwarp(SelectedObjectKey, benchPointer));
            }
        }

        private void ToggleBench()
        {
            if (BenchwarpInterop.Benches.TryGetValue(SelectedObjectKey, out List<WorldMapBenchDef> defs) && defs.Count > 1)
            {
                benchPointer = (benchPointer + 1) % defs.Count;
                RandoMapMod.Instance.LogDebug($"Toggled bench to {defs[benchPointer]}");
                OnSelectionChanged();
            }
        }

        public override void AfterMainUpdate()
        {
            attackHoldTimer.Reset();
        }

        protected private override bool ActiveByCurrentMode()
        {
            return MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(NormalMode));
        }

        private bool ActiveByToggle()
        {
            return RandoMapMod.GS.BenchwarpWorldMap;
        }

        protected override void Deselect(ISelectable selectable)
        {
            benchPointer = 0;

            base.Deselect(selectable);
        }

        protected override void OnSelectionChanged()
        {
            BenchwarpText.Instance.Update();
        }

        internal static string GetBenchwarpText()
        {
            string text = "";

            if (Instance.SelectedObjectKey is NONE_SELECTED) return text;

            List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.attack.Bindings);

            if (BenchwarpInterop.Benches.TryGetValue(Instance.SelectedObjectKey, out List<WorldMapBenchDef> defs))
            {
                text += $"{L.Localize("Hold")} {Utils.GetBindingsText(bindings)} {L.Localize("to warp to")} {defs[benchPointer].benchName.Replace("Warp ", "").Replace("Bench ", "")}.";

                if (defs.Count > 1)
                {
                    text += $"\n{L.Localize("Tap")} {Utils.GetBindingsText(bindings)} {L.Localize("to toggle to another bench here")}.";
                }
            }

            return text;
        }
    }
}
