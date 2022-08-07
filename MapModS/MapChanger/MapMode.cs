﻿using System;
using GlobalEnums;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace MapChanger
{
    public enum OverrideType
    {
        Neutral,
        ForceOff,
        ForceOn
    }

    public class MapMode
    {
        public (string, string) ModeKey => (Mod, ModeName);

        public virtual string Mod => "MapChangerMod";
        public virtual string ModeName => "Disabled";
        public virtual bool ForceHasMap => false;
        public virtual bool ForceHasQuill => false;
        public virtual OverrideType VanillaPins => OverrideType.Neutral;
        public virtual OverrideType MapMarkers => OverrideType.Neutral;
        /// <summary>
        /// Determines if the map immediately gets filled in when visiting a new scene with Quill.
        /// </summary>
        public virtual bool ImmediateMapUpdate => false;
        /// <summary>
        /// Forces all map areas/rooms to be visible and filled in.
        /// </summary>
        public virtual bool FullMap => false;
        public virtual bool DisableAreaNames => false;
        public virtual bool DisableNextArea => false;

        public virtual bool? RoomActiveOverride(RoomSprite roomSprite) => null;
        public virtual bool? RoomCanSelectOverride(RoomSprite roomSprite) => null;
        public virtual Vector4? RoomColorOverride(RoomSprite roomSprite) => null;
        public virtual Vector4? AreaNameColorOverride(AreaName areaName) => null;
        public virtual Vector4? NextAreaColorOverride(NextArea nextArea) => null;
        public virtual Vector4? QuickMapTitleColorOverride(QuickMapTitle qmt) => null;
    }
}
