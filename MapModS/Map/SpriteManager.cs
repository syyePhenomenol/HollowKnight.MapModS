using MapModS.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Code borrowed from homothety: https://github.com/homothetyhk/RandomizerMod/
namespace MapModS.Map
{
    internal static class SpriteManager
    {
        private static Dictionary<string, Sprite> _sprites;

        public static void LoadEmbeddedPngs(string prefix)
        {
            Assembly a = typeof(SpriteManager).Assembly;
            _sprites = new Dictionary<string, Sprite>();

            foreach (string name in a.GetManifestResourceNames().Where(name => name.Substring(name.Length - 3).ToLower() == "png"))
            {
                string altName = prefix != null ? name.Substring(prefix.Length) : name;
                altName = altName.Remove(altName.Length - 4);
                altName = altName.Replace(".", "");
                Sprite sprite = FromStream(a.GetManifestResourceStream(name));
                _sprites[altName] = sprite;
            }
        }

        public static Sprite GetSpriteFromPool(string pool, PinBorderColor color)
        {
            string spriteName = "undefined";

            if (MapModS.GS.pinStyle == PinStyle.Normal
                || color == PinBorderColor.Previewed
                || color == PinBorderColor.Persistent)
            {
                spriteName = pool switch
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
                    "Journal Entries" => "pinLore",
                    "Geo Rocks" => "pinRock",
                    "Boss Geo" => "pinGeo",
                    "Soul Totems" => "pinTotem",
                    "Lore Tablets" => "pinLore",
                    "Shops" => "pinShop",
                    "Levers" => "pinLever",
                    "Mr Mushroom" => "pinLore",
                    _ => "pinUnknown",
                };
            }
            else if (MapModS.GS.pinStyle == PinStyle.Q_Marks_1)
            {
                spriteName = pool switch
                {
                    "Shops" => "pinShop",
                    _ => "pinUnknown",
                };
            }
            else if (MapModS.GS.pinStyle == PinStyle.Q_Marks_2)
            {
                spriteName = pool switch
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
            else if (MapModS.GS.pinStyle == PinStyle.Q_Marks_3)
            {
                spriteName = pool switch
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

            switch (color)
            {
                case PinBorderColor.Previewed:
                    spriteName += "Green";
                    break;
                case PinBorderColor.OutOfLogic:
                    spriteName += "Red";
                    break;
                case PinBorderColor.Persistent:
                    spriteName += "Cyan";
                    break;
            }

            return GetSprite(spriteName);
        }

        public static Sprite GetSprite(string name)
        {
            if (_sprites.TryGetValue(name, out Sprite sprite))
            {
                return sprite;
            }

            MapModS.Instance.LogWarn("Failed to load sprite named '" + name + "'");

            return _sprites["pinUnknown"];
        }

        private static Sprite FromStream(Stream s)
        {
            Texture2D tex = new(1, 1);
            byte[] buffer = ToArray(s);
            _ = tex.LoadImage(buffer, markNonReadable: true);
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 55);
        }

        private static byte[] ToArray(Stream s)
        {
            using MemoryStream ms = new();
            s.CopyTo(ms);
            return ms.ToArray();
        }
    }
}