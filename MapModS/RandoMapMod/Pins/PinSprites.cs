using ItemChanger;
using RandoMapMod.Settings;

namespace RandoMapMod.Pins
{
    internal static class PinSprites
    {
        public static ISprite GetLocationSprite(string poolGroup)
        {
            string spriteName = "Unknown";

            if (RandoMapMod.GS.PinStyle == PinStyle.Normal)
            {
                return GetNormalSprite(poolGroup);
            }
            else if (RandoMapMod.GS.PinStyle == PinStyle.Q_Marks_1)
            {
                spriteName = poolGroup switch
                {
                    "Shops" => "Shop",
                    _ => "Unknown",
                };
            }
            else if (RandoMapMod.GS.PinStyle == PinStyle.Q_Marks_2)
            {
                spriteName = poolGroup switch
                {
                    "Grubs" => "UnknownGrubInv",
                    "Mimics" => "UnknownGrubInv",
                    "Lifeblood Cocoons" => "UnknownLifebloodInv",
                    "Geo Rocks" => "UnknownGeoRockInv",
                    "Soul Totems" => "UnknownTotemInv",
                    "Shops" => "Shop",
                    _ => "Unknown",
                };
            }
            else if (RandoMapMod.GS.PinStyle == PinStyle.Q_Marks_3)
            {
                spriteName = poolGroup switch
                {
                    "Grubs" => "UnknownGrub",
                    "Mimics" => "UnknownGrub",
                    "Lifeblood Cocoons" => "UnknownLifeblood",
                    "Geo Rocks" => "UnknownGeoRock",
                    "Soul Totems" => "UnknownTotem",
                    "Shops" => "Shop",
                    _ => "Unknown",
                };
            }

            return new EmbeddedSprite($"Pins.{spriteName}");
        }

        public static ISprite GetItemSprite(string poolGroup)
        {
            return GetNormalSprite(poolGroup);
        }

        private static ISprite GetNormalSprite(string poolGroup)
        {
            string spriteName = poolGroup switch
            {
                "Dreamers" => "Dreamer",
                "Skills" => "Skill",
                "Charms" => "Charm",
                "Keys" => "Key",
                "Mask Shards" => "Mask",
                "Vessel Fragments" => "Vessel",
                "Charm Notches" => "Notch",
                "Pale Ore" => "Ore",
                "Geo Chests" => "Geo",
                "Rancid Eggs" => "Egg",
                "Relics" => "Relic",
                "Whispering Roots" => "Root",
                "Boss Essence" => "EssenceBoss",
                "Grubs" => "Grub",
                "Mimics" => "Grub",
                "Maps" => "Map",
                "Stags" => "Stag",
                "Lifeblood Cocoons" => "Cocoon",
                "Grimmkin Flames" => "Flame",
                "Journal Entries" => "Journal",
                "Geo Rocks" => "Rock",
                "Boss Geo" => "Geo",
                "Soul Totems" => "Totem",
                "Lore Tablets" => "Lore",
                "Shops" => "Shop",
                "Levers" => "Lever",
                "Mr Mushroom" => "Lore",
                "Benches" => "Bench",
                _ => "Unknown",
            };

            return new EmbeddedSprite($"Pins.{spriteName}");
        }
    }
}
