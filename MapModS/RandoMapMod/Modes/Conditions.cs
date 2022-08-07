namespace RandoMapMod.Modes
{
    internal static class Conditions
    {
        internal static bool RandoMapModEnabled()
        {
            return MapChanger.Settings.MapModEnabled && MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(RmmMapMode));
        }

        internal static bool NormalModeEnabled()
        {
            return MapChanger.Settings.MapModEnabled && MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(NormalMode));
        }

        internal static bool TransitionModeEnabled()
        {
            return MapChanger.Settings.MapModEnabled && MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(TransitionMode));
        }
    }
}
