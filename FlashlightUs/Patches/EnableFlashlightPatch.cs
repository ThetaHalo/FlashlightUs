using HarmonyLib;
using UnityEngine;
using VentLib.Utilities.Extensions;

namespace FlashlightUs.Patches;

[HarmonyPatch(typeof(PlayerControl), "IsFlashlightEnabled")]
public static class EnableFlashlightPatch
{
    public static bool Prefix(ref bool __result)
    {
        if (LobbyBehaviour.Instance != null) return true;
        var isEnabled = FlashlightUsOptions.EnableFlashlightValue || (FlashlightUsOptions.ForceFlashlightValue && !PlayerControl.LocalPlayer.IsHost());

        __result = isEnabled;
        return false;
    }
}

// trying to change HNS options will result in a nullref, so we patch this instead to properly change it.
[HarmonyPatch(typeof(LightSource), "SetupLightingForGameplay")]
public static class SetupLightingPatch
{
    public static void Prefix(LightSource __instance, ref float flashlightSize, Transform touchFlashlightTarget)
    {
        var pc = __instance.GetComponentInParent<PlayerControl>();
        if (pc == null) return;

        bool isImpostor = pc.Data?.Role?.IsImpostor ?? false;
        flashlightSize = isImpostor
            ? FlashlightUsOptions.ImpostorFlashlightSizeValue
            : FlashlightUsOptions.CrewmateFlashlightSizeValue;
    }
}