using GlobalEnums;

namespace MapChanger.MonoBehaviours
{
    public interface IMapRoom
    {
        string SceneName { get; }
        MapZone MapZone { get; }
    }
}
