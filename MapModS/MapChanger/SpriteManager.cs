using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Code borrowed from homothety: https://github.com/homothetyhk/RandomizerMod/
namespace MapChanger
{
    internal static class SpriteManager
    {
        private static Dictionary<string, Sprite> _sprites;

        public static void Load()
        {
            Assembly a = typeof(SpriteManager).Assembly;
            _sprites = new Dictionary<string, Sprite>();

            foreach (string name in a.GetManifestResourceNames().Where(name => name.Substring(name.Length - 3).ToLower() == "png"))
            {
                Sprite sprite = FromStream(a.GetManifestResourceStream(name));

                MapChangerMod.Instance.LogDebug(name);

                string altName = name.Remove(name.Length - 4).Split('.').Last();
                _sprites[altName] = sprite;
            }
        }

        public static void LoadPinSprites()
        {
            string prefix = "MapModS.Resources.Pins";

            Assembly a = typeof(SpriteManager).Assembly;
            _sprites = new Dictionary<string, Sprite>();

            foreach (string name in a.GetManifestResourceNames().Where(name => name.Substring(name.Length - 3).ToLower() == "png"))
            {
                Sprite sprite = FromStream(a.GetManifestResourceStream(name));

                string altName = name.Substring(prefix.Length);
                altName = altName.Remove(altName.Length - 4);
                altName = altName.Replace(".", "");
                _sprites[altName] = sprite;
            }

            // Load custom pins
            prefix = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Pins");

            if (Directory.Exists(prefix))
            {
                foreach (string name in Directory.GetFiles(prefix).Where(name => name.Substring(name.Length - 3).ToLower() == "png"))
                {
                    Sprite sprite = FromStream(File.Open(name, FileMode.Open));

                    string altName = name.Substring(prefix.Length);
                    altName = altName.Remove(altName.Length - 4);
                    altName = altName.Replace("\\", "");

                    if (_sprites.ContainsKey(altName))
                    {
                        _sprites[altName] = sprite;
                    }
                }

                MapChangerMod.Instance.Log("Custom pin sprites loaded");
            }
        }

        public static Sprite GetSpriteFromPoolGroup(string poolGroup)
        {
            string spriteName = poolGroup switch
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
                "SoulTotems" => "pinTotem",
                "Soul Totems" => "pinTotem",
                "LoreTablets" => "pinLore",
                "Lore Tablets" => "pinLore",
                "Shops" => "pinShop",
                "Levers" => "pinLever",
                "Mr Mushroom" => "pinLore",
                "Benches" => "pinBench",
                _ => "pinUnknown",
            };

            return GetSprite(spriteName);
        }

        //public static Sprite GetSpriteFromPoolGroup(string poolGroup, bool normalOverride = false)
        //{
        //    string spriteName = "undefined";

        //    if (MapModS.GS.PinStyle == PinStyle.Normal || normalOverride)
        //    {
        //        spriteName = poolGroup switch
        //        {
        //            "Dreamers" => "pinDreamer",
        //            "Skills" => "pinSkill",
        //            "Charms" => "pinCharm",
        //            "Keys" => "pinKey",
        //            "Mask Shards" => "pinMask",
        //            "Vessel Fragments" => "pinVessel",
        //            "Charm Notches" => "pinNotch",
        //            "Pale Ore" => "pinOre",
        //            "Geo Chests" => "pinGeo",
        //            "Rancid Eggs" => "pinEgg",
        //            "Relics" => "pinRelic",
        //            "Whispering Roots" => "pinRoot",
        //            "Boss Essence" => "pinEssenceBoss",
        //            "Grubs" => "pinGrub",
        //            "Mimics" => "pinGrub",
        //            "Maps" => "pinMap",
        //            "Stags" => "pinStag",
        //            "Lifeblood Cocoons" => "pinCocoon",
        //            "Grimmkin Flames" => "pinFlame",
        //            "Journal Entries" => "pinJournal",
        //            "Geo Rocks" => "pinRock",
        //            "Boss Geo" => "pinGeo",
        //            "Soul Totems" => "pinTotem",
        //            "Lore Tablets" => "pinLore",
        //            "Shops" => "pinShop",
        //            "Levers" => "pinLever",
        //            "Mr Mushroom" => "pinLore",
        //            "Benches" => "pinBench",
        //            _ => "pinUnknown",
        //        };
        //    }
        //    else if (MapModS.GS.PinStyle == PinStyle.Q_Marks_1)
        //    {
        //        spriteName = poolGroup switch
        //        {
        //            "Shops" => "pinShop",
        //            _ => "pinUnknown",
        //        };
        //    }
        //    else if (MapModS.GS.PinStyle == PinStyle.Q_Marks_2)
        //    {
        //        spriteName = poolGroup switch
        //        {
        //            "Grubs" => "pinUnknown_GrubInv",
        //            "Mimics" => "pinUnknown_GrubInv",
        //            "Lifeblood Cocoons" => "pinUnknown_LifebloodInv",
        //            "Geo Rocks" => "pinUnknown_GeoRockInv",
        //            "Soul Totems" => "pinUnknown_TotemInv",
        //            "Shops" => "pinShop",
        //            _ => "pinUnknown",
        //        };
        //    }
        //    else if (MapModS.GS.PinStyle == PinStyle.Q_Marks_3)
        //    {
        //        spriteName = poolGroup switch
        //        {
        //            "Grubs" => "pinUnknown_Grub",
        //            "Mimics" => "pinUnknown_Grub",
        //            "Lifeblood Cocoons" => "pinUnknown_Lifeblood",
        //            "Geo Rocks" => "pinUnknown_GeoRock",
        //            "Soul Totems" => "pinUnknown_Totem",
        //            "Shops" => "pinShop",
        //            _ => "pinUnknown",
        //        };
        //    }

        //    return GetSprite(spriteName);
        //}

        public static Sprite GetSprite(string name)
        {
            if (_sprites.TryGetValue(name, out Sprite sprite))
            {
                return sprite;
            }

            MapChangerMod.Instance.LogWarn("Failed to load sprite named '" + name + "'");

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