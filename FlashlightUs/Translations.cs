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
        [Localized("Force Flashlight")] public static string ForceFlashlight = "Force Flashlight for Clients";
        [Localized("Kick Unmodded Players")] public static string KickUnmoddedPlayers = "Kick Unmodded Players";
        [Localized("Crewmate Flashlight Size")] public static string CrewmateFlashlightSize = "Crewmate Flashlight Size";
        [Localized("Impostor Flashlight Size")] public static string ImpostorFlashlightSize = "Impostor Flashlight Size";
    }
}