using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VentLib.Utilities;
using VentLib.Utilities.Optionals;
using Object = UnityEngine.Object;

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
        ModUpdateMenu = __instance.gameObject.AddComponent<ModUpdateMenu>();
        
        var playButton = __instance.playButton;
        
        PassiveButton updateButton = Object.Instantiate(playButton, __instance.transform);
        Async.Schedule(() =>
        {
            TextMeshPro tmp = updateButton.GetComponentInChildren<TextMeshPro>();
            tmp.text = "Update Found!";
            tmp.enableWordWrapping = true;
        }, 0.1f);
        updateButton.transform.localPosition += new Vector3(0f, 1.85f);
        updateButton.transform.localScale -= new Vector3(0f, 0.25f);
        //updateButton.GetComponentInChildren<ButtonRolloverHandler>().OutColor = ModConstants.Palette.GeneralColor5;
        //updateButton.GetComponentInChildren<SpriteRenderer>().color = ModConstants.Palette.GeneralColor5;
        Button.ButtonClickedEvent buttonClickedEvent = new();
        updateButton.GetComponentInChildren<PassiveButton>().OnClick = buttonClickedEvent;
        buttonClickedEvent.AddListener((UnityAction)(Action)(() => ModUpdateMenu.Open()));

        UpdateButton = UnityOptional<GameObject>.Of(updateButton.gameObject);

        if (!FlashlightUsPlugin.ModUpdater.HasUpdate) updateButton.gameObject.SetActive(false);
    }
}