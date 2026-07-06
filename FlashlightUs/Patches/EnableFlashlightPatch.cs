using System;
using AmongUs.Data;
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

[HarmonyPatch(typeof(HudManager), "SetTouchType")] // fixes issue where the right joystick is not shown
public static class ForceRightJoystickPatch
{
    public static bool Prepare => OperatingSystem.IsAndroid();
    public static void Postfix(HudManager __instance, ControlTypes type)
    {
        if (!OperatingSystem.IsAndroid()) return;
        if (__instance.joystickR != null) return;

        bool shouldEnable = FlashlightUsOptions.EnableFlashlightValue || (FlashlightUsOptions.ForceFlashlightValue && !PlayerControl.LocalPlayer.IsHost());
        if (!shouldEnable) return;

        var instance = Object.Instantiate(__instance.RightVJoystick, __instance.transform, false);
        if (instance == null) return;

        __instance.joystickR = instance.GetComponent<VirtualJoystick>();
        __instance.joystickR.ToggleVisuals(LobbyBehaviour.Instance == null);
        __instance.SetJoystickSize(DataManager.Settings.Input.TouchJoystickSize);
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