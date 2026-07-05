using VentLib.Localization.Attributes;

namespace FlashlightUs;

[Localized("FlashlightUs")]
public class Translations
{
    public static string ModName = "FlashlightUs";
    
    
    [Localized("Options")]
    public static class OptionsMenu
    {
        [Localized(nameof(Options))] public static string Options = "Options";
        [Localized(nameof(Close))] public static string Close = "Close";
        [Localized("Enable Flashlight")] public static string EnableFlashlight = "Enable Flashlight";
        [Localized("Force Flashlight")] public static string ForceFlashlight = "Force Flashlight";
    }
}