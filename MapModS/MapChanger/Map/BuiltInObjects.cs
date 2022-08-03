﻿using System;
using System.Collections.Generic;
using System.Linq;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using TMPro;
using UnityEngine;

namespace MapChanger.Map
{
    public static class BuiltInObjects
    {
        private static Dictionary<string, RoomSpriteDef> roomSpriteDefs;
        private static Dictionary<string, MiscObjectDef> miscObjectDefs;
        public static Dictionary<string, RoomSprite> MappedRooms { get; private set; }

        internal static void Load()
        {
            roomSpriteDefs = JsonUtil.Deserialize<Dictionary<string, RoomSpriteDef>>("MapModS.MapChanger.Resources.roomSprites.json");
            miscObjectDefs = JsonUtil.Deserialize<Dictionary<string, MiscObjectDef>>("MapModS.MapChanger.Resources.miscObjects.json");
            Events.AfterSetGameMap += Make;
        }

        private static void Make(GameObject goMap)
        {
            MapChangerMod.Instance.LogDebug("AttachMapModifiers");

            MappedRooms = new();

            foreach (Transform t0 in goMap.transform)
            {
                if (t0.name.Contains("WHITE_PALACE") || t0.name.Contains("GODS_GLORY")) continue;

                foreach (Transform t1 in t0.transform)
                {
                    foreach (Transform t2 in t1.transform)
                    {
                        if (t2.name == "pin_blue_health")
                        {
                            LifebloodPin lifebloodPin = t2.gameObject.AddComponent<LifebloodPin>();
                            lifebloodPin.Initialize();
                        }
                        else if (t2.name == "pin_dream_tree")
                        {
                            WhisperingRootPin whisperingRootPin = t2.gameObject.AddComponent<WhisperingRootPin>();
                            whisperingRootPin.Initialize();
                        }
                    }
                }
            }

            foreach ((string pathName, RoomSpriteDef rsd) in roomSpriteDefs.Select(kvp => (kvp.Key, kvp.Value)))
            {
                Transform child = goMap.transform.FindChildInHierarchy(pathName);
                if (child is null)
                {
                    MapChangerMod.Instance.LogDebug($"Transform not found: {pathName}");
                    continue;
                }

                child.gameObject.SetActive(false);
                RoomSprite roomSprite = child.gameObject.AddComponent<RoomSprite>();
                roomSprite.Initialize(rsd);

                MappedRooms[roomSprite.name] = roomSprite;

                MapChangerMod.Instance.LogDebug($"Attaching: {pathName}");
            }

            // Disable extra map arrow
            goMap.transform.FindChildInHierarchy("Deepnest/Fungus2_25/Next Area (3)")?.gameObject.SetActive(false);

            foreach ((string pathName, MiscObjectDef mod) in miscObjectDefs.Select(kvp => (kvp.Key, kvp.Value)))
            {
                Transform child;
                if (pathName.Contains("*"))
                {
                    child = goMap.transform.FindChildInHierarchy(pathName.Split('*')[0]).GetChild(1);
                }
                else
                {
                    child = goMap.transform.FindChildInHierarchy(pathName);
                }

                if (child is null)
                {
                    MapChangerMod.Instance.LogWarn($"Transform not found: {pathName}");
                    continue;
                }

                child.gameObject.SetActive(false);

                if (mod.Type == MiscObjectType.NextArea)
                {
                    NextArea nextArea = child.gameObject.AddComponent<NextArea>();
                    nextArea.Initialize(mod);
                }
                else if (mod.Type == MiscObjectType.AreaName)
                {
                    AreaName areaName = child.gameObject.AddComponent<AreaName>();
                    areaName.Initialize(mod);
                }

                MapChangerMod.Instance.LogDebug($"Attaching: {pathName}");
            }

            GameObject goQmt = GameCameras.instance.hudCamera.transform.FindChildInHierarchy("Quick Map/Area Name").gameObject;
            goQmt.SetActive(false);
            QuickMapTitle qmt = goQmt.AddComponent<QuickMapTitle>();
            qmt.Initialize();

            MapChangerMod.Instance.LogDebug("~AttachMapModifiers");
        }

        internal static bool TryGetMapRoomPosition(string scene, out float x, out float y)
        {
            (x, y) = (0f, 0f);

            if (scene is null) return false;

            if (MappedRooms.TryGetValue(scene, out RoomSprite roomSprite))
            {
                Vector2 position = roomSprite.transform.parent.localPosition + roomSprite.transform.localPosition;
                (x, y) = (position.x, position.y);
                return true;
            }

            MapChangerMod.Instance.LogDebug($"Map room not recognized: {scene}");
            return false;
        }
    }
}
