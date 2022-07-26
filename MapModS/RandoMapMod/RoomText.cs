using System;
using GlobalEnums;
using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class RoomText : MapObject, ITextMeshPro, IMapRoom, ISelectable
    {        
        public TextMeshPro Tmp => GetComponent<TextMeshPro>();
        public string SceneName => mld.SceneName;
        public MapZone MapZone => mld.MapZone;

        private MapLocationDef mld;
        public bool Selected { get; private set; } = false;

        public bool Visible => gameObject.activeSelf;

        public bool CanUsePosition()
        {
            return gameObject.activeSelf;
        }

        public void Select()
        {
            MapChangerMod.Instance.LogDebug($"Currently selected: {transform.name}");
            Selected = true;
            //Set();
        }

        public void Deselect()
        {
            MapChangerMod.Instance.LogDebug($"Currently deselected: {transform.name}");
            Selected = false;
            //Set();
        }

        public (string, Vector2) GetKeyAndPosition()
        {
            return (SceneName, transform.position);
        }

        internal void Initialize(MapLocationDef mld)
        {
            base.Initialize();

            ActiveModifiers.Add(IsActive);

            this.mld = mld;
            name = mld.Name;
            gameObject.layer = UI_LAYER;
            Tmp.text = mld.SceneName;

            transform.localPosition = new Vector3(mld.X, mld.Y, 0f);

            SetText();
            SetTextColor();
        }

        private bool IsActive()
        {
            return Settings.MapModEnabled
                && (States.WorldMapOpen
                    || (States.QuickMapOpen && States.CurrentMapZone == MapZone))
                && Settings.CurrentMode().EnableExtraRoomNames;
        }

        //public override void Set()
        //{
        //    try
        //    {
        //        if (Settings.MapModEnabled && Settings.CurrentMode().OnRoomTextSet is not null)
        //        {
        //            if (Settings.CurrentMode().OnRoomTextSet.Invoke(this)) return;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MapChangerMod.Instance.LogError(e);
        //    }

        //    gameObject.SetActive(
        //        Settings.MapModEnabled
        //        && (States.WorldMapOpen
        //            || (States.QuickMapOpen && States.CurrentMapZone == MapZone))
        //        && Settings.CurrentMode().EnableExtraRoomNames);

        //    SetText();
        //    SetTextColor();

        //    mld.OffsetX = transform.localPosition.x;
        //    mld.OffsetY = transform.localPosition.y;
        //}

        public void SetText()
        {

        }

        public void SetTextColor()
        {
            if (Selected)
            {
                Tmp.color = Colors.GetColor(ColorSetting.Room_Selected);
            }
            else
            {
                Tmp.color = Colors.GetColor(ColorSetting.Room_Normal);
            }
        }
    }
}
