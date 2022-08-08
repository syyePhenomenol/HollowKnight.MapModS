using System;
using System.Collections.Generic;
using MapChanger.MonoBehaviours;
using RandoMapMod.Modes;
using RandoMapMod.UI;

namespace RandoMapMod.Pins
{

    internal class RmmPinSelector : Selector
    {
        internal static RmmPinSelector Instance { get; private set; }

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

        protected override void Select(ISelectable selectable)
        {
            if (selectable is RmmPin pin)
            {
                RandoMapMod.Instance.LogDebug($"Selected {pin.name}");
                pin.Selected = true;
            }
        }

        protected override void Deselect(ISelectable selectable)
        {
            if (selectable is RmmPin pin)
            {
                RandoMapMod.Instance.LogDebug($"Deselected {pin.name}");
                pin.Selected = false;
            }
        }

        protected override void OnSelectionChanged()
        {
            InfoPanels.UpdateLookupPanel();
        }

        private bool ActiveByCurrentMode()
        {
            return Conditions.RandoMapModEnabled();
        }

        private bool ActiveByToggle()
        {
            return RandoMapMod.GS.LookupOn;
        }

        internal static string GetLookupText()
        {
            if (RmmPinManager.Pins.TryGetValue(Instance.SelectedObjectKey, out RmmPin pin))
            {
                return pin.GetLookupText();
            }

            return "";
        }
    }
}
