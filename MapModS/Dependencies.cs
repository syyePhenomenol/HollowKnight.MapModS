using Modding;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapModS
{
    internal static class Dependencies
    {
        public static Dictionary<string, Assembly> strictDependencies = new()
        {
            { "RandomizerMod", null },
            { "RandomizerCore", null },
            { "ItemChanger", null },
            { "MenuChanger", null },
            { "ConnectionMetadataInjector", null },
            { "MagicUI", null },
            { "Vasi", null }
        };

        public static Dictionary<string, Assembly> optionalDependencies = new()
        {
            { "AdditionalMaps", null },
            { "Benchwarp", null },
            { "RandomizableLevers", null },
            { "RandoPlus", null }
        };

        public static void GetDependencies()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (strictDependencies.ContainsKey(assembly.GetName().Name))
                {
                    strictDependencies[assembly.GetName().Name] = assembly;
                }

                if (optionalDependencies.ContainsKey(assembly.GetName().Name))
                {
                    optionalDependencies[assembly.GetName().Name] = assembly;
                }
            }
        }

        public static bool HasDependency(string name)
        {
            return (strictDependencies.ContainsKey(name) && strictDependencies[name] != null)
                || (optionalDependencies.ContainsKey(name) && optionalDependencies[name] != null);
        }

        public static void BenchwarpInterop()
        {
            if (ModHooks.GetMod("Benchwarp", true) is Mod
                && optionalDependencies["Benchwarp"]?.GetType("Benchwarp.BenchKey") == null)
            {
                MapModS.Instance.LogWarn("Benchwarp is outdated");
                optionalDependencies["Benchwarp"] = null;
                return;
            }
        }
    }
}
