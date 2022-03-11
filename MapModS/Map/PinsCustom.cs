using GlobalEnums;
using MapModS.Data;
using MapModS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapModS.Map
{
    public class PinsCustom : MonoBehaviour
    {
        private List<PinAnimatedSprite> _pins = new();

        public void MakePins(GameMap gameMap)
        {
            DestroyPins();

            foreach (PinDef pinDef in DataLoader.GetUsedPinArray())
            {
                try
                {
                    MakePin(pinDef, gameMap);
                }
                catch (Exception e)
                {
                    MapModS.Instance.LogError(e);
                }
            }

            GetRandomizedOthersGroups();
        }

        private void MakePin(PinDef pinDef, GameMap gameMap)
        {
            // Create new pin GameObject
            GameObject goPin = new($"pin_mapmod_{pinDef.name}")
            {
                layer = 30
            };

            // Attach sprite renderer to the GameObject
            SpriteRenderer sr = goPin.AddComponent<SpriteRenderer>();

            // Initialize sprite to vanillaPool
            sr.sprite = SpriteManager.GetSpriteFromPool(pinDef.locationPoolGroup, PinBorderColor.Normal);
            sr.sortingLayerName = "HUD";
            sr.size = new Vector2(1f, 1f);

            // Attach pin data to the GameObject
            PinAnimatedSprite pin = goPin.AddComponent<PinAnimatedSprite>();
            pin.SetPinData(pinDef);
            _pins.Add(pin);

            pin.gameObject.transform.SetParent(transform);
            SetPinPosition(pinDef, goPin, gameMap);
        }

        private void SetPinPosition(PinDef pinDef, GameObject goPin, GameMap gameMap)
        {
            string roomName = pinDef.pinScene ?? pinDef.sceneName;

            Vector3 vec = GetRoomPos(roomName, gameMap);
            vec.Scale(new Vector3(1.46f, 1.46f, 1));

            vec += new Vector3(pinDef.offsetX, pinDef.offsetY, pinDef.offsetZ);

            goPin.transform.localPosition = new Vector3(vec.x, vec.y, vec.z - 0.01f);
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

        private Vector3 GetRoomPos(string roomName, GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (roomObj.gameObject.name == roomName)
                    {
                        Vector3 roomVec = roomObj.transform.localPosition;
                        roomVec.Scale(areaObj.transform.localScale);
                        return areaObj.transform.localPosition + roomVec;
                    }
                }
            }

            MapModS.Instance.LogWarn($"{roomName} is not a valid room name!");
            return new Vector3(0, 0, 0);
        }

        public void UpdatePins(MapZone mapZone, HashSet<string> transitionPinScenes)
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                pin.ResetSpriteIndex();

                PinDef pd = pin.pinDef;

                // Show based on map settings
                if ((mapZone == MapZone.NONE && (MapModS.LS.mapMode != MapMode.PinsOverMap || SettingsUtil.GetMapSetting(pd.mapZone)))
                    || mapZone == pd.mapZone)
                {
                    //pin.gameObject.SetActive(true);

                    pin.pinDef.canShowOnMap = true;

                    if (transitionPinScenes.Count != 0 && !transitionPinScenes.Contains(pd.sceneName))
                    {
                        //pin.gameObject.SetActive(false);

                        pin.pinDef.canShowOnMap = false;
                    }
                }
                else
                {
                    //pin.gameObject.SetActive(false);

                    pin.pinDef.canShowOnMap = false;
                }

                if (pd.pinLocationState == PinLocationState.ClearedPersistent)
                {
                    if (!MapModS.GS.persistentOn)
                    {
                        //pin.gameObject.SetActive(false);

                        pin.pinDef.canShowOnMap = false;
                    }
                    continue;
                }

                if (pd.pinLocationState == PinLocationState.NonRandomizedUnchecked)
                {
                    if (DataLoader.HasObtainedVanillaItem(pd))
                    {
                        pd.pinLocationState = PinLocationState.Cleared;
                        //pin.gameObject.SetActive(false);

                        pin.pinDef.canShowOnMap = false;

                        // Destroy(pin.gameObject);
                    }
                    continue;
                }

                if (RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableLocations.Contains(pd.name))
                {
                    if (RandomizerMod.RandomizerMod.RS.TrackerDataWithoutSequenceBreaks.uncheckedReachableLocations.Contains(pin.pinDef.name))
                    {
                        pd.pinLocationState = PinLocationState.UncheckedReachable;
                    }
                    else
                    {
                        pd.pinLocationState = PinLocationState.OutOfLogicReachable;
                    }
                }

                if (RandomizerMod.RandomizerMod.RS.TrackerData.previewedLocations.Contains(pd.name) && pd.canPreviewItem)
                {
                    pd.pinLocationState = PinLocationState.Previewed;
                }

                // Remove obtained rando items from list
                if (pd.randoItems != null && pd.randoItems.Any())
                {
                    List<ItemDef> newRandoItems = new();

                    foreach (ItemDef item in pd.randoItems)
                    {
                        if ((!RandomizerMod.RandomizerMod.RS.TrackerData.obtainedItems.Contains(item.id)
                            && !RandomizerMod.RandomizerMod.RS.TrackerData.outOfLogicObtainedItems.Contains(item.id))
                            || item.persistent)
                        {
                            newRandoItems.Add(item);
                        }
                    }

                    pd.randoItems = newRandoItems;
                }

                if (RandomizerMod.RandomizerMod.RS.TrackerData.clearedLocations.Contains(pd.name))
                {
                    if (pd.randoItems != null && pd.randoItems.Any(i => i.persistent))
                    {
                        pd.pinLocationState = PinLocationState.ClearedPersistent;
                    }
                    else
                    {
                        pd.pinLocationState = PinLocationState.Cleared;
                        //pin.gameObject.SetActive(false);

                        pin.pinDef.canShowOnMap = false;

                        // Destroy(pin.gameObject);
                    }
                }
            }

            _pins.RemoveAll(p => p.gameObject == null);
        }

        // Called every time when any relevant setting is changed, or when the Map is opened
        public void SetPinsActive()
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                if (!pin.pinDef.canShowOnMap)
                {
                    pin.gameObject.SetActive(false);
                    continue;
                }

                PoolGroup targetPoolGroup = PoolGroup.Unknown;

                // Custom pool setting control
                if (MapModS.LS.groupBy == GroupBy.Location)
                {
                    targetPoolGroup = pin.pinDef.locationPoolGroup;
                }
                
                if (MapModS.LS.groupBy == GroupBy.Item)
                {
                    targetPoolGroup = GetActiveRandoItemGroup(pin.pinDef);

                    if (targetPoolGroup == PoolGroup.Unknown && !pin.pinDef.randomized)
                    {
                        targetPoolGroup = pin.pinDef.locationPoolGroup;
                    }
                    // All of the corresponding item groups for that location are off
                    else if (targetPoolGroup == PoolGroup.Unknown && pin.pinDef.randomized)
                    {
                        pin.gameObject.SetActive(false);
                        continue;
                    }
                }

                switch (MapModS.LS.GetPoolGroupState(targetPoolGroup))
                {
                    case PoolGroupState.Off:
                        pin.gameObject.SetActive(false);
                        continue;
                    case PoolGroupState.On:
                        pin.gameObject.SetActive(true);
                        continue;
                    case PoolGroupState.Mixed:
                        pin.gameObject.SetActive((pin.pinDef.randomized && MapModS.LS.randomizedOn)
                            || (!pin.pinDef.randomized && MapModS.LS.othersOn));
                        continue;
                }
            }
        }

        private PoolGroup GetActiveRandoItemGroup(PinDef pd)
        {
            if (pd.randomized && pd.randoItems != null && pd.randoItems.Any())
            {
                ItemDef item = pd.randoItems.FirstOrDefault(i => MapModS.LS.GetPoolGroupState(i.poolGroup) == PoolGroupState.On
                    || (MapModS.LS.GetPoolGroupState(i.poolGroup) == PoolGroupState.Mixed && (MapModS.LS.randomizedOn || MapModS.LS.othersOn)));

                if (item != null)
                {
                    return item.poolGroup;
                }
            }
            return PoolGroup.Unknown;
        }

        private bool HasRandoItemGroup(PinDef pd, PoolGroup group)
        {
            if (pd.randomized && pd.randoItems != null && pd.randoItems.Any())
            {
                ItemDef item = pd.randoItems.FirstOrDefault(i => i.poolGroup == group);

                if (item != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void ResetPoolSettings()
        {
            foreach (PoolGroup group in Enum.GetValues(typeof(PoolGroup)))
            {
                bool hasRandomized = false;
                bool hasOthers = false;

                if (MapModS.LS.groupBy == GroupBy.Location)
                {
                    if (_pins.Where(p => p.pinDef.locationPoolGroup == group).Any(p => p.pinDef.randomized))
                    {
                        hasRandomized = true;
                    }

                    if (_pins.Where(p => p.pinDef.locationPoolGroup == group).Any(p => !p.pinDef.randomized))
                    {
                        hasOthers = true;
                    }
                }
                else if (MapModS.LS.groupBy == GroupBy.Item)
                {
                    if (_pins.Any(p => HasRandoItemGroup(p.pinDef, group)))
                    {
                        hasRandomized = true;
                    }

                    if (_pins.Where(p => p.pinDef.locationPoolGroup == group).Any(p => !p.pinDef.randomized))
                    {
                        hasOthers = true;
                    }
                }

                if (hasRandomized == true && hasOthers == false)
                {
                    if (MapModS.LS.randomizedOn)
                    {
                        MapModS.LS.SetPoolGroupState(group, PoolGroupState.On);
                    }
                    else
                    {
                        MapModS.LS.SetPoolGroupState(group, PoolGroupState.Off);
                    }
                }
                else if (hasRandomized == false && hasOthers == true)
                {
                    if (MapModS.LS.othersOn)
                    {
                        MapModS.LS.SetPoolGroupState(group, PoolGroupState.On);
                    }
                    else
                    {
                        MapModS.LS.SetPoolGroupState(group, PoolGroupState.Off);
                    }
                }
                else if (hasRandomized == true && hasOthers == true)
                {
                    if (MapModS.LS.randomizedOn && MapModS.LS.othersOn)
                    {
                        MapModS.LS.SetPoolGroupState(group, PoolGroupState.On);
                    }
                    else if (MapModS.LS.randomizedOn || MapModS.LS.othersOn)
                    {
                        MapModS.LS.SetPoolGroupState(group, PoolGroupState.Mixed);
                    }
                    else
                    {
                        MapModS.LS.SetPoolGroupState(group, PoolGroupState.Off);
                    }
                }
                else
                {
                    MapModS.LS.SetPoolGroupState(group, PoolGroupState.Off);
                }
            }
        }

        private HashSet<PoolGroup> randomizedGroups = new();
        private HashSet<PoolGroup> othersGroups = new();

        public void GetRandomizedOthersGroups()
        {
            if (MapModS.LS.groupBy == GroupBy.Location)
            {
                randomizedGroups = new HashSet<PoolGroup>(_pins.Where(p => p.pinDef.randomized).Select(p => p.pinDef.locationPoolGroup));
            }
            else if (MapModS.LS.groupBy == GroupBy.Item)
            {
                randomizedGroups = new HashSet<PoolGroup>(_pins.Where(p => p.pinDef.randomized).SelectMany(p => p.pinDef.randoItems).Select(i => i.poolGroup));
            }

            othersGroups = new HashSet<PoolGroup>(_pins.Where(p => !p.pinDef.randomized).Select(p => p.pinDef.locationPoolGroup));
        }

        public bool IsRandomizedCustom()
        {
            if (!randomizedGroups.Any()) return false;

            return (randomizedGroups.Any(g => MapModS.LS.GetPoolGroupState(g) == PoolGroupState.On && !MapModS.LS.randomizedOn
                     || MapModS.LS.GetPoolGroupState(g) == PoolGroupState.Off && MapModS.LS.randomizedOn));
        }

        public bool IsOthersCustom()
        {
            if (!othersGroups.Any()) return false;

            return (othersGroups.Any(g => MapModS.LS.GetPoolGroupState(g) == PoolGroupState.On && !MapModS.LS.othersOn
                     || MapModS.LS.GetPoolGroupState(g) == PoolGroupState.Off && MapModS.LS.othersOn));

            //return (Enum.GetValues(typeof(PoolGroup)).Cast<PoolGroup>().Except(randomizedGroups).Where(g => g != PoolGroup.Unknown && g != PoolGroup.Shop)
            //    .Any(g => MapModS.LS.GetPoolGroupState(g) == PoolGroupState.On && !MapModS.LS.othersOn
            //         || MapModS.LS.GetPoolGroupState(g) == PoolGroupState.Off && MapModS.LS.othersOn));
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
            //Destroy(randomizedGroup);
            //Destroy(othersGroup);
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
                    selectedLocation = pin.pinDef.name;
                }
            }

            return previousLocation != selectedLocation;
        }

        public void ResizePins(string selectedLocation)
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                if (pin.pinDef.name == selectedLocation)
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