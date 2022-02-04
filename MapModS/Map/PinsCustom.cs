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

            string roomName = pinDef.pinScene ?? pinDef.sceneName;

            Vector3 vec = GetRoomPos(roomName, gameMap);
            vec.Scale(new Vector3(1.46f, 1.46f, 1));

            vec += new Vector3(pinDef.offsetX, pinDef.offsetY, pinDef.offsetZ);

            goPin.transform.localPosition = new Vector3(vec.x, vec.y, vec.z - 0.01f);
        }

        private void AssignGroup(PinAnimatedSprite pin)
        {
            PoolGroup poolGroup;

            if (pin.pinDef.randoItems == null || !pin.pinDef.randoItems.Any())
            {
                poolGroup = pin.pinDef.locationPoolGroup;
            }
            else
            {
                poolGroup = pin.pinDef.randoItems.First().poolGroup;
            }

            if (!_Groups.ContainsKey(poolGroup))
            {
                _Groups[poolGroup] = new GameObject("PinGroup " + poolGroup);
                _Groups[poolGroup].transform.SetParent(transform);
                _Groups[poolGroup].SetActive(true);
            }

            pin.gameObject.transform.SetParent(_Groups[poolGroup].transform);
        }

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
        public List<PoolGroup> RandomizedGroups = new();

        public List<PoolGroup> OthersGroups = new();

        internal void FindRandomizedGroups()
        {
            RandomizedGroups.Clear();
            OthersGroups.Clear();

            foreach (PoolGroup group in _Groups.Keys)
            {
                AddGroupToList(group);
            }
        }

        // If any pin has vanillaPool != spoilerPool, consider the whole group Randomized
        internal void AddGroupToList(PoolGroup group)
        {
            if (group == PoolGroup.Shop)
            {
                RandomizedGroups.Add(group);
                return;
            }

            foreach (Transform pinT in _Groups[group].GetComponentsInChildren<Transform>())
            {
                PinAnimatedSprite pin = pinT.GetComponent<PinAnimatedSprite>();

                if (pin == null) continue;

                if (pin.pinDef.randoItems != null)
                {
                    RandomizedGroups.Add(group);
                    return;
                }
            }

            OthersGroups.Add(group);
        }

        internal void ToggleSpoilers()
        {
            MapModS.LS.SpoilerOn = !MapModS.LS.SpoilerOn;
            SetSprites();
        }

        internal void TogglePinStyle()
        {
            switch (MapModS.LS.pinStyle)
            {
                case PinStyle.Normal:
                    MapModS.LS.pinStyle = PinStyle.Q_Marks_1;
                    break;

                case PinStyle.Q_Marks_1:
                    MapModS.LS.pinStyle = PinStyle.Q_Marks_2;
                    break;

                case PinStyle.Q_Marks_2:
                    MapModS.LS.pinStyle = PinStyle.Q_Marks_3;
                    break;

                case PinStyle.Q_Marks_3:
                    MapModS.LS.pinStyle = PinStyle.Normal;
                    break;
            }

            SetSprites();
        }

        internal void ToggleRandomized()
        {
            MapModS.LS.RandomizedOn = !MapModS.LS.RandomizedOn;
            foreach (PoolGroup group in RandomizedGroups)
            {
                MapModS.LS.SetOnFromGroup(group, MapModS.LS.RandomizedOn);
            }

            RefreshGroups();
        }

        internal void ToggleOthers()
        {
            MapModS.LS.OthersOn = !MapModS.LS.OthersOn;
            foreach (PoolGroup group in OthersGroups)
            {
                MapModS.LS.SetOnFromGroup(group, MapModS.LS.OthersOn);
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
                    if ((pd.pdBool != null && PlayerData.instance.GetBool(pd.pdBool))
                        || (pd.pdInt != null && PlayerData.instance.GetInt(pd.pdInt) >= pd.pdIntValue)
                        || (pd.locationPoolGroup == PoolGroup.WhisperingRoots && PlayerData.instance.scenesEncounteredDreamPlantC.Contains(pd.sceneName))
                        || (pd.locationPoolGroup == PoolGroup.Grubs && PlayerData.instance.scenesGrubRescued.Contains(pd.sceneName))
                        || (pd.locationPoolGroup == PoolGroup.GrimmkinFlames && PlayerData.instance.scenesFlameCollected.Contains(pd.sceneName))
                        || (MapModS.LS.ObtainedVanillaItems.ContainsKey(pd.objectName + pd.sceneName)))
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