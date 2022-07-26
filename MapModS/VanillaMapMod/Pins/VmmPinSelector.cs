using System.Collections.Generic;
using MapChanger.MonoBehaviours;

namespace VanillaMapMod
{

    internal class VmmPinSelector : Selector
    {
        internal void Initialize(List<VmmPin> pins)
        {
            base.Initialize();

            ActiveModifiers.Add(ActiveByCurrentMode);

            foreach (VmmPin pin in pins)
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
            if (selectable is VmmPin pin)
            {
                VanillaMapMod.Instance.LogDebug($"Selected {pin.name}");
                pin.Selected = true;
            }
        }

        protected override void Deselect(ISelectable selectable)
        {
            if (selectable is VmmPin pin)
            {
                VanillaMapMod.Instance.LogDebug($"Deselected {pin.name}");
                pin.Selected = false;
            }
        }

        private bool ActiveByCurrentMode()
        {
            return MapChanger.Settings.CurrentMode() is NormalMode or FullMapMode;
        }
    }
}
