using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapChanger.Map
{
    /// <summary>
    /// Replaces the name of variables, so we can override their values
    /// to enable enable full map or disable vanilla pins with the mod enabled.
    /// </summary>
    internal class VariableOverrides : HookModule
    {
        private enum OverrideType
        {
            Map,
            Pins
        }

        private record FsmBoolOverrideDef
        {
            [JsonProperty]
            internal Dictionary<string, FsmActionBoolOverride[]> BoolsIndex { get; init; }
            [JsonProperty]
            internal Dictionary<string, FsmActionBoolRangeOverride> BoolsRange { get; init; }
        }

        private record FsmActionBoolOverride
        {
            [JsonProperty]
            internal int Index { get; init; }

            [JsonProperty]
            internal OverrideType Type { get; init; }
        }

        private record FsmActionBoolRangeOverride
        {
            [JsonProperty]
            internal int Range { get; init; }

            [JsonProperty]
            internal OverrideType Type { get; init; }
        }

        private const string MAP_PREFIX = "MCO0";
        private const string PINS_PREFIX = "MCO1";
        private const string HAS_MAP = "hasMap";

        private static readonly Dictionary<string, string> ilVariables = new()
        {
            { "hasQuill", MAP_PREFIX },
            { "mapAllRooms", MAP_PREFIX },
            { "mapAbyss", MAP_PREFIX },
            { "mapCity", MAP_PREFIX },
            { "mapCliffs", MAP_PREFIX },
            { "mapCrossroads", MAP_PREFIX },
            { "mapMines", MAP_PREFIX },
            { "mapDeepnest", MAP_PREFIX },
            { "mapFogCanyon", MAP_PREFIX },
            { "mapFungalWastes", MAP_PREFIX },
            { "mapGreenpath", MAP_PREFIX },
            { "mapOutskirts", MAP_PREFIX },
            { "mapRoyalGardens", MAP_PREFIX },
            { "mapRestingGrounds", MAP_PREFIX },
            { "mapWaterways", MAP_PREFIX },
            { "scenesEncounteredCocoon", PINS_PREFIX },
            { "scenesEncounteredDreamPlant", PINS_PREFIX },
            { "scenesEncounteredDreamPlantC", PINS_PREFIX }
        };

        private static Dictionary<string, FsmBoolOverrideDef> fsmOverrideDefs;

        internal static void Load()
        {
            fsmOverrideDefs = JsonUtil.Deserialize<Dictionary<string, FsmBoolOverrideDef>>("MapModS.MapChanger.Resources.fsmOverrides.json");
        }

        public override void Hook()
        {
            On.PlayMakerFSM.OnEnable += ReplaceVariablesFSM;

            IL.GameMap.WorldMap += ReplaceVariablesIL;
            IL.GameMap.SetupMap += ReplaceVariablesIL;
            IL.RoughMapRoom.OnEnable += ReplaceVariablesIL;

            ModHooks.GetPlayerBoolHook += BoolGetOverride;
            ModHooks.GetPlayerVariableHook += ModHooks_GetPlayerVariableHook;
        }

        public override void Unhook()
        {
            On.PlayMakerFSM.OnEnable -= ReplaceVariablesFSM;

            IL.GameMap.WorldMap -= ReplaceVariablesIL;
            IL.GameMap.SetupMap -= ReplaceVariablesIL;
            IL.RoughMapRoom.OnEnable -= ReplaceVariablesIL;

            ModHooks.GetPlayerBoolHook -= BoolGetOverride;
            ModHooks.GetPlayerVariableHook -= ModHooks_GetPlayerVariableHook;
        }

        private static void ReplaceVariablesFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (fsmOverrideDefs.TryGetValue(self.FsmName, out FsmBoolOverrideDef fod)
                || fsmOverrideDefs.TryGetValue(self.name + "-" + self.FsmName, out fod))
            {
                foreach (FsmState state in self.FsmStates)
                {
                    //MapModS.Instance.LogDebug($"Trying to replace bools in {state.Name} of {self.FsmName}");
                    if (fod.BoolsIndex.TryGetValue(state.Name, out FsmActionBoolOverride[] overrides))
                    {
                        foreach(FsmActionBoolOverride boolOverride in overrides)
                        {
                            ReplaceBool(state, boolOverride.Index, boolOverride.Type);
                        }
                    }
                    if (fod.BoolsRange.TryGetValue(state.Name, out FsmActionBoolRangeOverride overrideRange))
                    {
                        for (int i = 0; i < overrideRange.Range; i++)
                        {
                            ReplaceBool(state, i, overrideRange.Type);
                        }
                    }
                }
            }

            static void ReplaceBool(FsmState state, int index, OverrideType type)
            {
                FsmStateAction action = state.Actions[index];
                if (action.GetType() == typeof(PlayerDataBoolTest))
                {
                    PlayerDataBoolTest boolTest = (PlayerDataBoolTest)action;
                    boolTest.boolName = NewBoolName(boolTest.boolName, type);
                    return;
                }
                if (action.GetType() == typeof(GetPlayerDataBool))
                {
                    GetPlayerDataBool getBool = (GetPlayerDataBool)action;
                    getBool.boolName = NewBoolName(getBool.boolName, type);
                    return;
                }
                if (action.GetType() == typeof(PlayerDataBoolAllTrue))
                {
                    PlayerDataBoolAllTrue pdat = (PlayerDataBoolAllTrue)action;
                    pdat.stringVariables = pdat.stringVariables.Select(boolName => NewBoolName(boolName, type)).ToArray();
                    return;
                }
                MapChangerMod.Instance.LogWarn($"Unrecognized FsmAction: {state.Name}, {index}");
                return;
            }

            static FsmString NewBoolName(FsmString name, OverrideType type)
            {
                if (name.ToString() == ""
                    || name.ToString().StartsWith(MAP_PREFIX)
                    || name.ToString().StartsWith(PINS_PREFIX))
                {
                    return name;
                }
                MapChangerMod.Instance.LogDebug($"Replacement of {name} is successful");
                return type switch
                {
                    OverrideType.Map => MAP_PREFIX + name,
                    OverrideType.Pins => PINS_PREFIX + name,
                    _ => name
                };
            }
        }
        
        private static void ReplaceVariablesIL(ILContext il)
        {
            ILCursor cursor = new(il);
            while (cursor.TryGotoNext(instr => instr.OpCode == OpCodes.Ldstr && ilVariables.ContainsKey((string)instr.Operand)))
            {
                string name = cursor.ToString().Split('\"')[1];
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, ilVariables[name] + name);
            }
        }

        private static bool BoolGetOverride(string name, bool orig)
        {
            if (!name.StartsWith(MAP_PREFIX) && !name.StartsWith(PINS_PREFIX))
            {
                return orig;
            }
            if (!Settings.MapModEnabled)
            {
                return GetOriginalBool(name);
            }
            if (name == MAP_PREFIX + HAS_MAP)
            {
                return true;
            }
            if (name.StartsWith(MAP_PREFIX))
            {
                if (Settings.ForceFullMap)
                {
                    return GetOriginalBool(name);
                }
                return true;
            }
            if (name.StartsWith(PINS_PREFIX))
            {
                return false;
            }
            return orig;

            static bool GetOriginalBool(string name)
            {
                return PlayerData.instance.GetBool(name.Remove(0, MAP_PREFIX.Length));
            }
        }

        private static object ModHooks_GetPlayerVariableHook(Type type, string name, object value)
        {
            if (!name.StartsWith(MAP_PREFIX) && !name.StartsWith(PINS_PREFIX))
            {
                return value;
            }
            if (!Settings.MapModEnabled)
            {
                return GetOriginalVariable<List<string>>(name);
            }
            if (name.StartsWith(PINS_PREFIX) && type == typeof(List<string>))
            {
                return new List<string> { };
            }
            return value;

            static object GetOriginalVariable<T>(string name)
            {
                return PlayerData.instance.GetVariable<T>(name.Remove(0, MAP_PREFIX.Length));
            }
        }
    }
}
