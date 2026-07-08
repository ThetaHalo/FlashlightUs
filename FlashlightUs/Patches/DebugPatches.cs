using HarmonyLib;

namespace FlashlightUs.Patches;

// These patches exist because i test without any mod that has a no-game-end option, so ignore them. ty
[HarmonyPatch]
public static class DebugPatches
{
    [HarmonyPatch(typeof(LogicGameFlowHnS), nameof(LogicGameFlowHnS.CheckEndCriteria))]
    public class CheckEndGamePatchHNS
    {
        public static bool Prefix() => false;
        public static bool Prepare() => FlashlightUsPlugin.Debug;
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    class RpcEndGamePatch
    {
        public static bool Prefix() => false;
        public static bool Prepare() => FlashlightUsPlugin.Debug;
    }
    
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public class LogicGameFlowNormalPatch
    {
        public static bool Prefix() => false;
        public static bool Prepare() => FlashlightUsPlugin.Debug;
    }
}