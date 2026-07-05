using FlashlightUs.UI;
using HarmonyLib;

namespace FlashlightUs.Patches;

[HarmonyPatch(typeof(OptionsMenuBehaviour))]
public class OptionsMenuPatches
{
    private static OptionsMenuItem EnableFlashlight;
    private static OptionsMenuItem ForceFlashlight;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Start))]
    public static void CreateOptions(OptionsMenuBehaviour __instance)
    {
        if (!__instance.DisableMouseMovement) return;

        if (EnableFlashlight == null || !EnableFlashlight.ToggleButton)
        {
            EnableFlashlight = OptionsMenuItem.Create(Translations.OptionsMenu.EnableFlashlight, "EnableFlashlight", FlashlightUsPlugin.Instance.EnableFlashlight, __instance);
        }
        if (ForceFlashlight == null || !ForceFlashlight.ToggleButton)
        {
            ForceFlashlight = OptionsMenuItem.Create(Translations.OptionsMenu.ForceFlashlight, "ForceFlashlight", FlashlightUsPlugin.Instance.ForceFlashlight, __instance);
        }
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Close))]
    public static void OnClose(OptionsMenuBehaviour __instance)
    {
        OptionsMenuItem.CustomBackground?.gameObject.SetActive(false);
    }
}