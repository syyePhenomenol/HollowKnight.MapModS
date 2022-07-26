using System.Collections.Generic;
using ConnectionMetadataInjector.Util;
using MapChanger;
using Newtonsoft.Json;
using UnityEngine;

namespace RandoMapMod.Defs
{
    public class VanillaPinDef : RMMPinDef
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
                LocationPoolGroup = PoolGroup.Other.FriendlyName();
            }
        }

        public override void Update()
        {
            Active = RandoMapMod.LS.GetPoolGroupSetting(LocationPoolGroup) == Settings.PoolState.On || RandoMapMod.LS.VanillaOn
                        && !Tracker.HasClearedLocation(Name);
            
            base.Update();
        }

        public override Sprite GetSprite()
        {
            return PinSprites.GetSpriteFromPoolGroup(LocationPoolGroup);
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
