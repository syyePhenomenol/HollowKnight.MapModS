using MapChanger.Map;
using Modding;
using System.Collections.Generic;
using System.Reflection;

namespace MapChanger
{
    public class MapChangerMod : Mod
    {
        internal static MapChangerMod Instance;
        public override string GetVersion() => "0.0.1";

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

            foreach (KeyValuePair<string, Assembly> pair in Dependencies.optionalDependencies)
            {
                if (pair.Value == null)
                {
                    Log($"{pair.Key} is not installed. Some features are disabled.");
                }
            }

            Finder.Load();
            SpriteManager.Load();
            Tracker.Load();
            BuiltInObjects.Load();
            VariableOverrides.Load();

            Events.Initialize();
        }
    }
}
