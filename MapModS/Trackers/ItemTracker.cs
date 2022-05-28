using HutongGames.PlayMaker;
using MapModS.Data;
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

        public static void Unhook()
        {
            On.HealthManager.SendDeathEvent -= HealthManager_SendDeathEvent;
            On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        }

        // Called after every time the map is opened
        public static void UpdateObtainedItems()
        {
            // The following is needed in case the mod is installed halfway through an existing save
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
                    // Crystal/Enraged Guardian Boss Geo
                    || pbd.id == "Mega Zombie Beam Miner (1)"
                    || pbd.id == "Zombie Beam Miner Rematch")
                    && !pbd.id.Contains("-"))
                {
                    MapModS.LS.ObtainedVanillaItems[pbd.id + pbd.sceneName] = true;
                }
                // Soul Warrior Sanctum Boss Geo
                else if (pbd.id == "Battle Scene v2" && pbd.sceneName == "Ruins1_23")
                {
                    MapModS.LS.ObtainedVanillaItems["Mage Knight" + pbd.sceneName] = true;
                }
                // Soul Warrior Elegant Key Boss Geo
                else if (pbd.id == "Battle Scene v2" && pbd.sceneName == "Ruins1_31")
                {
                    MapModS.LS.ObtainedVanillaItems["Mage Knight" + pbd.sceneName + "b"] = true;
                }
                // Gruz Mother Boss Geo
                else if (pbd.id == "Battle Scene" && pbd.sceneName == "Crossroads_04")
                {
                    MapModS.LS.ObtainedVanillaItems["Giant Fly" + pbd.sceneName] = true;
                }
            }
        }

        // For Boss Geo
        private static void HealthManager_SendDeathEvent(On.HealthManager.orig_SendDeathEvent orig, HealthManager self)
        {
            orig(self);

            if (RandomizerMod.RandomizerMod.RS.GenerationSettings == null) return;

            switch (self.gameObject.name)
            {
                case "Mage Knight":
                case "Mega Zombie Beam Miner (1)":
                case "Zombie Beam Miner Rematch":
                case "Giant Fly":
                    MapModS.LS.ObtainedVanillaItems[self.gameObject.name + Utils.CurrentScene()??""] = true;
                    break;
                default:
                    break;
            }
        }

        private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            string goName = self.gameObject.name;
            FsmState fsmState;

            // Most items: charms, charm notches, pale ore, rancid eggs, relics
            if (self.FsmName == "Shiny Control")
            {
                if (!FsmUtil.TryGetState(self, "Finish", out fsmState)) return;
            }

            // Mask/Vessel
            else if (goName == "Heart Piece" || goName == "Vessel Fragment")
            {
                if (!FsmUtil.TryGetState(self, "Get", out fsmState)) return;
            }

            // Geo Chests
            else if (self.FsmName == "Chest Control")
            {
                if (!FsmUtil.TryGetState(self, "Open", out fsmState)) return;
            }

            // Mimics
            else if (goName.Contains("Mimic") && self.FsmName == "Bottle Control")
            {
                if (!FsmUtil.TryGetState(self, "Shatter", out fsmState)) return; 
            }

            else
            {
                return;
            }

            FsmUtil.AddAction(fsmState, new TrackItem(goName));
        }
    }
}