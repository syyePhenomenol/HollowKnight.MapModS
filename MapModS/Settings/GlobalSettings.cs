using System;

namespace MapModS.Settings
{
    public abstract class GlobalSettings
    {
        public bool ControlPanelOn = true;
        public bool MapKeyOn = false;
        public bool LookupOn = false;
        public bool BenchwarpWorldMap = true;
        public bool AllowBenchWarpSearch = true;
        public bool ShowUncheckedPanel = true;
        public RouteTextInGame RouteTextInGame = RouteTextInGame.NextTransitionOnly;
        public OffRouteBehaviour WhenOffRoute = OffRouteBehaviour.Reevaluate;
        public bool ShowRouteCompass = true;
        public PinStyle PinStyle = PinStyle.Normal;
        public PinSize PinSize = PinSize.Medium;
        public bool PersistentOn = false;

        /// <summary>
        /// By default, the mode is set to Full Map in item rando, and Transition in a transition rando (at
        /// least one randomized transition). Use the below settings to override them.
        /// </summary>
        public bool OverrideDefaultMode = false;
        public MapMode ItemRandoModeOverride = MapMode.FullMap;
        public MapMode TransitionRandoModeOverride = MapMode.Transition;

        internal void ToggleControlPanel()
        {
            ControlPanelOn = !ControlPanelOn;
        }

        internal void ToggleMapKey()
        {
            MapKeyOn = !MapKeyOn;
        }

        internal void ToggleLookup()
        {
            LookupOn = !LookupOn;
        }

        internal void ToggleBenchwarpWorldMap()
        {
            BenchwarpWorldMap = !BenchwarpWorldMap;
        }

        internal void ToggleAllowBenchWarp()
        {
            AllowBenchWarpSearch = !AllowBenchWarpSearch;
        }

        internal void ToggleUncheckedPanel()
        {
            ShowUncheckedPanel = !ShowUncheckedPanel;
        }

        internal void ToggleRouteTextInGame()
        {
            RouteTextInGame = (RouteTextInGame)(((int)RouteTextInGame + 1) % Enum.GetNames(typeof(RouteTextInGame)).Length);
        }

        internal void ToggleWhenOffRoute()
        {
            WhenOffRoute = (OffRouteBehaviour)(((int)WhenOffRoute + 1) % Enum.GetNames(typeof(OffRouteBehaviour)).Length);
        }

        internal void ToggleRouteCompassEnabled()
        {
            ShowRouteCompass = !ShowRouteCompass;
        }

        internal void TogglePinStyle()
        {
            PinStyle = (PinStyle)(((int)PinStyle + 1) % Enum.GetNames(typeof(PinStyle)).Length);
        }

        internal void TogglePinSize()
        {
            PinSize = (PinSize)(((int)PinSize + 1) % Enum.GetNames(typeof(PinSize)).Length);
        }

        internal void TogglePersistentOn()
        {
            PersistentOn = !PersistentOn;
        }
    }
}