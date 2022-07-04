using MapModS.Map;
using MapModS.Settings;
using System.Collections.Generic;
using UnityEngine;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Pins
{
    public abstract class RandomizerModPinDef : AbstractPinDef
    {
        private protected const float REACHABLE_SIZE_SCALE = 1f;
        private protected const float UNREACHABLE_SIZE_SCALE = 0.7f;
        private protected const float UNREACHABLE_COLOR_SCALE = 0.5f;

        private const float SMALL_SCALE = 0.31f;
        private const float MEDIUM_SCALE = 0.37f;
        private const float LARGE_SCALE = 0.44f;

        internal string LocationPoolGroup { get; private protected init; }
        internal abstract HashSet<string> ItemPoolGroups { get; }

        private protected RandomizerModPinDef()
        {
            BorderPlacement = BorderPlacement.InFront;
        }

        public override void Update()
        {
            Active = Active
                && ((MapModS.QuickMapOpen && MapModS.CurrentMapZone == MapZone)
                    || (MapModS.WorldMapOpen && (MapModS.LS.Mode != MapMode.PinsOverMap || Utils.HasMapSetting(MapZone))));
            //TODO: Transition mode settings here
        }

        public override Sprite GetBorderSprite()
        {
            return SpriteManager.GetSprite("pinBorder");
        }

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
            return MapModS.GS.PinSize switch
            {
                PinSize.Small => SMALL_SCALE,
                PinSize.Medium => MEDIUM_SCALE,
                PinSize.Large => LARGE_SCALE,
                _ => LARGE_SCALE
            };
        }

        public override Sprite GetMainSprite()
        {
            return null;
        }

        public override Vector4 GetMainColor()
        {
            return Vector4.negativeInfinity;
        }

        public override Vector4 GetBorderColor()
        {
            return Vector4.negativeInfinity;
        }

        public override Vector3 GetScale()
        {
            return Vector3.negativeInfinity;
        }
    }
}
