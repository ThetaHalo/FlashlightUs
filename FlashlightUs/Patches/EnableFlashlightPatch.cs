using HarmonyLib;
using InnerNet;

namespace FlashlightUs.Patches;

[HarmonyPatch(typeof(PlayerControl), "IsFlashlightEnabled")]
public static class EnableFlashlightPatch
{
    public static bool Prefix(ref bool __result)
    {
        if (AmongUsClient.Instance.GameState is not InnerNetClient.GameStates.Started) return true;
        
        __result = FlashlightUsPlugin.Instance.EnableFlashlight.Value;
        return false;
    }
}