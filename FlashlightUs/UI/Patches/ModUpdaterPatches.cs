using System;
using HarmonyLib;
using TMPro;
using Twitch;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VentLib.Utilities;
using VentLib.Utilities.Optionals;

namespace FlashlightUs.UI.Patches;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public class ModUpdaterPatches
{
    public static ModUpdateMenu ModUpdateMenu;
    internal static UnityOptional<GameObject> UpdateButton = UnityOptional<GameObject>.Null();

    public static bool IsReady;
    
    // based on portions of https://github.com/Lotus-AU/LotusContinued/blob/8a00694ae082339c9abad299945ce1f1522be74c/src/GUI/Patches/SplashPatch.cs
    public static void Prefix(MainMenuManager __instance)
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
        
        ModUpdateMenu = __instance.gameObject.AddComponent<ModUpdateMenu>();
        
        var UpdateAvailableText = new GameObject("FlashlightUs_UpdateAvailable");
        var pos = new Vector3(8.8f, -2f, 0);

        var text = OperatingSystem.IsAndroid()
            ? Translations.ModUpdater.StarlightUpdateAvailable
            : Translations.ModUpdater.PCUpdateAvailable;

        UpdateAvailableText.transform.position = pos;
        
        var tmp = UpdateAvailableText.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.horizontalAlignment = HorizontalAlignmentOptions.Left;
        tmp.text = text;
        tmp.color = Color.yellow;
        tmp.fontSize = 2.25f;
        
        if (!FlashlightUsPlugin.ModUpdater.HasUpdate) UpdateAvailableText.gameObject.SetActive(false);
    }
}