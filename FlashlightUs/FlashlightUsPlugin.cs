global using VentLib.Logging;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FlashlightUs;
using HarmonyLib;
using VentLib;
using VentLib.Version;
using VentLib.Version.Git;
using VentLib.Version.Updater;

[assembly: AssemblyVersion(FlashlightUsPlugin.ModVersion)]
namespace FlashlightUs;

[BepInPlugin(Id, "FlashlightUs", ModVersion)]
[BepInProcess("Among Us.exe")]
[BepInDependency(Vents.Id)]
public partial class FlashlightUsPlugin : BasePlugin, IGitVersionEmitter
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(FlashlightUsPlugin));
    public const string Id = "lol.eps.FlashlightUs";
    public const string ModVersion = $"{MajorVersion}.{MinorVersion}.{PatchVersion}.{BuildNumber}";

    public const string MajorVersion = "1";
    public const string MinorVersion = "0";
    public const string PatchVersion = "0";
    public const string BuildNumber = "0001";

    public readonly GitVersion CurrentVersion = new(typeof(FlashlightUsPlugin).Assembly);
    public GitVersion Version() => CurrentVersion;

    public Harmony Harmony { get; private set; }
    public static FlashlightUsPlugin Instance { get; private set; }
    public static ModUpdater ModUpdater = null!;

    public ConfigEntry<bool> EnableFlashlight { get; private set; }
    public ConfigEntry<bool> ForceFlashlight { get; private set; }

    public override void Load()
    {
        Harmony = new Harmony(Id);

        EnableFlashlight = Config.Bind("Settings", "Enable Flashlight", true, "Enable Flashlights (for only you.)");
        ForceFlashlight = Config.Bind("Settings", "Force Flashlight", false, "Force Flashlights (for all modded clients)");
        Harmony.PatchAll();
        log.Info("FlashlightUs loaded!");
    }

    public FlashlightUsPlugin()
    {
        Instance = this;
        
        VersionControl _ = VersionControl.For(this);
        ModUpdater = ModUpdater.Default();
        ModUpdater.EstablishConnection();
    }

}
