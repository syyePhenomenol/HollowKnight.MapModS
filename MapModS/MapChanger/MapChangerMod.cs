﻿using System.Collections.Generic;
using System.Reflection;
using MapChanger.Map;
using Modding;

namespace MapChanger
{
    public class MapChangerMod : Mod, ILocalSettings<Settings>
    {
        internal static MapChangerMod Instance;
        public override string GetVersion() => "MC PRERELEASE 5";
        public void OnLoadLocal(Settings ls) => Settings.Instance = ls;
        public Settings OnSaveLocal() => Settings.Instance;

        public override void Initialize()
        {
            Instance = this;

            Dependencies.GetDependencies();

            foreach (KeyValuePair<string, Assembly> pair in Dependencies.strictDependencies)
            {
                if (pair.Value == null)
                {
                    Log($"{pair.Key} is not installed. MapChangerMod disabled");
                    return;
                }
            }
            
            Finder.Load();
            Tracker.Load();
            BuiltInObjects.Load();
            VariableOverrides.Load();

            Events.Initialize();
        }
    }
}
