using MapModS.Pins;
using System.Collections.Generic;

namespace MapModS
{
    public static class Finder
    {
        public static Dictionary<string, RandomizerModPinDef> RandomizerModPinDefs = new();

        public static RandomizerModPinDef GetRandomizerModPinDef(string name)
        {
            if (RandomizerModPinDefs.TryGetValue(name, out RandomizerModPinDef def))
            {
                return def;
            }
            MapModS.Instance.LogError($"Def not found! {name}");
            return null;
        }
    }
}
