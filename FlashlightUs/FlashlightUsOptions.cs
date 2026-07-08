using UnityEngine;
using VentLib.Options;
using VentLib.Options.UI;
using VentLib.Utilities.Attributes;

namespace FlashlightUs;

[LoadStatic]
public static class FlashlightUsOptions
{
    public static readonly OptionManager OptionManager =
        OptionManager.GetManager(file: "FlashlightUs.txt", managerFlags: OptionManagerFlags.SyncOverRpc | OptionManagerFlags.IgnorePreset);
    
    public static readonly OptionManager ClientOptionManager =
        OptionManager.GetManager(file: "FlashlightUs_Client.txt", managerFlags: OptionManagerFlags.IgnorePreset);
    
    public static GameOption EnableFlashlight;
    public static GameOption ForceFlashlight;
    public static GameOption KickUnmoddedPlayers;
    public static GameOption CrewmateFlashlightSize;
    public static GameOption ImpostorFlashlightSize;


    public static bool EnableFlashlightValue
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            EnableFlashlight.SetHardValue(value);

            if (LobbyBehaviour.Instance == null)
                Utilities.SendNotification($"The flashlight will be changed when you next enter the lobby.",
                    Utilities.LoadSprite("FlashlightUs.assets.logo.png", 500f));
        }
    }

    public static bool ForceFlashlightValue
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            ForceFlashlight.SetHardValue(value);
        }
    }

    public static bool KickUnmoddedPlayersValue
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            KickUnmoddedPlayers.SetHardValue(value);
        }
    }

    public static float CrewmateFlashlightSizeValue
    {
        get;
        set
        {
            if (Mathf.Approximately(field, value)) return;
            field = value;
            CrewmateFlashlightSize.SetHardValue(value);
        }
    }

    public static float ImpostorFlashlightSizeValue
    {
        get;
        set
        {
            if (Mathf.Approximately(field, value)) return;
            field = value;
            ImpostorFlashlightSize.SetHardValue(value);
        }
    }

    static FlashlightUsOptions()
    {
        EnableFlashlight = new GameOptionBuilder()
            .KeyName("Enable Flashlight", Translations.OptionsMenu.EnableFlashlight)
            .BindBool(b =>
            {
                EnableFlashlightValue = b;
                ClientOptionManager.DelaySave(0);
            })
            .AddBoolean()
            .BuildAndRegister(ClientOptionManager);

        GameOption header = new GameOptionTitleBuilder()
            .Title("FlashlightUs Settings")
            .Build();

        ForceFlashlight = new GameOptionBuilder()
            .KeyName("Force Flashlight for Clients", Translations.OptionsMenu.ForceFlashlight)
            .BindBool(b =>
            {
                ForceFlashlightValue = b;
                OptionManager.DelaySave(0);
            })
            .AddBoolean(false)
            .BuildAndRegister(OptionManager);

        KickUnmoddedPlayers = new GameOptionBuilder()
            .KeyName("Kick Unmodded Players", Translations.OptionsMenu.KickUnmoddedPlayers)
            .BindBool(b =>
            {
                KickUnmoddedPlayersValue = b;
                OptionManager.DelaySave(0);
            })
            .AddBoolean(false)
            .BuildAndRegister(OptionManager);

        CrewmateFlashlightSize = new GameOptionBuilder()
            .KeyName("Crewmate Flashlight Size", Translations.OptionsMenu.CrewmateFlashlightSize)
            .AddFloatRange(0.1f, 1f, 0.05f, defaultIndex: 5, suffix: "x")
            .BindFloat(f =>
            {
                AdjustCrewFlashlightSize(f);
                OptionManager.DelaySave(0);
            })
            .BuildAndRegister(OptionManager);

        ImpostorFlashlightSize = new GameOptionBuilder()
            .KeyName("Impostor Flashlight Size", Translations.OptionsMenu.ImpostorFlashlightSize)
            .AddFloatRange(0.1f, 1f, 0.05f, defaultIndex: 3, suffix: "x")
            .BindFloat(f =>
            {
                AdjustImpostorFlashlightSize(f);
                OptionManager.DelaySave(0);
            })
            .BuildAndRegister(OptionManager);
    }

    public static void AdjustCrewFlashlightSize(float f)
    {
        CrewmateFlashlightSizeValue = f;
    }

    public static void AdjustImpostorFlashlightSize(float f)
    {
        ImpostorFlashlightSizeValue = f;
    }
}