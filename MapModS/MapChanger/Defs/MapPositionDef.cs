using GlobalEnums;
using Newtonsoft.Json;

namespace MapChanger.Defs
{
    public record MapPositionDef : IMapPosition
    {
        [JsonConstructor]
        public MapPositionDef(string name, string sceneName, float offsetX, float offsetY)
        {
            Name = name;
            SceneName = sceneName;
            MappedScene = Finder.GetMappedScene(sceneName);
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public string Name { get; set; }
        public string SceneName { get; set; }
        public string MappedScene { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public MapZone MapZone => Finder.GetMapZone(SceneName);
    }
}