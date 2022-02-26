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
        private readonly List<PinAnimatedSprite> _pins = new();

        private GameObject randomizedGroup;
        private GameObject othersGroup;

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

            // Set pin transform (by pool)
            AssignGroup(pin);
            SetPinPosition(pinDef, goPin, gameMap);

            //AddGroupToSet(pinDef);
        }

        private void AssignGroup(PinAnimatedSprite pin)
        {
            if (pin.pinDef.pinLocationState != PinLocationState.NonRandomizedUnchecked)
            {
                if (randomizedGroup == null)
                {
                    randomizedGroup = new GameObject("Randomized Pin Group");
                    randomizedGroup.transform.SetParent(transform);
                    randomizedGroup.SetActive(MapModS.GS.randomizedOn);
                }

                pin.gameObject.transform.SetParent(randomizedGroup.transform);
            }
            else
            {
                if (othersGroup == null)
                {
                    othersGroup = new GameObject("Randomized Pin Group");
                    othersGroup.transform.SetParent(transform);
                    othersGroup.SetActive(MapModS.GS.othersOn);
                }

                pin.gameObject.transform.SetParent(othersGroup.transform);
            }
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

        internal void ToggleSpoilers()
        {
            MapModS.LS.SpoilerOn = !MapModS.LS.SpoilerOn;

            SetSprites();
        }

        internal void TogglePinStyle()
        {
            MapModS.GS.TogglePinStyle();

            SetSprites();
        }

        public void UpdatePins(MapZone mapZone, HashSet<string> transitionPinScenes)
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                // Custom pool setting control
                if (MapModS.LS.groupBy == GroupBy.Location)
                {
                    if (!MapModS.LS.GetOnFromGroup(pin.pinDef.locationPoolGroup))
                    {
                        pin.gameObject.SetActive(false);
                        continue;
                    }
                }
                else if (MapModS.LS.groupBy == GroupBy.Item)
                {
                    if (pin.pinDef.pinLocationState == PinLocationState.NonRandomizedUnchecked
                        || pin.pinDef.randoItems == null || !pin.pinDef.randoItems.Any())
                    {
                        if (!MapModS.LS.GetOnFromGroup(pin.pinDef.locationPoolGroup))
                        {
                            pin.gameObject.SetActive(false);
                            continue;
                        }
                    }
                    else
                    {
                        if (pin.pinDef.randoItems.All(i => !MapModS.LS.GetOnFromGroup(i.poolGroup)))
                        {
                            pin.gameObject.SetActive(false);
                            continue;
                        }
                    }
                }

                pin.ResetSpriteIndex();

                PinDef pd = pin.pinDef;

                // Show based on map settings
                if ((mapZone == MapZone.NONE && (MapModS.LS.mapMode != MapMode.PinsOverMap || SettingsUtil.GetMapSetting(pd.mapZone)))
                    || mapZone == pd.mapZone)
                {
                    pin.gameObject.SetActive(true);

                    if (transitionPinScenes.Count != 0 && !transitionPinScenes.Contains(pd.sceneName))
                    {
                        pin.gameObject.SetActive(false);
                    }
                }
                else
                {
                    pin.gameObject.SetActive(false);
                }

                // Show based on location/item state
                if (pd.pinLocationState == PinLocationState.Cleared)
                {
                    pin.gameObject.SetActive(false);
                    continue;
                }

                if (pd.pinLocationState == PinLocationState.ClearedPersistent)
                {
                    if (!MapModS.GS.persistentOn)
                    {
                        pin.gameObject.SetActive(false);
                    }
                    continue;
                }

                if (pd.pinLocationState == PinLocationState.NonRandomizedUnchecked)
                {
                    if (DataLoader.HasObtainedVanillaItem(pd))
                    {
                        pd.pinLocationState = PinLocationState.Cleared;
                        pin.gameObject.SetActive(false);
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

                    if (pd.randoItems.Any())
                    {
                        AssignGroup(pin);
                    }
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
                        pin.gameObject.SetActive(false);
                    }
                }
            }
        }

        // Called every time when any relevant setting is changed, or when the Map is opened
        public void RefreshGroups()
        {
            if (randomizedGroup == null || othersGroup == null) return;

            randomizedGroup.SetActive(MapModS.GS.randomizedOn);
            othersGroup.SetActive(MapModS.GS.othersOn);
        }

        public void SetSprites()
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
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
            Destroy(randomizedGroup);
            Destroy(othersGroup);
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