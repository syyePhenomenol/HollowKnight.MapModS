using System.Linq;
using ConnectionMetadataInjector;
using ItemChanger;
using MapChanger.Defs;

namespace RandoMapMod.Pins
{
    internal static class InteropProperties
    {
        internal static readonly MetadataProperty<AbstractPlacement, ISprite> LocationPinSprite = new("PinSprite", GetDefaultLocationSprite);

        private static ISprite GetDefaultLocationSprite(AbstractPlacement placement)
        {
            return PinSprites.GetLocationSprite(SupplementalMetadata.OfPlacementAndLocations(placement).Get(InjectedProps.LocationPoolGroup));
        }

        internal static readonly MetadataProperty<AbstractItem, ISprite> ItemPinSprite = new("PinSprite", GetDefaultItemSprite);

        private static ISprite GetDefaultItemSprite(AbstractItem item)
        {
            return PinSprites.GetItemSprite(SupplementalMetadata.Of(item).Get(InjectedProps.ItemPoolGroup));
        }

        internal static readonly MetadataProperty<AbstractPlacement, string[]> HighlightScenes = new("HighlightScenes", (placement) => { return null; });

        internal static readonly MetadataProperty<AbstractPlacement, (string, float, float)[]> MapLocations = new("MapLocations", (placement) => { return GetDefaultMapLocations(placement.Name); });

        internal static (string, float, float)[] GetDefaultMapLocations(string name)
        {
            if (MapChanger.Finder.TryGetLocation(name, out MapLocationDef mld))
            {
                return mld.MapLocations.Select(mapLocation => ((string, float, float))mapLocation).ToArray();
            }

            RandoMapMod.Instance.LogDebug($"No MapLocationDef found for placement {name}");

            if (ItemChanger.Finder.GetLocation(name) is AbstractLocation al && al.sceneName is not null)
            {
                return new (string, float, float)[] { new MapLocation() { MappedScene = MapChanger.Finder.GetMappedScene(al.sceneName) } };
            }

            RandoMapMod.Instance.LogWarn($"No MapLocation found for {name}!");

            return new (string, float, float)[] { new MapLocation() { } };
        }
    }
}
