using System;
using System.Collections.Generic;
using MapChanger;
using MapChanger.MonoBehaviours;
using RandoMapMod.Modes;
using RandoMapMod.Rooms;
using RandoMapMod.UI;
using L = RandomizerMod.Localization;

namespace RandoMapMod.Pins
{

    internal class RmmPinSelector : Selector
    {
        internal static RmmPinSelector Instance { get; private set; }

        internal static HashSet<ISelectable> HighlightedRooms { get; private set; } = new();

        internal void Initialize(IEnumerable<RmmPin> pins)
        {
            Instance = this;

            base.Initialize();

            ActiveModifiers.AddRange
            (
                new Func<bool>[]
                { 
                    ActiveByCurrentMode,
                    ActiveByToggle
                }
            );

            foreach (RmmPin pin in pins)
            {
                if (Objects.TryGetValue(pin.name, out List<ISelectable> selectables))
                {
                    selectables.Add(pin);
                }
                else
                {
                    Objects[pin.name] = new() { pin };
                }
            }
        }

        private void Update()
        {
            // Press dream nail to toggle lock selection
            if (InputHandler.Instance.inputActions.dreamNail.WasPressed)
            {
                ToggleLockSelection();
                SelectionPanels.UpdatePinPanel();
            }
        }

        public override void OnMainUpdate(bool active)
        {
            base.OnMainUpdate(active);

            SpriteObject.SetActive(RandoMapMod.GS.ShowReticle);
        }

        protected override void Select(ISelectable selectable)
        {
            if (selectable is RmmPin pin)
            {
                RandoMapMod.Instance.LogDebug($"Selected {pin.name}");
                pin.Selected = true;

                if (pin is RandomizedRmmPin rmmPin && rmmPin.HighlightScenes is string[] scenes)
                {
                    HighlightRooms(scenes);
                }
            }
        }

        protected override void Deselect(ISelectable selectable)
        {
            if (selectable is RmmPin pin)
            {
                RandoMapMod.Instance.LogDebug($"Deselected {pin.name}");
                pin.Selected = false;
            }

            UnhighlightRooms();
        }

        private void HighlightRooms(string[] scenes)
        {
            foreach (string scene in scenes)
            {
                if (!TransitionRoomSelector.Instance.Objects.TryGetValue(scene, out List<ISelectable> rooms)) continue;

                foreach (ISelectable room in rooms)
                {
                    HighlightedRooms.Add(room);
                    room.Selected = true;
                }
            }
        }

        private void UnhighlightRooms()
        {
            foreach (ISelectable room in HighlightedRooms)
            {
                room.Selected = false;
            }

            HighlightedRooms.Clear();
        }

        protected override void OnSelectionChanged()
        {
            SelectionPanels.UpdatePinPanel();
        }

        private bool ActiveByCurrentMode()
        {
            return Conditions.RandoMapModEnabled();
        }

        private bool ActiveByToggle()
        {
            return RandoMapMod.GS.PinSelectionOn;
        }

        internal string GetText()
        {
            if (RmmPinManager.Pins.TryGetValue(SelectedObjectKey, out RmmPin pin))
            {
                string text = pin.GetLookupText();

                List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.dreamNail.Bindings);

                if (LockSelection)
                {
                    text += $"\n\n{L.Localize("Press")} {Utils.GetBindingsText(bindings)} {L.Localize("to unlock pin selection")}.";
                }
                else
                {
                    text += $"\n\n{L.Localize("Press")} {Utils.GetBindingsText(bindings)} {L.Localize("to lock pin selection")}.";
                }

                return text;
            }

            return "";
        }
    }
}
