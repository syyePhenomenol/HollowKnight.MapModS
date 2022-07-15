namespace MapChanger.Defs
{
    public abstract class MapObjectDef
    {
        public string Name { get => MapPosition.Name; }
        public string SceneName { get => MapPosition.SceneName; }
        public bool Active { get; private protected set; }

        public MapLocationDef MapPosition { get; private set; }

        public MapObjectDef(MapLocationDef mpd)
        {
            MapPosition = mpd;
        }

        public virtual void Update()
        {
            Active = Active
                && ((States.QuickMapOpen && States.CurrentMapZone == MapPosition.MapZone)
                    || (States.WorldMapOpen && (Settings.CurrentMode().ForceFullMap || Utils.HasMapSetting(MapPosition.MapZone))));
        }

        public void SetPosition(IMapPosition mpd)
        {
            MapPosition.OffsetX = mpd.OffsetX;
            MapPosition.OffsetY = mpd.OffsetY;
            if (mpd.MappedScene != null)
            {
                MapPosition.MappedScene = mpd.MappedScene;
            }
        }
    }
}