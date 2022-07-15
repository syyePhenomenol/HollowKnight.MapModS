using System.Collections.Generic;

namespace MapChanger.MonoBehaviours
{
    internal class LifebloodPin : MapObject
    {
        public override void Set()
        {
            gameObject.SetActive((!Settings.MapModEnabled || !Settings.CurrentMode().DisableVanillaPins)
                && PlayerData.instance.GetBool("hasPinCocoon")
                && PlayerData.instance.GetVariable<List<string>>("scenesEncounteredCocoon").Contains(transform.parent.name));
        }
    }
}
