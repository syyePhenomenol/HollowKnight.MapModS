using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using MapModS.Data;

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

                MapModS.Instance.Log(altName);
            }
        }
        // TODO: Add switch for pin style
        public static Sprite GetSpriteFromPool(PoolGroup pool)
        {
            string spriteName = pool switch
            {
                PoolGroup.Dreamers => "pinDreamer",
                PoolGroup.Skills => "pinSkill",
                PoolGroup.Charms => "pinCharm",
                PoolGroup.Keys => "pinKey",
                PoolGroup.MaskShards => "pinMask",
                PoolGroup.VesselFragments => "pinVessel",
                PoolGroup.CharmNotches => "pinCharm",
                PoolGroup.PaleOre => "pinOre",
                PoolGroup.GeoChests => "pinGeo",
                PoolGroup.RancidEggs => "pinEgg",
                PoolGroup.Relics => "pinRelic",
                PoolGroup.WhisperingRoots => "pinRoot",
                PoolGroup.BossEssence => "pinEssenceBoss",
                PoolGroup.Grubs => "pinGrub",
                PoolGroup.Mimics => "pinGrub",
                PoolGroup.Maps => "pinMap",
                PoolGroup.Stags => "pinStag",
                PoolGroup.LifebloodCocoons => "pinCocoon",
                PoolGroup.GrimmkinFlames => "pinFlame",
                PoolGroup.JournalEntries => "pinLore",
                PoolGroup.GeoRocks => "pinRock",
                PoolGroup.BossGeo => "pinGeo",
                PoolGroup.SoulTotems => "pinTotem",
                PoolGroup.LoreTablets => "pinLore",
                _ => "pinUnknown",
            };

            return GetSprite(spriteName);
        }

        public static Sprite GetSprite(string name)
        {
            if (_sprites.TryGetValue(name, out Sprite sprite))
            {
                return sprite;
            }

            MapModS.Instance.LogWarn("Failed to load sprite named '" + name + "'");
            return null;
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