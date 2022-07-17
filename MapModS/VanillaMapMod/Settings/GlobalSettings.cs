using System;

namespace VanillaMapMod.Settings
{
    public class GlobalSettings
    {
        public PinStyle PinStyle = PinStyle.Normal;
        public PinSize PinSize = PinSize.Medium;

        internal void TogglePinStyle()
        {
            PinStyle = (PinStyle)(((int)PinStyle + 1) % Enum.GetNames(typeof(PinStyle)).Length);
        }

        internal void TogglePinSize()
        {
            PinSize = (PinSize)(((int)PinSize + 1) % Enum.GetNames(typeof(PinSize)).Length);
        }
    }
}