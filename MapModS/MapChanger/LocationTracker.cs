using HutongGames.PlayMaker;
using ItemChanger;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vasi;

namespace MapChanger
{
    internal class LocationTracker : HookModule
    {
        internal record TrackingDef
        {
            [JsonProperty]
            internal string ObjectName { get; init; }
            [JsonProperty]
            internal string PdBoolName { get; init; }
            [JsonProperty]
            internal string[] PdBoolNames { get; init; }
            [JsonProperty]
            internal string PdIntName { get; init; }
            [JsonProperty]
            internal int PdIntValue { get; init; }
            [JsonProperty]
            internal string PdListName { get; init; }
        }

        private class TrackGeoRock : FsmStateAction
        {
            private readonly GameObject go;
            private readonly GeoRockData grd;

            internal TrackGeoRock(GameObject go)
            {
                this.go = go;
                grd = this.go.GetComponent<GeoRock>().geoRockData;
            }

            public override void OnEnter()
            {
                if (grd.id.Contains("-")) return;
                AddVanillaItem(grd.id, grd.sceneName);
                //MapModS.LS.geoRockCounter ++;
                MapChangerMod.Instance.LogDebug("Geo Rock broken");
                MapChangerMod.Instance.LogDebug(" ID: " + grd.id);
                MapChangerMod.Instance.LogDebug(" Scene: " + grd.sceneName);
            }
        }

        private class TrackItem : FsmStateAction
        {
            private readonly string name;

            internal TrackItem(string name)
            {
                this.name = name;
            }

            public override void OnEnter()
            {
                string scene = Finder.CurrentScene() ?? "";
                if (name.Contains("-")) return;
                AddVanillaItem(name, scene);
                MapChangerMod.Instance.LogDebug("Item picked up");
                MapChangerMod.Instance.LogDebug(" Name: " + name);
                MapChangerMod.Instance.LogDebug(" Scene: " + scene);
            }
        }

        private static Dictionary<string, TrackingDef> trackingDefs = new();

        private static HashSet<string> clearedLocations;

        internal static void Load()
        {
            trackingDefs = JsonUtil.Deserialize<Dictionary<string, TrackingDef>>("MapModS.MapChanger.Resources.tracking.json");
        }

        internal static void VerifyTrackingDefs()
        {
            foreach (string name in trackingDefs.Keys)
            {
                MapChangerMod.Instance.LogDebug($"Has {name}: {HasClearedLocation(name)}");
            }
        }

        public override void Hook()
        {
            On.PlayMakerFSM.OnEnable += AddItemTrackers;
            On.HealthManager.SendDeathEvent += TrackBossGeo;
            On.GeoRock.SetMyID += RenameDupeGeoRockIds;
        }

        public override void Unhook()
        {
            On.PlayMakerFSM.OnEnable -= AddItemTrackers;
            On.HealthManager.SendDeathEvent -= TrackBossGeo;
            On.GeoRock.SetMyID -= RenameDupeGeoRockIds;
        }

        internal static bool HasClearedLocation(string name)
        {
            if (trackingDefs.TryGetValue(name, out TrackingDef td))
            {
                if (td.PdBoolName is not null)
                {
                    return PlayerData.instance.GetBool(td.PdBoolName);
                }
                if (td.PdBoolNames is not null)
                {
                    return td.PdBoolNames.All(boolName => PlayerData.instance.GetBool(boolName));
                }
                if (td.PdIntName is not null)
                {
                    return PlayerData.instance.GetInt(td.PdIntName) >= td.PdIntValue;
                }
                AbstractLocation location = ItemChanger.Finder.GetLocation(name);
                if (location is not null)
                {
                    if (td.ObjectName is not null)
                    {
                        return HasObtainedItem(td.ObjectName, location.sceneName);
                    }
                    if (td.PdListName is not null)
                    {
                        return PlayerData.instance.GetVariable<List<string>>(td.PdListName).Contains(location.sceneName);
                    }
                }
                MapChangerMod.Instance.LogDebug($"Everything is null!");
            }
            return false;
        }

        internal static bool HasObtainedItem(string objectName, string scene)
        {
            return clearedLocations.Contains($"{objectName}-{scene}");
        }

