using System.Collections.Generic;
using System.Linq;
using MapChanger;
using MapChanger.Map;
using MapChanger.MonoBehaviours;
using RandoMapMod.Modes;
using RandoMapMod.UI;
using L = RandomizerMod.Localization;

namespace RandoMapMod.Rooms
{
    /// <summary>
    /// This selector is only active in item rando map modes.
    /// </summary>
    internal class BenchwarpRoomSelector : RoomSelector
    {
        internal static BenchwarpRoomSelector Instance;

        public override float SelectionRadius { get; } = 20.0f;

        private int benchPointer = 0;

        internal new void Initialize()
        {
            Instance = this;

            IEnumerable<string> benchMappedScenes = BenchwarpInterop.benchKeys.Values.Select(benchKey => Finder.GetMappedScene(benchKey.Item1));

            IEnumerable<RoomSprite> rooms = BuiltInObjects.MappedRooms.Values.Where(roomSprite => benchMappedScenes.Contains(roomSprite.Rsd.SceneName));

            base.Initialize(rooms);
        }

        protected private override bool ActiveByCurrentMode()
        {
            return MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(ItemRandoMode));
        }

        protected private override bool ActiveByToggle()
        {
            return RandoMapMod.GS.BenchwarpSelectionOn;
        }

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
                RandoMapMod.Instance.LogDebug($"Toggled bench to {defs[benchPointer].benchName}");
                SelectionPanels.UpdateBenchwarpPanel();
            }
        }

        protected override void OnSelectionChanged()
        {
            benchPointer = 0;
            SelectionPanels.UpdateBenchwarpPanel();
            InstructionText.Instance.Update();
        }

        internal string GetText()
        {
            string text = "";

            if (!BenchwarpInterop.Benches.TryGetValue(SelectedObjectKey, out List<WorldMapBenchDef> defs)) return text;

            List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.attack.Bindings);

            text += $"{L.Localize("Hold")} {Utils.GetBindingsText(bindings)} {L.Localize("to benchwarp")}.";

            if (defs.Count > 1)
            {
                text += $"\n{L.Localize("Tap")} {Utils.GetBindingsText(bindings)} {L.Localize("to toggle bench")}.";
            }

            for (int i = 0; i < defs.Count; i++)
            {
                text += "\n\n";

                text += defs[i].benchName.Replace("Warp ", "").Replace("Bench ", "");

                if (i == benchPointer)
                {
                    text += " <--";
                }
                else
                {
                    text += "    ";
                }
            }

            return text;
        }
    }
}
