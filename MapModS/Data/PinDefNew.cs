using GlobalEnums;

namespace MapModS.Data
{
    public class PinDefNew
    {
        public string name;

        // The name of the scene
        public string sceneName;

        // For non-randomized item tracking
        public string objectName;

        public string pdBool;
        public string pdInt;
        public int pdIntValue;

        // The name of the scene the pin belongs to, an override to sceneName in some cases
        public string pinScene;

        // The map area/zone the pin belongs to
        public MapZone mapZone;

        // The local offset of the pin relative to its pinScene/sceneName map object
        public float offsetX;

        public float offsetY;
        public float offsetZ;

        public bool isShop = false;
    }
}