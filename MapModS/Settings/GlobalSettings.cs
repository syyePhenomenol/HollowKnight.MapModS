using MapModS.Map;

namespace MapModS.Settings
{
    public enum PinSize
    {
        Small,
        Medium,
        Large
    }
    public class GlobalSettings
    {
        public bool allowBenchWarpSearch = true;

        public bool uncheckedPanelActive = false;

        public PinSize pinSize = PinSize.Medium;

        public void ToggleAllowBenchWarp()
        {
            allowBenchWarpSearch = !allowBenchWarpSearch;
        }

        public void ToggleUncheckedPanel()
        {
            uncheckedPanelActive = !uncheckedPanelActive;
        }

        public void TogglePinSize()
        {
            switch (pinSize)
            {
                case PinSize.Small:
                case PinSize.Medium:
                    pinSize += 1;
                    break;
                default:
                    pinSize = PinSize.Small;
                    break;
            }
        }
    }
}