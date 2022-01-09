using GlobalEnums;

namespace MapModS.Data
{
    public class VanillaItemDef
    {
        public string name;

        //// The list of objects that when all found, the pin will disappear
        //public string[] vanillaObjectName;

        public string sceneName;

        public string objectName;
        public string playerDataSetting;

        // The pools/groups the pin belongs to
        public PoolGroup pool;
    }
}