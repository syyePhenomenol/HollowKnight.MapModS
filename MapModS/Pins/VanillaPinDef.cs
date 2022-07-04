using ConnectionMetadataInjector.Util;
using Newtonsoft.Json;
using MapModS.Map;
using RD = RandomizerMod.RandomizerData;
using UnityEngine;
using MapModS.Data;
using System.Collections.Generic;

namespace MapModS.Pins
{
    public class VanillaPinDef : RandomizerModPinDef
    {
        private readonly string objectName;
        private readonly string pdBool;
        private readonly string pdInt;
        private readonly int pdIntValue;

        internal override HashSet<string> ItemPoolGroups => new() { LocationPoolGroup };

        [JsonConstructor]
        internal VanillaPinDef(string name, string objectName, string pdBool, string pdInt, int pdIntValue, float offsetX, float offsetY) : base()
        {
            Name = name;
            OffsetX = offsetX;
            OffsetY = offsetY;

            if (!RD.Data.IsLocation(name)) return;

            this.objectName = objectName;
            this.pdBool = pdBool;
            this.pdInt = pdInt;
            this.pdIntValue = pdIntValue;

            Scene = RD.Data.GetLocationDef(name).SceneName;

            SetMapData();

            LocationPoolGroup = SubcategoryFinder.GetLocationPoolGroup(name).FriendlyName();
            if (LocationPoolGroup == null)
            {
                LocationPoolGroup = PoolGroupNames.UNKNOWN;
            }
        }

        public override void Update()
        {
            Active = MapModS.LS.GetPoolGroupSetting(LocationPoolGroup) == Settings.PoolState.On || MapModS.LS.VanillaOn
                && !(ItemTracker.HasObtainedItem(objectName, Scene)
                    || (pdBool != null && PlayerData.instance.GetBool(pdBool))
                    || (pdInt != null && PlayerData.instance.GetInt(pdInt) >= pdIntValue)
                    || (LocationPoolGroup == PoolGroupNames.WHISPERINGROOTS && PlayerData.instance.scenesEncounteredDreamPlantC.Contains(Scene))
                    || (LocationPoolGroup == PoolGroupNames.GRUBS && PlayerData.instance.scenesGrubRescued.Contains(Scene))
                    || (LocationPoolGroup == PoolGroupNames.GRIMMKINFLAMES && PlayerData.instance.scenesFlameCollected.Contains(Scene)));
            
            base.Update();
        }

        public override Sprite GetMainSprite()
        {
            return SpriteManager.GetSpriteFromPoolGroup(LocationPoolGroup);
        }

        public override Vector4 GetMainColor()
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

        public override Vector3 GetScale()
        {
        //    if (Selected)
        //    {
        //        // do something
        //    }

            return Vector3.one * UNREACHABLE_SIZE_SCALE * GetPinSizeScale();
        }

        internal override string[] GetPreviewText()
        {
            return null;
        }
    }
}
