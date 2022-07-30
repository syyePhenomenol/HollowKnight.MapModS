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
        private static Dictionary<string, Sprite> sprites;

        public static void Load()
        {
            Assembly a = typeof(SpriteManager).Assembly;
            sprites = new Dictionary<string, Sprite>();

            foreach (string name in a.GetManifestResourceNames().Where(name => name.Substring(name.Length - 3).ToLower() == "png"))
            {
                Sprite sprite = FromStream(a.GetManifestResourceStream(name));

                MapChangerMod.Instance.LogDebug(name);

                string altName = name.Remove(name.Length - 4).Split('.').Last();
                sprites[altName] = sprite;
            }
        }

        //public static void LoadPinSprites()
        //{
        //    string prefix = "MapModS.Resources.Pins";

        //    Assembly a = typeof(SpriteManager).Assembly;
        //    sprites = new Dictionary<string, Sprite>();

        //    foreach (string name in a.GetManifestResourceNames().Where(name => name.Substring(name.Length - 3).ToLower() == "png"))
        //    {
        //        Sprite sprite = FromStream(a.GetManifestResourceStream(name));

        //        string altName = name.Substring(prefix.Length);
        //        altName = altName.Remove(altName.Length - 4);
        //        altName = altName.Replace(".", "");
        //        sprites[altName] = sprite;
        //    }

        //    // Load custom pins
        //    prefix = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Pins");

        //    if (Directory.Exists(prefix))
        //    {
        //        foreach (string name in Directory.GetFiles(prefix).Where(name => name.Substring(name.Length - 3).ToLower() == "png"))
        //        {
        //            Sprite sprite = FromStream(File.Open(name, FileMode.Open));

        //            string altName = name.Substring(prefix.Length);
        //            altName = altName.Remove(altName.Length - 4);
        //            altName = altName.Replace("\\", "");

        //            if (sprites.ContainsKey(altName))
        //            {
        //                sprites[altName] = sprite;
        //            }
        //        }

        //        MapChangerMod.Instance.Log("Custom pin sprites loaded");
        //    }
        //}

        public static Sprite GetSpriteFromPoolGroup(string poolGroup)
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
                "SoulTotems" => "Totem",
                "Soul Totems" => "Totem",
                "LoreTablets" => "Lore",
                "Lore Tablets" => "Lore",
                "Shops" => "Shop",
                "Levers" => "Lever",
                "Mr Mushroom" => "Lore",
                "Benches" => "Bench",
                _ => "Unknown",
            };

            return GetSprite(spriteName);
        }

        public static Sprite GetSprite(string name)
        {
            if (sprites.TryGetValue(name, out Sprite sprite))
            {
                return sprite;
            }

            MapChangerMod.Instance.LogWarn("Failed to load sprite named '" + name + "'");

            return null;
        }

        private static Sprite FromStream(Stream s)
        {
            Texture2D tex = new(1, 1, TextureFormat.RGBA32, mipChain: true);
            byte[] buffer = ToArray(s);
            tex.LoadImage(buffer, markNonReadable: true);
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        }

        private static byte[] ToArray(Stream s)
        {
            using MemoryStream ms = new();
            s.CopyTo(ms);
            return ms.ToArray();
        }
    }
}