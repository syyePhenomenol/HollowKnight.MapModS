using GlobalEnums;
using MapModS.Data;
using MapModS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PGS = MapModS.Settings.PoolGroupState;
using PLS = MapModS.Data.PinLocationState;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Map
{
    public class PinsCustom : MonoBehaviour
    {
        private readonly List<PinAnimatedSprite> _pins = new();

        public void MakePins(GameMap gameMap)
        {
            DestroyPins();

            PinDef[] sortedPins = MainData.GetUsedPinArray();

            for (int i = 0; i < sortedPins.Count(); i++)
            {
                float offsetZ = (float)i / sortedPins.Count();
                try
                {
                    MakePin(sortedPins[i], offsetZ, gameMap);
                }
                catch (Exception e)
                {
                    MapModS.Instance.LogError(e);
                }
            }

            GetRandomizedOthersGroups();
        }

        private void MakePin(PinDef pinDef, float offsetZ, GameMap gameMap)
        {
            // Create new pin
            GameObject goPin = new($"pin_mapmod_{pinDef.name}")
            {
                layer = 30
            };

            SpriteRenderer sr = goPin.AddComponent<SpriteRenderer>();

            // Initialize sprite to vanillaPool
            sr.sprite = SpriteManager.GetSpriteFromPool(pinDef.locationPoolGroup, false);
            sr.sortingLayerName = "HUD";
            sr.size = new Vector2(1f, 1f);

            // Create separate colorizable border
            GameObject goPinBorder = new($"pin_mapmod_{pinDef.name}_border")
            {
                layer = 30
            };

            sr = goPinBorder.AddComponent<SpriteRenderer>();

            sr.sprite = SpriteManager.GetSprite("pinBorder");
            sr.sortingLayerName = "HUD";
            sr.size = new Vector2(1f, 1f);

            goPinBorder.transform.SetParent(goPin.transform);
            goPinBorder.transform.localPosition = new Vector3(0f, 0f, -0.00001f);

            // Attach pin data to the GameObject
            PinAnimatedSprite pin = goPin.AddComponent<PinAnimatedSprite>();
            pin.SetPinData(pinDef);
            _pins.Add(pin);

            pin.gameObject.transform.SetParent(transform);

            SetPinPosition(pinDef, goPin, offsetZ, gameMap);
        }

        private void SetPinPosition(PinDef pinDef, GameObject goPin, float offsetZ, GameMap gameMap)
        {
            string roomName = pinDef.pinScene ?? pinDef.sceneName;

            if (TryGetRoomPos(roomName, gameMap, out Vector2 vec))
            {
                vec.Scale(new Vector2(1.46f, 1.46f));

                vec += new Vector2(pinDef.offsetX, pinDef.offsetY);

                goPin.transform.localPosition = new Vector3(vec.x, vec.y, -2f + offsetZ);
            }
            else
            {
                MapModS.Instance.LogWarn($"{roomName} is not a valid room name!");

                pinDef.pinLocationState = PLS.Cleared;
            }
        }

        // For debugging pins
        //public void ReadjustPinPostiions()
        //{
        //    foreach (PinAnimatedSprite pin in _pins)
        //    {
        //        if (DataLoader.newPins.TryGetValue(pin.pinDef.name, out PinDef newPinDef))
        //        {
        //            SetPinPosition(newPinDef, pin.gameObject, GameManager.instance.gameMap.GetComponent<GameMap>());
        //        }
        //    }
        //}

        private bool TryGetRoomPos(string roomName, GameMap gameMap, out Vector2 pos)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (roomObj.gameObject.name == roomName)
                    {
                        Vector3 roomVec = roomObj.transform.localPosition;
                        roomVec.Scale(areaObj.transform.localScale);
                        pos = areaObj.transform.localPosition + roomVec;
                        return true;
                    }
                }
            }

            pos = Vector2.zero;
            return false;
        }

        public void UpdatePins(MapZone mapZone, HashSet<string> transitionPinScenes)
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                pin.ResetSpriteIndex();

                PinDef pd = pin.PD;

                UpdatePinLocationState(pd);

                // Show based on map settings
                if ((mapZone == MapZone.NONE && (MapModS.LS.mapMode != MapMode.PinsOverMap || Utils.GetMapSetting(pd.mapZone)))
                    || mapZone == pd.mapZone)
                {
                    pin.PD.canShowOnMap = true;

#if RELEASE
                    if (transitionPinScenes.Count != 0 && !transitionPinScenes.Contains(pd.sceneName))
                    {
                        pin.PD.canShowOnMap = false;
                    }
#endif
                }
                else
                {
                    pin.PD.canShowOnMap = false;
                }

                if (pd.pinLocationState == PLS.Cleared
                    || pd.pinLocationState == PLS.ClearedPersistent && !MapModS.GS.persistentOn)
                {
                    pin.PD.canShowOnMap = false;
                }
            }

            _pins.RemoveAll(p => p.gameObject == null);
        }

        public void UpdatePinLocationState(PinDef pd)
        {
            // Check vanilla item
            if (pd.pinLocationState == PLS.NonRandomizedUnchecked)
            {
                if (MainData.HasObtainedVanillaItem(pd))
                {
                    pd.pinLocationState = PLS.Cleared;
                }
                return;
            }

            // Remove obtained rando items from list
            if (pd.randoItems != null && pd.randoItems.Any())
            {
                List<ItemDef> newRandoItems = new();

                foreach (ItemDef item in pd.randoItems)
                {
                    if ((!RM.RS.TrackerData.obtainedItems.Contains(item.id)
                        && !RM.RS.TrackerData.outOfLogicObtainedItems.Contains(item.id))
                        || item.persistent)
                    {
                        newRandoItems.Add(item);
                    }
                }

                pd.randoItems = newRandoItems;
            }

            // Check if reachable
            if (RM.RS.TrackerData.uncheckedReachableLocations.Contains(pd.name))
            {
                if (RM.RS.TrackerDataWithoutSequenceBreaks.uncheckedReachableLocations.Contains(pd.name))
                {
                    pd.pinLocationState = PLS.UncheckedReachable;
                }
                else
                {
                    pd.pinLocationState = PLS.OutOfLogicReachable;
                }
            }

            // Check if previewed
            if (RM.RS.TrackerData.previewedLocations.Contains(pd.name) && pd.canPreviewItem)
            {
                pd.pinLocationState = PLS.Previewed;
            }

            // Check if cleared
            if (RM.RS.TrackerData.clearedLocations.Contains(pd.name))
            {
                if (pd.randoItems != null && pd.randoItems.Any(i => i.persistent))
                {
                    pd.pinLocationState = PLS.ClearedPersistent;
                }
                else
                {
                    pd.pinLocationState = PLS.Cleared;
                }
            }
        }

        // Called every time when any relevant setting is changed, or when the Map is opened
        public void SetPinsActive()
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                if (!pin.PD.canShowOnMap)
                {
                    pin.gameObject.SetActive(false);
                    continue;
                }

                string targetPoolGroup = "";

                // Custom pool setting control
                if (MapModS.LS.groupBy == GroupBy.Location)
                {
                    targetPoolGroup = pin.PD.locationPoolGroup;
                }
                
                if (MapModS.LS.groupBy == GroupBy.Item)
                {
                    targetPoolGroup = GetActiveRandoItemGroup(pin.PD);

                    if (targetPoolGroup == "" && !pin.PD.randomized)
                    {
                        targetPoolGroup = pin.PD.locationPoolGroup;
                    }
                    // All of the corresponding item groups for that location are off
                    else if (targetPoolGroup == "" && pin.PD.randomized)
                    {
                        pin.gameObject.SetActive(false);
                        continue;
                    }
                }

                switch (MapModS.LS.GetPoolGroupSetting(targetPoolGroup))
                {
                    case PGS.Off:
                        pin.gameObject.SetActive(false);
                        continue;
                    case PGS.On:
                        pin.gameObject.SetActive(true);
                        continue;
                    case PGS.Mixed:
                        pin.gameObject.SetActive((pin.PD.randomized && MapModS.LS.randomizedOn)
                            || (!pin.PD.randomized && MapModS.LS.othersOn));
                        continue;
                }
            }
        }

        private string GetActiveRandoItemGroup(PinDef pd)
        {
            if (pd.randomized && pd.randoItems != null && pd.randoItems.Any())
            {
                ItemDef item = pd.randoItems.FirstOrDefault(i => MapModS.LS.GetPoolGroupSetting(i.poolGroup) == PGS.On
                    || (MapModS.LS.GetPoolGroupSetting(i.poolGroup) == PGS.Mixed && (MapModS.LS.randomizedOn || MapModS.LS.othersOn)));

                if (item != null)
                {
                    return item.poolGroup;
                }
            }
            return "";
        }

        private bool HasRandoItemGroup(PinDef pd, string poolGroup)
        {
            if (pd.randomized && pd.randoItems != null && pd.randoItems.Any())
            {
                ItemDef item = pd.randoItems.FirstOrDefault(i => i.poolGroup == poolGroup);

                if (item != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void ResetPoolSettings()
        {
            foreach (string group in MainData.usedPoolGroups)
            {
                bool hasRandomized = false;
                bool hasOthers = false;

                if (MapModS.LS.groupBy == GroupBy.Location)
                {
                    if (_pins.Where(p => p.PD.locationPoolGroup == group).Any(p => p.PD.randomized))
                    {
                        hasRandomized = true;
                    }

                    if (_pins.Where(p => p.PD.locationPoolGroup == group).Any(p => !p.PD.randomized))
                    {
                        hasOthers = true;
                    }
                }
                else if (MapModS.LS.groupBy == GroupBy.Item)
                {
                    if (_pins.Any(p => HasRandoItemGroup(p.PD, group)))
                    {
                        hasRandomized = true;
                    }

                    if (_pins.Where(p => p.PD.locationPoolGroup == group).Any(p => !p.PD.randomized))
                    {
                        hasOthers = true;
                    }
                }

                if (hasRandomized == true && hasOthers == false)
                {
                    if (MapModS.LS.randomizedOn)
                    {
                        MapModS.LS.SetPoolGroupSetting(group, PGS.On);
                    }
                    else
                    {
                        MapModS.LS.SetPoolGroupSetting(group, PGS.Off);
                    }
                }
                else if (hasRandomized == false && hasOthers == true)
                {
                    if (MapModS.LS.othersOn)
                    {
                        MapModS.LS.SetPoolGroupSetting(group, PGS.On);
                    }
                    else
                    {
                        MapModS.LS.SetPoolGroupSetting(group, PGS.Off);
                    }
                }
                else if (hasRandomized == true && hasOthers == true)
                {
                    if (MapModS.LS.randomizedOn && MapModS.LS.othersOn)
                    {
                        MapModS.LS.SetPoolGroupSetting(group, PGS.On);
                    }
                    else if (MapModS.LS.randomizedOn || MapModS.LS.othersOn)
                    {
                        MapModS.LS.SetPoolGroupSetting(group, PGS.Mixed);
                    }
                    else
                    {
                        MapModS.LS.SetPoolGroupSetting(group, PGS.Off);
                    }
                }
                else
                {
                    MapModS.LS.SetPoolGroupSetting(group, PGS.Off);
                }
            }
        }

        private HashSet<string> randomizedGroups = new();
        private HashSet<string> othersGroups = new();

        public void GetRandomizedOthersGroups()
        {
            if (MapModS.LS.groupBy == GroupBy.Location)
            {
                randomizedGroups = new(_pins.Where(p => p.PD.randomized).Select(p => p.PD.locationPoolGroup));
            }
            else if (MapModS.LS.groupBy == GroupBy.Item)
            {
                randomizedGroups = new(_pins.Where(p => p.PD.randomized).SelectMany(p => p.PD.randoItems).Select(i => i.poolGroup));
            }

            othersGroups = new(_pins.Where(p => !p.PD.randomized).Select(p => p.PD.locationPoolGroup));
        }

        public bool IsRandomizedCustom()
        {
            if (!randomizedGroups.Any()) return false;

            return (randomizedGroups.Any(g => MapModS.LS.GetPoolGroupSetting(g) == PGS.On && !MapModS.LS.randomizedOn
                     || MapModS.LS.GetPoolGroupSetting(g) == PGS.Off && MapModS.LS.randomizedOn));
        }

        public bool IsOthersCustom()
        {
            if (!othersGroups.Any()) return false;

            return (othersGroups.Any(g => MapModS.LS.GetPoolGroupSetting(g) == PGS.On && !MapModS.LS.othersOn
                     || MapModS.LS.GetPoolGroupSetting(g) == PGS.Off && MapModS.LS.othersOn));
        }

        public void SetSprites()
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                pin.ResetSpriteIndex();
                pin.SetSprite();
            }
        }

        public void DestroyPins()
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                Destroy(pin.gameObject);
            }

            _pins.Clear();
        }

        // The following is for the lookup panel
        private double DistanceToMiddle(Transform transform)
        {
            return Math.Pow(transform.position.x, 2) + Math.Pow(transform.position.y, 2);
        }

        public bool GetPinClosestToMiddle(string previousLocation, out string selectedLocation)
        {
            selectedLocation = "None selected";
            double minDistance = double.PositiveInfinity;

            foreach (PinAnimatedSprite pin in _pins)
            {
                if (!pin.gameObject.activeInHierarchy) continue;

                double distance = DistanceToMiddle(pin.transform);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    selectedLocation = pin.PD.name;
                }
            }

            return previousLocation != selectedLocation;
        }

        public void ResizePins(string selectedLocation)
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                if (pin.PD.name == selectedLocation && MapModS.GS.lookupOn)
                {
                    pin.SetSizeAndColorSelected();
                }
                else
                {
                    pin.SetSizeAndColor();
                }
            }
        }

        protected void Start()
        {
            gameObject.SetActive(false);
        }
    }
}