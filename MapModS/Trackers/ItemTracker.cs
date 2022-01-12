using Modding;
using Vasi;

namespace MapModS.Trackers
{
    public static class ItemTracker
    {
        public static void Hook()
        {
            On.HealthManager.SendDeathEvent += HealthManager_SendDeathEvent;
            On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        }

        private static void HealthManager_SendDeathEvent(On.HealthManager.orig_SendDeathEvent orig, HealthManager self)
        {
            orig(self);

            switch (self.gameObject.name)
            {
                case "Mage Knight":
                case "Mega Zombie Beam Miner (1)":
                case "Zombie Beam Miner Rematch":
                case "Giant Fly":
                    MapModS.LS.ObtainedVanillaItems[self.gameObject.name + GameManager.instance.sceneName] = true;
                    break;
                default:
                    break;
            }

            //MapModS.Instance.Log($"{self.gameObject.name} died");
        }

        // Called after every time the map is opened
        public static void UpdateObtainedItems()
        {
            // The following is needed in case the mod is installed halfway through an existing vanilla save
            foreach (GeoRockData grd in GameManager.instance.sceneData.geoRocks)
            {
                if (grd.hitsLeft == 0)
                {
                    MapModS.LS.ObtainedVanillaItems[grd.id + grd.sceneName] = true;
                }
            }

            foreach (PersistentBoolData pbd in GameManager.instance.sceneData.persistentBoolItems)
            {
                if (!pbd.activated) continue;

                if ((pbd.id.Contains("Shiny Item")
                    || pbd.id == "Heart Piece"
                    || pbd.id == "Vessel Fragment"
                    || pbd.id.Contains("Chest")
                    // Crystal/Enraged Guardian
                    || pbd.id == "Mega Zombie Beam Miner (1)"
                    || pbd.id == "Zombie Beam Miner Rematch")
                    && !pbd.id.Contains("-"))
                {
                    MapModS.LS.ObtainedVanillaItems[pbd.id + pbd.sceneName] = true;
                }
                // Soul Warrior Sanctum
                else if (pbd.id == "Battle Scene v2" && pbd.sceneName == "Ruins1_23")
                {
                    MapModS.LS.ObtainedVanillaItems["Mage Knight" + pbd.sceneName] = true;
                }
                // Soul Warrior Elegant Key
                else if (pbd.id == "Battle Scene v2" && pbd.sceneName == "Ruins1_31")
                {
                    MapModS.LS.ObtainedVanillaItems["Mage Knight" + pbd.sceneName + "b"] = true;
                }
                // Gruz Mother
                else if (pbd.id == "Battle Scene" && pbd.sceneName == "Crossroads_04")
                {
                    MapModS.LS.ObtainedVanillaItems["Giant Fly" + pbd.sceneName] = true;
                }
            }
        }

        // sceneData doesn't immediately get updated when items are picked up,
        // therefore we need to use the item FSMs to determine this
        private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            string goName = self.gameObject.name;

            // Most items: charms, charm notches, pale ore, rancid eggs, relics
            if (self.FsmName == "Shiny Control")
            {
                FsmUtil.AddAction(FsmUtil.GetState(self, "Finish"), new TrackItem(goName));
            }

            // Mask/Vessel
            else if (goName == "Heart Piece"
                || goName == "Vessel Fragment")
            {
                FsmUtil.AddAction(FsmUtil.GetState(self, "Get"), new TrackItem(goName));
            }

            // Geo Chests
            else if (goName.Contains("Chest"))
            {
                FsmUtil.AddAction(FsmUtil.GetState(self, "Open"), new TrackItem(goName));
            }

            // Mimics
            else if (goName.Contains("Mimic") && self.FsmName == "Bottle Control")
            {
                FsmUtil.AddAction(FsmUtil.GetState(self, "Shatter"), new TrackItem(goName));
            }
        }
    }
}