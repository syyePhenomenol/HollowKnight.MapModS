using MapModS.Data;
using System.Collections.Generic;

namespace MapModS.Map
{
    internal static class MapData
    {
        public static Dictionary<string, MapPositionDef> RoomLookup;

        internal static void LoadGlobalMapDefs()
        {
            RoomLookup = JsonUtil.Deserialize<Dictionary<string, MapPositionDef>>("MapModS.Resources.rooms.json");
        }

        public static bool IsVanillaMapScene(string scene)
        {
            return RoomLookup.ContainsKey(scene) && RoomLookup[scene].MappedScene == scene;
        }
    }
}
