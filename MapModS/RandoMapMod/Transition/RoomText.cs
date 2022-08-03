using System;
using MapChanger;
using MapChanger.MonoBehaviours;
using TMPro;
using UnityEngine;

namespace RandoMapMod.Transition
{
    internal class RoomText : MapObject, ISelectable
    {
        internal RoomTextDef Rtd { get; private set; }

        private TMP_FontAsset font;

        private TextMeshPro tmp;
        internal Vector4 Color
        {
            get => tmp.color;
            set
            {
                tmp.color = value;
            }
        }

        private bool selected = false;
        public bool Selected
        {
            get => selected;
            set
            {
                if (Selected != value)
                {
                    selected = value;
                    UpdateColor();
                }
            }
        }

        public bool CanSelect()
        {
            return gameObject.activeSelf;
        }

        public (string, Vector2) GetKeyAndPosition()
        {
            return (Rtd.Name, transform.position);
        }

        internal void Initialize(RoomTextDef rtd, TMP_FontAsset font)
        {
            Rtd = rtd;
            this.font = font;

            base.Initialize();

            ActiveModifiers.AddRange
            (
                new Func<bool>[]
                {
                    TransitionData.TransitionModeActive,
                    ActiveByMap,
                    GetRoomActive
                }
            );

            tmp = gameObject.AddComponent<TextMeshPro>();
            transform.localPosition = new Vector3(Rtd.X, Rtd.Y, 0f);
        }

        private void Start()
        {
            tmp.sortingLayerID = 629535577;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.font = font;
            tmp.fontSize = 2.4f;
            tmp.text = Rtd.Name;
        }

        private bool ActiveByMap()
        {
            return States.WorldMapOpen || (States.QuickMapOpen && States.CurrentMapZone == Rtd.MapZone);
        }

        private bool GetRoomActive()
        {
            return TransitionTracker.GetRoomActive(Rtd.Name);
        }

        private void OnEnable()
        {
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (Selected)
            {
                Color = RmmColors.GetColor(RmmColorSetting.Room_Selected);
            }
            else
            {
                Color = TransitionTracker.GetRoomColor(Rtd.Name);
            }
        }
    }
}
