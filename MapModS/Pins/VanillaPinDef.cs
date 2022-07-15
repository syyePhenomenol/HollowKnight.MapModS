using ConnectionMetadataInjector.Util;
using MapChanger;
using MapChanger.Defs;
//using MapModS.Map;
using Newtonsoft.Json;
using RandomizerCore;
using System.Collections.Generic;
using UnityEngine;
using RD = RandomizerMod.RandomizerData;

namespace MapModS.Pins
{
    public class VanillaPinDef : RandomizerModPinDef
    {
        internal override HashSet<string> ItemPoolGroups => new() { LocationPoolGroup };

        [JsonConstructor]
        internal VanillaPinDef(string name) : base(Finder.GetLocation(name))
        {
            if (MapPosition is null)
            {
                MapChangerMod.Instance.LogWarn($"No valid MapPositionDef found for VanillaPinDef! {name}");
                return;
            }

            LocationPoolGroup = SubcategoryFinder.GetLocationPoolGroup(Name).FriendlyName();
            if (LocationPoolGroup == null)
            {
                LocationPoolGroup = PoolGroupNames.UNKNOWN;
            }
        }

        public override void Update()
        {
            Active = MapModS.LS.GetPoolGroupSetting(LocationPoolGroup) == Settings.PoolState.On || MapModS.LS.VanillaOn
                        && !Tracker.HasClearedLocation(Name);
            
            base.Update();
        }

        public override Sprite GetSprite()
        {
            return null;
            //return SpriteManager.GetSpriteFromPoolGroup(LocationPoolGroup);
        }

        public override Vector4 GetSpriteColor()
        {
            Vector3 color = Colors.GetColor(ColorSetting.Pin_Normal) * UNREACHABLE_COLOR_SCALE;
            return new(color.x, color.y, color.z, 1f);
        }

        public override Vector4 GetBorderColor()
        {
            //if (Selected)
            //{
            //    // do something
            //}

            Vector3 color = Colors.GetColor(ColorSetting.Pin_Normal) * UNREACHABLE_COLOR_SCALE;
            return new(color.x, color.y, color.z, 1f);
        }

        public override float GetScale()
        {
        //    if (Selected)
        //    {
        //        // do something
        //    }

            return UNREACHABLE_SIZE_SCALE * GetPinSizeScale();
        }

        internal override string[] GetPreviewText()
        {
            return null;
        }
    }
}
