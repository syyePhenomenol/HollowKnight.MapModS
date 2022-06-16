using System;

namespace MapModS.Settings
{
    public enum PinSize
    {
        Small,
        Medium,
        Large
    }

    public enum PinStyle
    {
        Normal,
        Q_Marks_1,
        Q_Marks_2,
        Q_Marks_3
    }

    public enum RouteTextInGame
    {
        Hide,
        Show,
        ShowNextTransitionOnly
    }

    public enum OffRouteBehaviour
    {
        Keep,
        Cancel,
        Reevaluate
    }

    [Serializable]
    public class GlobalSettings
    {
        public bool controlPanelOn = true;

        public bool mapKeyOn = false;

        public bool lookupOn = false;

        public bool benchwarpWorldMap = true;

        public bool allowBenchWarpSearch = true;

        public bool uncheckedPanelActive = true;

        public RouteTextInGame routeTextInGame = RouteTextInGame.ShowNextTransitionOnly;

        public OffRouteBehaviour whenOffRoute = OffRouteBehaviour.Reevaluate;

        public bool routeCompassEnabled = true;

        public PinStyle pinStyle = PinStyle.Normal;

        public PinSize pinSize = PinSize.Medium;

        public bool persistentOn = false;

        public void ToggleControlPanel()
        {
            controlPanelOn = !controlPanelOn;
        }

        public void ToggleMapKey()
        {
            mapKeyOn = !mapKeyOn;
        }

        public void ToggleLookup()
        {
            lookupOn = !lookupOn;
        }

        public void ToggleBenchwarpWorldMap()
        {
            benchwarpWorldMap = !benchwarpWorldMap;
        }

        public void ToggleAllowBenchWarp()
        {
            allowBenchWarpSearch = !allowBenchWarpSearch;
        }

        public void ToggleUncheckedPanel()
        {
            uncheckedPanelActive = !uncheckedPanelActive;
        }

        public void ToggleRouteTextInGame()
        {
            switch (routeTextInGame)
            {
                case RouteTextInGame.Hide:
                case RouteTextInGame.Show:
                    routeTextInGame += 1;
                    break;
                default:
                    routeTextInGame = RouteTextInGame.Hide;
                    break;
            }
        }

        public void ToggleWhenOffRoute()
        {
            switch (whenOffRoute)
            {
                case OffRouteBehaviour.Keep:
                case OffRouteBehaviour.Cancel:
                    whenOffRoute += 1;
                    break;
                default:
                    whenOffRoute = OffRouteBehaviour.Keep;
                    break;
            }
        }

        public void ToggleRouteCompassEnabled()
        {
            routeCompassEnabled = !routeCompassEnabled;
        }

        public void TogglePinStyle()
        {
            switch (pinStyle)
            {
                case PinStyle.Normal:
                case PinStyle.Q_Marks_1:
                case PinStyle.Q_Marks_2:
                    pinStyle += 1;
                    break;
                default:
                    pinStyle = PinStyle.Normal;
                    break;
            }
        }

        public void TogglePinSize()
        {
            switch (pinSize)
            {
                case PinSize.Small:
                case PinSize.Medium:
                    pinSize += 1;
                    break;
                default:
                    pinSize = PinSize.Small;
                    break;
            }
        }

        public void TogglePersistentOn()
        {
            persistentOn = !persistentOn;
        }
    }
}