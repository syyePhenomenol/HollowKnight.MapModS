using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    internal class WhisperingRootPin : MapObject
    {
        public void Awake()
        {
            if (transform.parent.name == "Hive_02")
            {
                MapChangerMod.Instance.LogDebug("Fixing position of Hive_02 root pin");
                transform.localPosition = new Vector3(0.4f, -0.32f, -0.05f);
            }
        }

        public override void Set()
        {
            gameObject.SetActive((!Settings.MapModEnabled || !Settings.CurrentMode().DisableVanillaPins)
                && PlayerData.instance.GetBool("hasPinDreamPlant")
                && PlayerData.instance.GetVariable<List<string>>("scenesEncounteredDreamPlant").Contains(transform.parent.name)
                && !PlayerData.instance.GetVariable<List<string>>("scenesEncounteredDreamPlantC").Contains(transform.parent.name));
        }
    }
}
