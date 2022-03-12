using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Modding;
using System.Collections;

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
            { "Vasi", null }
        };

        public static Dictionary<string, Assembly> optionalDependencies = new()
        {
            { "AdditionalMaps", null },
            { "Benchwarp", null },
            { "RandomizableLevers", null }
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
            return (strictDependencies.GetValueOrDefault(name) != null
                || optionalDependencies.GetValueOrDefault(name) != null);
        }

        // Taken from BadMagic's BenchwarpInterop
        // MIT License

        // Copyright(c) 2022 BadMagic100

        // Permission is hereby granted, free of charge, to any person obtaining a copy
        // of this software and associated documentation files(the "Software"), to deal
        // in the Software without restriction, including without limitation the rights
        // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        // copies of the Software, and to permit persons to whom the Software is
        // furnished to do so, subject to the following conditions:

        // The above copyright notice and this permission notice shall be included in all
        // copies or substantial portions of the Software.

        static Type? bwBenchKey;
        static PropertyInfo? bwBenchKey_SceneName;
        static object? bwLocalSettings_Instance;
        static FieldInfo? bwLocalSettings_VisitedBenchScenes;
        static IEnumerable bwLocalSettings_VisitedBenchScenes_Instance;
        static bool isOldBenchwarp = false;

        public static void BenchwarpInterop()
        {
            if (ModHooks.GetMod("Benchwarp", true) is Mod bw)
            {
                bwLocalSettings_Instance = bw.GetType().GetProperty("LS").GetValue(bw);

                Type? bwLocalSettingsType = bwLocalSettings_Instance?.GetType();

                bwLocalSettings_VisitedBenchScenes = bwLocalSettingsType?.GetField("visitedBenchScenes");

                Type? vbsType = bwLocalSettings_VisitedBenchScenes.GetType();

                bwBenchKey = optionalDependencies["Benchwarp"]?.GetType("Benchwarp.BenchKey");

                if (bwBenchKey == null)
                {
                    MapModS.Instance?.LogWarn("Benchwarp is outdated");
                    isOldBenchwarp = true;
                    return;
                }

                bwBenchKey_SceneName = bwBenchKey.GetProperty("SceneName");

                if (bwBenchKey == null
                    || bwBenchKey_SceneName == null
                    || bwLocalSettings_Instance == null
                    || bwLocalSettingsType == null
                    || bwLocalSettings_VisitedBenchScenes == null)
                {
                    MapModS.Instance?.LogError("Found benchwarp installed, but couldn't get necessary interop info");
                }
            }
        }

        public static IEnumerable<string> GetVisitedBenchScenes()
        {
            if (!HasDependency("Benchwarp")) return Enumerable.Empty<string>();

            bwLocalSettings_VisitedBenchScenes_Instance = (IEnumerable) bwLocalSettings_VisitedBenchScenes.GetValue(bwLocalSettings_Instance);

            if (isOldBenchwarp)
            {
                return bwLocalSettings_VisitedBenchScenes_Instance.Cast<KeyValuePair<string, bool>>().Where(s => s.Value).Select(s => s.Key);
            }

            return bwLocalSettings_VisitedBenchScenes_Instance.Cast<object>().Select(s => (string)bwBenchKey_SceneName.GetValue(s));
        }
    }
}
