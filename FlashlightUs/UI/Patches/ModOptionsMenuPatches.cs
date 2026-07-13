using HarmonyLib;
using VentLib.Utilities.Harmony.Attributes;

namespace FlashlightUs.UI.Patches;

[HarmonyPatch(typeof(OptionsMenuBehaviour))]
public class ModOptionsMenuPatches
{
    private static OptionsMenuItem EnableFlashlight;
    private static OptionsMenuItem ForceFlashlight;
    private static OptionsMenuItem EnableFlashlightInLobby;
    private static OptionsMenuItem KickUnmoddedPlayers;
    private static OptionsMenuItem CrewmateFlashlightSize;
    private static OptionsMenuItem ImpostorFlashlightSize;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Start))]
    public static void CreateOptions(OptionsMenuBehaviour __instance)
    {
        if (!__instance.DisableMouseMovement) return;
        if (!__instance.MusicSlider) return;

        ActuallyCreateOptions(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Open))]
    public static void RefreshOptions(OptionsMenuBehaviour __instance)
    {
        StaticLogger.Info("Refreshing options");
        if (OptionsMenuItem.modOptionsButtonV2 != null) OptionsMenuItem.modOptionsButtonV2.gameObject.SetActive(true);
        ActuallyCreateOptions(__instance);
        OptionsMenuItem.TryCreateModOptionsButton(__instance); // this cannot be removed or else the button will disappear on scene change D:
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
        OptionsMenuItem.OptionsBaseMenu?.gameObject.SetActive(false);
    }

    private static void ActuallyCreateOptions(OptionsMenuBehaviour __instance)
    {
        if (EnableFlashlight == null || !EnableFlashlight.ToggleButton)
        {
            EnableFlashlight = OptionsMenuItem.Create(
                Translations.OptionsMenu.EnableFlashlight,
                "EnableFlashlight",
                () => FlashlightUsOptions.EnableFlashlightValue,
                v => FlashlightUsOptions.EnableFlashlightValue = v,
                __instance);
        }



        if (ForceFlashlight == null || !ForceFlashlight.ToggleButton)
        {
            ForceFlashlight = OptionsMenuItem.Create(
                Translations.OptionsMenu.ForceFlashlight,
                "ForceFlashlight",
                () => FlashlightUsOptions.ForceFlashlightValue,
                v => FlashlightUsOptions.ForceFlashlight.SetHardValue(v),
                __instance);
        }

        if (EnableFlashlightInLobby == null || !EnableFlashlightInLobby.ToggleButton)
        {
            EnableFlashlightInLobby = OptionsMenuItem.Create(
                Translations.OptionsMenu.EnableFlashlightInLobby,
                "EnableFlashlightInLobby",
                () => FlashlightUsOptions.EnableFlashlightInLobbyValue,
                v => FlashlightUsOptions.EnableFlashlightInLobby.SetHardValue(v),
                __instance);
        }

    if (KickUnmoddedPlayers == null || !KickUnmoddedPlayers.ToggleButton)
        {
            KickUnmoddedPlayers = OptionsMenuItem.Create(
                Translations.OptionsMenu.KickUnmoddedPlayers,
                "KickUnmoddedPlayers",
                () => FlashlightUsOptions.KickUnmoddedPlayersValue,
                v => FlashlightUsOptions.KickUnmoddedPlayers.SetHardValue(v),
                __instance);
        }

        // sliders
        if (CrewmateFlashlightSize == null || !CrewmateFlashlightSize.SlideBar)
        {
            CrewmateFlashlightSize = OptionsMenuItem.CreateSlider(
                Translations.OptionsMenu.CrewmateFlashlightSize,
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
                Translations.OptionsMenu.ImpostorFlashlightSize,
                "ImpostorFlashlightSize",
                () => FlashlightUsOptions.ImpostorFlashlightSizeValue,
                v => FlashlightUsOptions.ImpostorFlashlightSize.SetHardValue(v),
                new FloatRange(0.1f, 1f),
                __instance,
                snapStep: 0.05f);
        }
    }
}