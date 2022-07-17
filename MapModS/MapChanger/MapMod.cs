using Modding;
using UnityEngine;

namespace MapChanger
{
    public abstract class MapMod : Mod
    {
        private readonly string[] dependencies;

        private readonly MapModeSetting[] modes;

        protected MapMod(string[] dependencies, MapModeSetting[] modes)
        {
            this.dependencies = dependencies;
            this.modes = modes;
        }

        public sealed override void Initialize()
        {
            LogDebug($"Initializing {GetType()}");

            foreach (string dependency in dependencies)
            {
                if (ModHooks.GetMod(dependency) is not Mod)
                {
                    MapChangerMod.Instance.LogWarn($"Dependency not found for {GetType()}: {dependency}");
                    return;
                }
            }

            LoadGlobalData();

            Events.AfterEnterGame += TryActivateMod;

            LogDebug($"Initialization for {GetType()} complete.");
        }

        private void TryActivateMod()
        {
            if (ActiveCondition())
            {
                Settings.AddModes(modes);
                Events.AfterSetGameMap += CreateMapObjects;
            }
        }

        protected virtual bool ActiveCondition()
        {
            return true;
        }

        protected virtual void LoadGlobalData()
        {

        }

        protected abstract void CreateMapObjects(GameObject goMap);
    }
}
