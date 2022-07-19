using MapChanger.UI;
using Modding;
using UnityEngine;

namespace MapChanger
{
    public abstract class MapMod : Mod, IMainHooks
    {
        private readonly string[] dependencies;

        private readonly MapModeSetting[] modes;

        private readonly MainButton[] mainButtons;

        private readonly ExtraButtonPanel[] extraButtonPanels;

        protected MapMod(string[] dependencies, MapModeSetting[] modes, MainButton[] mainButtons, ExtraButtonPanel[] extraButtonPanels)
        {
            this.dependencies = dependencies;
            this.modes = modes;
            this.mainButtons = mainButtons;
            this.extraButtonPanels = extraButtonPanels;
        }

        public sealed override void Initialize()
        {
            LogDebug($"Initializing {GetType().Name}");

            foreach (string dependency in dependencies)
            {
                if (ModHooks.GetMod(dependency) is not Mod)
                {
                    MapChangerMod.Instance.LogWarn($"Dependency not found for {GetType().Name}: {dependency}");
                    return;
                }
            }

            LoadGlobalData();

            LogDebug($"Initialization for {GetType().Name} complete.");
        }

        public void OnEnterGame()
        {
            if (ActivateCondition())
            {
                Settings.AddModes(modes);

                foreach (MainButton button in mainButtons)
                {
                    button.Make();
                }

                foreach (ExtraButtonPanel ebp in extraButtonPanels)
                {
                    ebp.Make();
                }

                Events.AfterSetGameMap += CreateMapObjects;
            }
        }

        public void OnQuitToMenu()
        {
            Events.AfterSetGameMap -= CreateMapObjects;
        }

        protected virtual void LoadGlobalData()
        {

        }

        protected virtual bool ActivateCondition()
        {
            return true;
        }


        protected virtual void OnSettingChanged()
        {

        }

        protected virtual void AddUI()
        {

        }

        protected abstract void CreateMapObjects(GameObject goMap);
    }
}
