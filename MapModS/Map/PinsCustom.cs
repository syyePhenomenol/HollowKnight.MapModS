using GlobalEnums;
using ItemChanger;
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
        private readonly Dictionary<PoolGroup, GameObject> _Groups = new();

        private readonly List<PinAnimatedSprite> _pins = new();

        public void MakePins(GameMap gameMap)
        {
            DestroyPins();

            RandomizedGroups.Clear();
            OthersGroups.Clear();

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

            AddGroupToSet(pinDef);
        }

        public void ReassignGroups()
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                AssignGroup(pin);
            }
        }

        private void AssignGroup(PinAnimatedSprite pin)
        {
            if (pin.pinDef.pinLocationState == PinLocationState.Cleared) return;

            PoolGroup poolGroup = PoolGroup.Unknown;

            if (MapModS.LS.groupBy == GroupBy.Item)
            {
                if (pin.pinDef.pinLocationState == PinLocationState.NonRandomizedUnchecked
                    || pin.pinDef.randoItems == null || !pin.pinDef.randoItems.Any())
                {
                    poolGroup = pin.pinDef.locationPoolGroup;
                }
                else
                {
                    ItemDef item = pin.pinDef.randoItems.FirstOrDefault(i => MapModS.LS.GroupSettings[i.poolGroup].On) ?? pin.pinDef.randoItems.First();
                    poolGroup = item.poolGroup;
                }
            }
            else
            {
                poolGroup = pin.pinDef.locationPoolGroup;
            }

            if (!_Groups.ContainsKey(poolGroup))
            {
                _Groups[poolGroup] = new GameObject("PinGroup " + poolGroup);
                _Groups[poolGroup].transform.SetParent(transform);
                //_Groups[poolGroup].SetActive(false);
            }

            pin.gameObject.transform.SetParent(_Groups[poolGroup].transform);
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

        // Used for updating button states
        public HashSet<PoolGroup> RandomizedGroups = new();

        public HashSet<PoolGroup> OthersGroups = new();

        public void InitializePoolSettings()
        {
            foreach (PoolGroup group in RandomizedGroups)
            {
                MapModS.LS.SetOnFromGroup(group, MapModS.GS.randomizedOn);
            }

            foreach (PoolGroup group in OthersGroups)
            {
                MapModS.LS.SetOnFromGroup(group, MapModS.GS.othersOn);
            }
        }

        private void AddGroupToSet(PinDef pinDef)
        {
            if (pinDef.locationPoolGroup == PoolGroup.Shop
                || pinDef.pinLocationState != PinLocationState.NonRandomizedUnchecked)
            {
                OthersGroups.Remove(pinDef.locationPoolGroup);
                RandomizedGroups.Add(pinDef.locationPoolGroup);
            }
            else if (!RandomizedGroups.Contains(pinDef.locationPoolGroup))
            {
                OthersGroups.Add(pinDef.locationPoolGroup);
            }
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

        internal void ToggleRandomized()
        {
            MapModS.GS.ToggleRandomizedOn();

            foreach (PoolGroup group in RandomizedGroups)
            {
                MapModS.LS.SetOnFromGroup(group, MapModS.GS.randomizedOn);
            }

            RefreshGroups();
        }

        internal void ToggleOthers()
        {
            MapModS.GS.ToggleOthersOn();

            foreach (PoolGroup group in OthersGroups)
            {
                MapModS.LS.SetOnFromGroup(group, MapModS.GS.othersOn);
            }

            RefreshGroups();
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

                if (pd.pinLocationState == PinLocationState.Cleared)
                {
                    pin.gameObject.SetActive(false);
                    continue;
                }

                if (pd.pinLocationState == PinLocationState.ClearedPersistent) continue;

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

                if (RandomizerMod.RandomizerMod.RS.TrackerData.previewedLocations.Contains(pd.name) && pd.canPreview)
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
            foreach (PoolGroup group in _Groups.Keys)
            {
                _Groups[group].SetActive(MapModS.LS.GetOnFromGroup(group));
            }
        }

        public void SetSprites()
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                pin.SetSprite();
            }
        }

        public void ResizePins()
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                pin.SetSizeAndColor();
            }
        }

        public void DestroyPins()
        {
            foreach (PinAnimatedSprite pin in _pins)
            {
                Destroy(pin.gameObject);
            }

            _pins.Clear();
            RandomizedGroups.Clear();
            OthersGroups.Clear();
        }

        protected void Start()
        {
            gameObject.SetActive(false);
        }
    }
}