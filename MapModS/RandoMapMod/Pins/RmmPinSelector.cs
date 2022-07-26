using System.Collections.Generic;
using MapChanger.MonoBehaviours;

namespace RandoMapMod.Pins
{

    internal class RmmPinSelector : Selector
    {
        internal void Initialize(IEnumerable<RmmPin> pins)
        {
            base.Initialize();

            ActiveModifiers.Add(ActiveByCurrentMode);

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

        private bool ActiveByCurrentMode()
        {
            return MapChanger.Settings.CurrentMode().Mod is "RandoMapMod";
        }
    }
}
