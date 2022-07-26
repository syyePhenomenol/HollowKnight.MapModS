using GlobalEnums;
using MapChanger.Map;

namespace MapChanger.Defs
{
    public record MapLocationPosition : IMapPosition
    {
        public float X { get; private set; } = 0f;
        public float Y { get; private set; } = 0f;

        public string MappedScene { get; private set; }
        public MapZone MapZone { get; private set; }

        public MapLocationPosition(MapLocation[] mapLocations)
        {
            foreach (MapLocation mapLocation in mapLocations)
            {
                if (TrySetPosition(mapLocation))
                {
                    break;
                }
            }
        }

        private bool TrySetPosition(MapLocation mapLocation)
        {
            if (!BuiltInObjects.TryGetMapRoomPosition(mapLocation.MappedScene, out float baseX, out float baseY))
            {
                return false;
            }

            X = baseX + mapLocation.X;
            Y = baseY + mapLocation.Y;
            MappedScene = mapLocation.MappedScene;
            MapZone = Finder.GetMapZone(mapLocation.MappedScene);
            return true;
        }
    }
}