        internal static void GetPreviouslyObtainedItems()
        {
            clearedLocations = new();

            foreach (GeoRockData grd in GameManager.instance.sceneData.geoRocks)
            {
                if (grd.hitsLeft > 0) continue;
                AddVanillaItem(grd.id, grd.sceneName);
            }
            foreach (PersistentBoolData pbd in GameManager.instance.sceneData.persistentBoolItems)
            {
                // Ignore SceneData added by ItemChanger
                if (!pbd.activated || pbd.id.Contains("-")) continue;

                if ((pbd.id.Contains("Shiny Item")
                    || pbd.id == "Heart Piece"
                    || pbd.id == "Vessel Fragment"
                    || pbd.id.Contains("Chest")
                    // Crystal/Enraged Guardian Boss Geo
                    || pbd.id == "Mega Zombie Beam Miner (1)"
                    || pbd.id == "Zombie Beam Miner Rematch"))
                {
                    AddVanillaItem(pbd.id, pbd.sceneName);
                }
                // Soul Warrior Sanctum Boss Geo
                else if (pbd.id == "Battle Scene v2" && pbd.sceneName == "Ruins1_23")
                {
                    AddVanillaItem("Mage Knight", pbd.sceneName);
                }
                // Soul Warrior Elegant Key Boss Geo
                else if (pbd.id == "Battle Scene v2" && pbd.sceneName == "Ruins1_31")
                {
                    AddVanillaItem("Mage Knight", $"{pbd.sceneName}b");
                }
                // Gruz Mother Boss Geo
                else if (pbd.id == "Battle Scene" && pbd.sceneName == "Crossroads_04")
                {
                    AddVanillaItem("Giant Fly", pbd.sceneName);
                }
            }
        }

        private static void TrackBossGeo(On.HealthManager.orig_SendDeathEvent orig, HealthManager self)
        {
            orig(self);

            switch (self.gameObject.name)
            {
                case "Mage Knight":
                case "Mega Zombie Beam Miner (1)":
                case "Zombie Beam Miner Rematch":
                case "Giant Fly":
                    AddVanillaItem(self.gameObject.name, Finder.CurrentScene()??"");
                    break;
                default:
                    break;
            }
        }

        private static void AddItemTrackers(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            string name = self.gameObject.name;
            FsmState state;

            if (self.FsmName == "Shiny Control")
            {
                if (!FsmUtil.TryGetState(self, "Finish", out state)) return;
            }
            else if (name == "Heart Piece" || name == "Vessel Fragment")
            {
                if (!FsmUtil.TryGetState(self, "Get", out state)) return;
            }
            else if (self.FsmName == "Chest Control")
            {
                if (!FsmUtil.TryGetState(self, "Open", out state)) return;
            }
            else if (name.Contains("Mimic") && self.FsmName == "Bottle Control")
            {
                if (!FsmUtil.TryGetState(self, "Shatter", out state)) return; 
            }
            else if (self.FsmName == "Geo Rock")
            {
                if (FsmUtil.TryGetState(self, "Destroy", out state))
                {
                    FsmUtil.AddAction(state, new TrackGeoRock(self.gameObject));
                    return;
                }
                return;
            }
            else
            {
                return;
            }

            FsmUtil.AddAction(state, new TrackItem(name));
        }

        private static void RenameDupeGeoRockIds(On.GeoRock.orig_SetMyID orig, GeoRock self)
        {
            orig(self);

            if (self.gameObject.scene.name == "Crossroads_ShamanTemple"
                && self.gameObject.name == "Geo Rock 2"
                && self.transform.parent != null)
            {
                self.geoRockData.id = "_Items/Geo Rock 2";
            }

            if (self.gameObject.scene.name == "Abyss_06_Core"
                && self.gameObject.name == "Geo Rock Abyss"
                && self.transform.parent != null)
            {
                self.geoRockData.id = "_Props/Geo Rock Abyss";
            }
        }

        private static void AddVanillaItem(string objectName, string scene)
        {
            clearedLocations.Add($"{objectName}-{scene}");
        }
    }
}