using FlashlightUs.UI;
using HarmonyLib;

namespace FlashlightUs.Patches;

[HarmonyPatch(typeof(OptionsMenuBehaviour))]
public class ModOptionsMenuPatches
{
    private static OptionsMenuItem EnableFlashlight;
    private static OptionsMenuItem ForceFlashlight;
    private static OptionsMenuItem KickUnmoddedPlayers;
    private static OptionsMenuItem CrewmateFlashlightSize;
    private static OptionsMenuItem ImpostorFlashlightSize;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Start))]
    public static void CreateOptions(OptionsMenuBehaviour __instance)
    {
        if (!__instance.DisableMouseMovement) return;
        if (!__instance.MusicSlider) return;
        
        if (EnableFlashlight == null || !EnableFlashlight.ToggleButton)
        {
            EnableFlashlight = OptionsMenuItem.Create(
                "Enable Flashlight",
                "EnableFlashlight",
                () => FlashlightUsOptions.EnableFlashlightValue,
                v => FlashlightUsOptions.EnableFlashlightValue = v,
                __instance);
        }

        if (ForceFlashlight == null || !ForceFlashlight.ToggleButton)
        {
            ForceFlashlight = OptionsMenuItem.Create(
                "Force Flashlight for Clients",
                "ForceFlashlight",
                () => FlashlightUsOptions.ForceFlashlightValue,
                v => FlashlightUsOptions.ForceFlashlight.SetHardValue(v),
                __instance);
        }

        if (KickUnmoddedPlayers == null || !KickUnmoddedPlayers.ToggleButton)
        {
            KickUnmoddedPlayers = OptionsMenuItem.Create(
                "Kick Unmodded Players",
                "KickUnmoddedPlayers",
                () => FlashlightUsOptions.KickUnmoddedPlayersValue,
                v => FlashlightUsOptions.KickUnmoddedPlayers.SetHardValue(v),
                __instance);
        }

        // sliders
        if (CrewmateFlashlightSize == null || !CrewmateFlashlightSize.SlideBar)
        {
            CrewmateFlashlightSize = OptionsMenuItem.CreateSlider(
                "Crewmate Flashlight Size",
                "CrewmateFlashlightSize",
                () => FlashlightUsOptions.CrewmateFlashlightSizeValue,
                v => FlashlightUsOptions.CrewmateFlashlightSize.SetHardValue(v),
                new FloatRange(0.1f, 1f),
                __instance,
                snapStep: 0.05f);
        }

        if (ImpostorFlashlightSize == null || !ImpostorFlashlightSize.SlideBar)
        {
            ImpostorFlashlightSize = OptionsMenuItem.CreateSlider(
                "Impostor Flashlight Size",
                "ImpostorFlashlightSize",
                () => FlashlightUsOptions.ImpostorFlashlightSizeValue,
                v => FlashlightUsOptions.ImpostorFlashlightSize.SetHardValue(v),
                new FloatRange(0.1f, 1f),
                __instance,
                snapStep: 0.05f);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Open))]
    public static void RefreshOptions(OptionsMenuBehaviour __instance)
    {
        EnableFlashlight?.UpdateToggle();
        ForceFlashlight?.UpdateToggle();
        KickUnmoddedPlayers?.UpdateToggle();
        CrewmateFlashlightSize?.UpdateSlider();
        ImpostorFlashlightSize?.UpdateSlider();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Close))]
    public static void OnClose(OptionsMenuBehaviour __instance)
    {
        OptionsMenuItem.CustomBackground?.gameObject.SetActive(false);
    }
}