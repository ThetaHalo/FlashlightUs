using VentLib.Localization.Attributes;

namespace FlashlightUs;

[Localized("FlashlightUs")]
public class Translations
{
    public static string ModName = "FlashlightUs";
    
    [Localized("Options")]
    public static class OptionsMenu
    {
        [Localized(nameof(Description))] public static string Description = "Configure game-related settings for FlashlightUs.";
        [Localized(nameof(ChangeInOptions))] public static string ChangeInOptions = "You can enable your own Flashlight in options.";
        
        [Localized(nameof(Options))] public static string Options = "Options";
        [Localized(nameof(Close))] public static string Close = "Close";
        [Localized("Enable Flashlight")] public static string EnableFlashlight = "Enable Flashlight";
        [Localized("Enable Flashlight in Lobby")] public static string EnableFlashlightInLobby = "Enable Flashlight in Lobby";
        [Localized("Force Flashlight")] public static string ForceFlashlight = "Force Flashlight for Clients";
        [Localized("Kick Unmodded Players")] public static string KickUnmoddedPlayers = "Kick Unmodded Players";
        [Localized("Crewmate Flashlight Size")] public static string CrewmateFlashlightSize = "Crewmate Flashlight Size";
        [Localized("Impostor Flashlight Size")] public static string ImpostorFlashlightSize = "Impostor Flashlight Size";
    }
    
    [Localized("ModUpdater")]
    public static class ModUpdater
    {
        [Localized("StarlightUpdateAvailable")] public static string StarlightUpdateAvailable = "FlashlightUs Update Available!\n<size=75%>Update inside the Starlight App</size>";
        [Localized("PCUpdateAvailable")] public static string PCUpdateAvailable = "FlashlightUs Update Available!\n<size=75%>Go to FlashlightUs Options to update!</size>";
        [Localized("Update")] public static string Update = "Update";
        [Localized("UpdateMod")] public static string UpdateMod = "Update Mod!";
        [Localized("MenuTitle")] public static string MenuTitle = "FlashlightUs Updater";
        [Localized("ExitGame")] public static string ExitGame = "Exit Game";
        [Localized("Updating")] public static string Updating = "Updating...";
        [Localized("Pending")] public static string Pending = "Pending";
        [Localized("Done")] public static string Done = "Done";
        [Localized("Failed")] public static string Failed = "Failed";
    }
}