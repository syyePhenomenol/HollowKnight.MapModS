using MapChanger;
using MapChanger.Defs;
using RandoMapMod.Settings;
using System.Collections.Generic;
using UnityEngine;
using RM = RandomizerMod.RandomizerMod;

namespace RandoMapMod.Defs
{
    public abstract class RMMPinDef : MapLocationDef
    {
        private protected const float REACHABLE_SIZE_SCALE = 1f;
        private protected const float UNREACHABLE_SIZE_SCALE = 0.7f;
        private protected const float UNREACHABLE_COLOR_SCALE = 0.5f;

        private const float SMALL_SCALE = 0.31f;
        private const float MEDIUM_SCALE = 0.37f;
        private const float LARGE_SCALE = 0.44f;

        public RMMPinDef(MapLocationDef mpd) : base(mpd)
        {

        }

        internal string LocationPoolGroup { get; private protected init; }
        internal abstract HashSet<string> ItemPoolGroups { get; }

        internal virtual string[] GetPreviewText()
        {
            return null;
        }

        internal string GetLogic()
        {
            if (RM.RS.TrackerData.lm.LogicLookup.ContainsKey(Name))
            {
                return RM.RS.TrackerData.lm.LogicLookup[Name].ToInfix();
            }
            return null;
        }

        private protected static float GetPinSizeScale()
        {
            return RandoMapMod.GS.PinSize switch
            {
                PinSize.Small => SMALL_SCALE,
                PinSize.Medium => MEDIUM_SCALE,
                PinSize.Large => LARGE_SCALE,
                _ => LARGE_SCALE
            };
        }

        public abstract Sprite GetSprite();

        public abstract Vector4 GetSpriteColor();

        public Sprite GetBorderSprite()
        {
            return SpriteManager.GetSprite("pinBorder");
        }

        public abstract Vector4 GetBorderColor();

        public abstract float GetScale();
    }
}
