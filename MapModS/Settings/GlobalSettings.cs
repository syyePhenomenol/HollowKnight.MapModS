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
        //public float PinScaleSize = 0.36f;

        public PinSize PinSizeSetting = PinSize.Medium;

        public void TogglePinSize()
        {
            switch (PinSizeSetting)
            {
                case PinSize.Small:
                case PinSize.Medium:
                    PinSizeSetting += 1;
                    break;
                default:
                    PinSizeSetting = PinSize.Small;
                    break;
            }
        }
    }
}