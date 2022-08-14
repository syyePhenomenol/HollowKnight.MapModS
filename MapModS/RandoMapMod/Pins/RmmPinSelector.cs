using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MapChanger;
using MapChanger.MonoBehaviours;
using RandoMapMod.Modes;
using RandoMapMod.Rooms;
using RandoMapMod.UI;
using UnityEngine;
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

            animateHighlightedRooms = AnimateHighlightedRooms();
        }

        private Stopwatch attackHoldTimer = new();

        private void Update()
        {
            // Press dream nail to toggle lock selection
            if (InputHandler.Instance.inputActions.dreamNail.WasPressed)
            {
                ToggleLockSelection();
                SelectionPanels.UpdatePinPanel();
            }

            // Hold attack to benchwarp
            if (InputHandler.Instance.inputActions.attack.WasPressed)
            {
                attackHoldTimer.Restart();
            }

            if (InputHandler.Instance.inputActions.attack.WasReleased)
            {
                attackHoldTimer.Reset();
            }

            if (attackHoldTimer.ElapsedMilliseconds >= 500 && BenchwarpInterop.IsVisitedBench(SelectedObjectKey))
            {
                attackHoldTimer.Reset();
                GameManager.instance.StartCoroutine(BenchwarpInterop.DoBenchwarp(SelectedObjectKey));
            }
        }

        public override void OnMainUpdate(bool active)
        {
            base.OnMainUpdate(active);

            SpriteObject.SetActive(RandoMapMod.GS.ShowReticle);

            StopCoroutine(animateHighlightedRooms);
            StartCoroutine(animateHighlightedRooms);
        }

        protected override void Select(ISelectable selectable)
        {
            if (selectable is RmmPin pin)
            {
                RandoMapMod.Instance.LogDebug($"Selected {pin.name}");
                pin.Selected = true;

                if (pin is RandomizedRmmPin randoPin)
                {
                    SetHighlightedRooms(randoPin);
                }
            }

            static void SetHighlightedRooms(RandomizedRmmPin randoPin)
            {
                if (randoPin.HighlightRooms is null) return;

                HighlightedRooms = new(randoPin.HighlightRooms);
            }
        }

        protected override void Deselect(ISelectable selectable)
        {
            if (selectable is RmmPin pin)
            {
                RandoMapMod.Instance.LogDebug($"Deselected {pin.name}");
                pin.Selected = false;
            }

            foreach (ISelectable room in HighlightedRooms)
            {
                if (room is RoomSprite sprite)
                {
                    sprite.UpdateColor();
                }

                if (room is RoomText text)
                {
                    text.UpdateColor();
                }
            }

            HighlightedRooms.Clear();
        }

        private const int HIGHLIGHT_HALF_PERIOD = 25;
        private const int HIGHLIGHT_PERIOD = HIGHLIGHT_HALF_PERIOD * 2;

        private static int highlightAnimationTick = 0;

        private IEnumerator animateHighlightedRooms;
        private IEnumerator AnimateHighlightedRooms()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

                highlightAnimationTick = (highlightAnimationTick + 1) % HIGHLIGHT_PERIOD;

                Vector4 color = RmmColors.GetColor(RmmColorSetting.Room_Benchwarp_Selected);
                color.w = 0.3f + TriangleWave(highlightAnimationTick) * 0.7f;

                foreach (ISelectable room in HighlightedRooms)
                {
                    if (room is RoomSprite roomSprite)
                    {
                        roomSprite.Color = color;
                    }
                    else if (room is RoomText text)
                    {
                        text.Color = color;
                    }
                }
            }

            static float TriangleWave(float x)
            {
                return Math.Abs(x - HIGHLIGHT_HALF_PERIOD) / HIGHLIGHT_HALF_PERIOD;
            }
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
                string text = pin.GetSelectionText();

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
