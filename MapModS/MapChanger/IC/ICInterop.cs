using ItemChanger;

namespace MapChanger.IC
{
    internal static class ICInterop
    {
        internal static string GetScene(string locationName)
        {
            if (ItemChanger.Finder.GetLocation(locationName) is AbstractLocation al)
            {
                return al.name;
            }
            return default;
        }
    }
}
