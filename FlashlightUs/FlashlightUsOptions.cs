using VentLib.Options;
using VentLib.Options.UI;
using VentLib.Options.UI.Tabs;
using VentLib.Utilities.Attributes;

namespace FlashlightUs;

[LoadStatic]
public static class FlashlightUsOptions
{
    public static readonly OptionManager OptionManager =
        OptionManager.GetManager(file: "FlashlightUs.txt", managerFlags: OptionManagerFlags.SyncOverRpc | OptionManagerFlags.IgnorePreset);
    
    public static readonly OptionManager ClientOptionManager =
        OptionManager.GetManager(file: "FlashlightUs_Client.txt", managerFlags: OptionManagerFlags.IgnorePreset);

    public static MainSettingTab Tab;

    public static GameOption EnableFlashlightMessage;
    public static GameOption EnableFlashlight;
    public static GameOption ForceFlashlight;
    public static GameOption KickUnmoddedPlayers;
    public static GameOption CrewmateFlashlightSize;
    public static GameOption ImpostorFlashlightSize;


    public static bool EnableFlashlightValue
    {
        get => _enableFlashlight;
        set
        {
            if (_enableFlashlight == value) return;
            _enableFlashlight = value;
            EnableFlashlight.SetHardValue(value);
            
            if (LobbyBehaviour.Instance == null) Utilities.SendNotification($"The flashlight will be changed when you next enter the lobby.", Utilities.LoadSprite("FlashlightUs.assets.logo.png", 500f));
        }
    }
    
    private static bool _enableFlashlight;
    public static bool ForceFlashlightValue;
    public static bool KickUnmoddedPlayersValue;
    public static float CrewmateFlashlightSizeValue;
    public static float ImpostorFlashlightSizeValue;

    static FlashlightUsOptions()
    {
        //Tab = new MainSettingTab("FlashlightUs", Translations.OptionsMenu.Description);
        //SettingsOptionController.SetMainTab(Tab);
        //SettingsOptionController.Enable();

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
            .BindBool(b => ForceFlashlightValue = b)
            .AddBoolean(false)
            .BuildAndRegister(OptionManager);
        
        KickUnmoddedPlayers = new GameOptionBuilder()
            .KeyName("Kick Unmodded Players", Translations.OptionsMenu.KickUnmoddedPlayers)
            .BindBool(b => KickUnmoddedPlayersValue = b)
            .AddBoolean(false)
            .BuildAndRegister(OptionManager);

        CrewmateFlashlightSize = new GameOptionBuilder()
            .KeyName("Crewmate Flashlight Size", Translations.OptionsMenu.CrewmateFlashlightSize)
            .AddFloatRange(0.1f, 1f, 0.05f, defaultIndex: 5, suffix: "x")
            .BindFloat(AdjustCrewFlashlightSize)
            .BuildAndRegister(OptionManager);

        ImpostorFlashlightSize = new GameOptionBuilder()
            .KeyName("Impostor Flashlight Size", Translations.OptionsMenu.ImpostorFlashlightSize)
            .AddFloatRange(0.1f, 1f, 0.05f, defaultIndex: 3, suffix: "x")
            .BindFloat(AdjustImpostorFlashlightSize)
            .BuildAndRegister(OptionManager);
        
        EnableFlashlightMessage = new GameOptionTitleBuilder()
            .Title(Translations.OptionsMenu.ChangeInOptions)
            .IsHeader(true)
            .Build();

        /*Tab.AddOption(header);
        Tab.AddOption(ForceFlashlight);
        Tab.AddOption(KickUnmoddedPlayers);
        Tab.AddOption(CrewmateFlashlightSize);
        Tab.AddOption(ImpostorFlashlightSize);
        Tab.AddOption(EnableFlashlightMessage);*/
    }

    public static void AdjustCrewFlashlightSize(float f)
    {
        //GameOptionsManager.Instance.currentHideNSeekGameOptions.SetFloat(FloatOptionNames.CrewmateFlashlightSize, f);
        CrewmateFlashlightSizeValue = f;
    }

    public static void AdjustImpostorFlashlightSize(float f)
    {
        //GameOptionsManager.Instance.currentHideNSeekGameOptions.SetFloat(FloatOptionNames.ImpostorFlashlightSize, f);
        ImpostorFlashlightSizeValue = f;
    }
}