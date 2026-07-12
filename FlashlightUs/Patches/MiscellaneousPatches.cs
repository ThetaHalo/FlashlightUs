using System;
using FlashlightUs.UI;
using HarmonyLib;
using Twitch;
using UnityEngine;
using VentLib.Utilities.Harmony.Attributes;

namespace FlashlightUs.Patches;

[HarmonyPatch]
public class MiscellaneousPatches
{
    [QuickPostfix(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static void MainMenuManagerStart(MainMenuManager __instance)
    {
        if (!OptionsMenuItem.MenuButton) // start button, used for FlashlightUs mod options button
        {
            var button = Object.Instantiate(__instance.playButton, null, true);
            button.name = "FlashlightUsMenuButton";
            button.gameObject.SetActive(false);
            Object.DontDestroyOnLoad(button.gameObject);
            OptionsMenuItem.MenuButton = button;
        }

        // GenericPopup, used for options menu background
        if (!OptionsMenuItem.popup2 && TwitchManager.Instance != null && TwitchManager.Instance.TwitchPopup != null)
        {
            var popupTemplate = Object.Instantiate(TwitchManager.Instance.TwitchPopup, null, true);
            popupTemplate.name = "FlashlightUsPopup";
            popupTemplate.gameObject.SetActive(false);
            Object.DontDestroyOnLoad(popupTemplate.gameObject);
            OptionsMenuItem.popup2 = popupTemplate;
        }
    }
}

