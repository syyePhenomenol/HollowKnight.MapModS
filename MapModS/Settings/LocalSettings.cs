using MapModS.Pins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapModS.Settings
{
    public abstract class LocalSettings
    {
        public bool InitializedPreviously = false;
        public HashSet<string> ScenesVisited;

        //// Vanilla only
        //public int geoRockCounter = 0;

        public bool ModEnabled = false;
        public MapMode Mode = MapMode.FullMap;
        public bool ShowBenchPins = false;

        public virtual void InitializeDerived()
        {

        }

        public void Initialize()
        {
            ScenesVisited = new(PlayerData.instance.scenesVisited);

            if (InitializedPreviously) return;

            InitializeDerived();

            InitializedPreviously = true;
        }

        internal void ToggleModEnabled()
        {
            ModEnabled = !ModEnabled;
        }

        internal void SetMode(MapMode mode)
        {
            Mode = mode;
        }

        internal void ToggleMode()
        {
            Mode = (MapMode)(((int)Mode + 1) % Enum.GetNames(typeof(MapMode)).Length);
        }
    }
}