using MapChanger.UI;
using Modding;
using UnityEngine;

namespace MapChanger
{
    public abstract class MapMod : Mod, IMainHooks
    {
        protected abstract string[] Dependencies { get; }

        public abstract MapMode[] Modes { get; }

        protected abstract MainButton[] MainButtons { get; }

        protected abstract ExtraButtonPanel[] ExtraButtonPanels { get; }

        public sealed override void Initialize()
        {
            LogDebug($"Initializing {GetType().Name}");

            foreach (string dependency in Dependencies)
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
                Settings.AddModes(Modes);
                ImportLS();

                foreach (MainButton button in MainButtons)
                {
                    button.Make(PauseMenu.MainButtonsGrid);
                }

                foreach (ExtraButtonPanel ebp in ExtraButtonPanels)
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

        public virtual void ImportLS()
        {

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

        protected virtual void CreateMapObjects(GameObject goMap)
        {

        }
    }
}
