using FlashlightUs.UI;
using HarmonyLib;
using UnityEngine;

namespace FlashlightUs.Patches;

[HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
class ModStampPatch
{
    public static void Prefix(ModManager __instance)
    {
        __instance.ShowModStamp();

        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (OptionsMenuItem.CustomBackground != null && OptionsMenuItem.CustomBackground.gameObject.activeSelf) OptionsMenuItem.CloseMenu();
            else OptionsMenuItem.OpenMenu();
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (LobbyBehaviour.Instance == null)
            {
                Utilities.SendNotification("You must be in the lobby to toggle the flashlight.", Utilities.LoadSprite("FlashlightUs.assets.logo.png", 500f));
                return;
            };
            FlashlightUsOptions.EnableFlashlightValue = !FlashlightUsOptions.EnableFlashlightValue;
            Utilities.SendNotification("Flashlight is now " + (FlashlightUsOptions.EnableFlashlightValue ? "enabled" : "disabled"), Utilities.LoadSprite("FlashlightUs.assets.logo.png", 500f));
        }
    }
}