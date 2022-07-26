using MapChanger;
using RandoMapMod.Settings;
using UnityEngine;

namespace RandoMapMod.Defs
{
    internal static class PinSprites
    {
        public static Sprite GetSpriteFromPoolGroup(string poolGroup, bool normalOverride = false)
        {
            string spriteName = "undefined";

            if (RandoMapMod.GS.PinStyle == PinStyle.Normal || normalOverride)
            {
                spriteName = poolGroup switch
                {
                    "Dreamers" => "pinDreamer",
                    "Skills" => "pinSkill",
                    "Charms" => "pinCharm",
                    "Keys" => "pinKey",
                    "Mask Shards" => "pinMask",
                    "Vessel Fragments" => "pinVessel",
                    "Charm Notches" => "pinNotch",
                    "Pale Ore" => "pinOre",
                    "Geo Chests" => "pinGeo",
                    "Rancid Eggs" => "pinEgg",
                    "Relics" => "pinRelic",
                    "Whispering Roots" => "pinRoot",
                    "Boss Essence" => "pinEssenceBoss",
                    "Grubs" => "pinGrub",
                    "Mimics" => "pinGrub",
                    "Maps" => "pinMap",
                    "Stags" => "pinStag",
                    "Lifeblood Cocoons" => "pinCocoon",
                    "Grimmkin Flames" => "pinFlame",
                    "Journal Entries" => "pinJournal",
                    "Geo Rocks" => "pinRock",
                    "Boss Geo" => "pinGeo",
                    "Soul Totems" => "pinTotem",
                    "Lore Tablets" => "pinLore",
                    "Shops" => "pinShop",
                    "Levers" => "pinLever",
                    "Mr Mushroom" => "pinLore",
                    "Benches" => "pinBench",
                    _ => "pinUnknown",
                };
            }
            else if (RandoMapMod.GS.PinStyle == PinStyle.Q_Marks_1)
            {
                spriteName = poolGroup switch
                {
                    "Shops" => "pinShop",
                    _ => "pinUnknown",
                };
            }
            else if (RandoMapMod.GS.PinStyle == PinStyle.Q_Marks_2)
            {
                spriteName = poolGroup switch
                {
                    "Grubs" => "pinUnknown_GrubInv",
                    "Mimics" => "pinUnknown_GrubInv",
                    "Lifeblood Cocoons" => "pinUnknown_LifebloodInv",
                    "Geo Rocks" => "pinUnknown_GeoRockInv",
                    "Soul Totems" => "pinUnknown_TotemInv",
                    "Shops" => "pinShop",
                    _ => "pinUnknown",
                };
            }
            else if (RandoMapMod.GS.PinStyle == PinStyle.Q_Marks_3)
            {
                spriteName = poolGroup switch
                {
                    "Grubs" => "pinUnknown_Grub",
                    "Mimics" => "pinUnknown_Grub",
                    "Lifeblood Cocoons" => "pinUnknown_Lifeblood",
                    "Geo Rocks" => "pinUnknown_GeoRock",
                    "Soul Totems" => "pinUnknown_Totem",
                    "Shops" => "pinShop",
                    _ => "pinUnknown",
                };
            }

            return SpriteManager.GetSprite(spriteName);
        }
    }
}
