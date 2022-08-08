using MapChanger;
using MapChanger.Defs;

namespace RandoMapMod
{
    internal static class Debugger
    {
        internal static void LogMapPosition()
        {
            //RandoMapMod.Instance.LogDebug($"Real world position: {HeroController.instance.transform.position.x}, {HeroController.instance.transform.position.y}");

            WorldMapPosition wmp = new(new MapLocation[]
            {
                new()
                {
                    MappedScene = Utils.CurrentScene(),
                    X = HeroController.instance.transform.position.x,
                    Y = HeroController.instance.transform.position.y
                },
            });

            RandoMapMod.Instance.LogDebug($"Absolute offset on map: {wmp.X}, {wmp.Y}");
            RandoMapMod.Instance.LogDebug($"Relative offset from center of map room: {wmp.RelativeX}, {wmp.RelativeY}");
        }
    }
}
