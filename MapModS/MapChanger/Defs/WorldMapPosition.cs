using GlobalEnums;
using MapChanger.Map;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace MapChanger.Defs
{
    /// <summary>
    /// Interprets the X and Y values of the input MapLocations
    /// as the real world coordinates from the center of the scene, and scaled
    /// correctly to the room on the map.
    /// The first MapLocation that has a MappedScene corresponding to a room sprite is used.
    /// </summary>
    public record WorldMapPosition : MapPosition
    {
        public WorldMapPosition((string, float, float)[] mapLocations) : base(mapLocations) { }

        public WorldMapPosition(MapLocation[] mapLocations) : base(mapLocations) { }

        protected private override bool TrySetPosition(MapLocation mapLocation)
        {
            if (!BuiltInObjects.TryGetMapRoomPosition(mapLocation.MappedScene, out float baseX, out float baseY)
                || !BuiltInObjects.MappedRooms.TryGetValue(mapLocation.MappedScene, out RoomSprite roomSprite)
                || !Finder.TryGetTileMapDef(mapLocation.MappedScene, out TileMapDef tmd)) return false;

            Vector2 spriteSize = roomSprite.GetComponent<SpriteRenderer>().sprite.bounds.size;
            X = baseX - spriteSize.x / 2f + mapLocation.X / tmd.Width * spriteSize.x;
            Y = baseY - spriteSize.y / 2f + mapLocation.Y / tmd.Height * spriteSize.y;
            return true;
        }
    }
}
