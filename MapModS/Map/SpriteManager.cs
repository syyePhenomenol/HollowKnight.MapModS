using MapModS.Data;
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

        public static Sprite GetSpriteFromPool(PoolGroup pool, PinBorderColor color)
        {
            string spriteName = "undefined";

            switch (MapModS.GS.pinStyle)
            {
                case PinStyle.Normal:
                    spriteName = pool switch
                    {
                        PoolGroup.Dreamers => "pinDreamer",
                        PoolGroup.Skills => "pinSkill",
                        PoolGroup.Charms => "pinCharm",
                        PoolGroup.Keys => "pinKey",
                        PoolGroup.MaskShards => "pinMask",
                        PoolGroup.VesselFragments => "pinVessel",
                        PoolGroup.CharmNotches => "pinNotch",
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
                        PoolGroup.Shop => "pinShop",
                        PoolGroup.Levers => "pinLever",
                        _ => "pinUnknown",
                    };
                    break;

                case PinStyle.Q_Marks_1:
                    spriteName = pool switch
                    {
                        PoolGroup.Shop => "pinShop",
                        _ => "pinUnknown",
                    };
                    break;

                case PinStyle.Q_Marks_2:
                    spriteName = pool switch
                    {
                        PoolGroup.Grubs => "pinUnknown_GrubInv",
                        PoolGroup.Mimics => "pinUnknown_GrubInv",
                        PoolGroup.LifebloodCocoons => "pinUnknown_LifebloodInv",
                        PoolGroup.GeoRocks => "pinUnknown_GeoRockInv",
                        PoolGroup.SoulTotems => "pinUnknown_TotemInv",
                        PoolGroup.Shop => "pinShop",
                        _ => "pinUnknown",
                    };
                    break;

                case PinStyle.Q_Marks_3:
                    spriteName = pool switch
                    {
                        PoolGroup.Grubs => "pinUnknown_Grub",
                        PoolGroup.Mimics => "pinUnknown_Grub",
                        PoolGroup.LifebloodCocoons => "pinUnknown_Lifeblood",
                        PoolGroup.GeoRocks => "pinUnknown_GeoRock",
                        PoolGroup.SoulTotems => "pinUnknown_Totem",
                        PoolGroup.Shop => "pinShop",
                        _ => "pinUnknown",
                    };
                    break;
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