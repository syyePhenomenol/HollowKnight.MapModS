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

    [Serializable]
    public class GlobalSettings
    {
        public bool allowBenchWarpSearch = true;

        public bool uncheckedPanelActive = false;

        public bool routeCompassEnabled = true;

        public bool controlPanelOn = true;

        public bool mapKeyOn = false;

        public bool lookupOn = false;

        public RouteTextInGame routeTextInGame = RouteTextInGame.ShowNextTransitionOnly;

        public PinStyle pinStyle = PinStyle.Normal;

        public PinSize pinSize = PinSize.Medium;

        public bool persistentOn = false;

        public void ToggleAllowBenchWarp()
        {
            allowBenchWarpSearch = !allowBenchWarpSearch;
        }

        public void ToggleUncheckedPanel()
        {
            uncheckedPanelActive = !uncheckedPanelActive;
        }

        public void ToggleRouteCompassEnabled()
        {
            routeCompassEnabled = !routeCompassEnabled;
        }
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