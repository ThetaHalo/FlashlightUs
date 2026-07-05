global using VentLib.Logging;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FlashlightUs;
using HarmonyLib;
using VentLib;
using VentLib.Utilities.Extensions;
using VentLib.Version;
using VentLib.Version.BuiltIn;
using VentLib.Version.Git;
using VentLib.Version.Updater;

[assembly: AssemblyVersion(FlashlightUsPlugin.ModVersion)]
namespace FlashlightUs;

[BepInPlugin(Id, "FlashlightUs", ModVersion)]
[BepInProcess("Among Us.exe")]
[BepInIncompatibility("com.discussions.LotusContinued")] // incompatible due to custom options menu
[BepInDependency(Vents.Id)]
public partial class FlashlightUsPlugin : BasePlugin, IGitVersionEmitter
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(FlashlightUsPlugin));
    public const string Id = "lol.eps.FlashlightUs";
    public const string ModVersion = $"{MajorVersion}.{MinorVersion}.{PatchVersion}.{BuildNumber}";
    public const bool Debug = false;

    public const string MajorVersion = "1";
    public const string MinorVersion = "0";
    public const string PatchVersion = "0";
    public const string BuildNumber = "0070";

    public readonly GitVersion CurrentVersion = new(typeof(FlashlightUsPlugin).Assembly);
    public GitVersion Version() => CurrentVersion;

    public Harmony Harmony { get; private set; }
    public static FlashlightUsPlugin Instance { get; private set; }
    public static ModUpdater ModUpdater = null!;

    public override void Load()
    {
        Harmony = new Harmony(Id);
        
        Harmony.PatchAll();
        log.Info("FlashlightUs loaded!");
        
        Assembly.GetExecutingAssembly().GetManifestResourceNames().ForEach(s => log.Info(s));
    }

    public FlashlightUsPlugin()
    {
        Instance = this;
        
        VersionControl vc = VersionControl.For(this);
        vc.AddVersionReceiver(OnJoin);
        ModUpdater = ModUpdater.Default();
        ModUpdater.EstablishConnection();
    }

    public void OnJoin(Version version, PlayerControl player)
    {
        if (player == null) return;
        if (version is not NoVersion) return;
        if (!FlashlightUsOptions.KickUnmoddedPlayersValue)
        {
            Utilities.SendNotification($"{player.name} does not have FlashlightUs installed.", Utilities.LoadSprite("FlashlightUs.assets.logo.png", 500f));
            return;
        }
        log.Info($"Kicking unmodded player: {player.name}");
        player.KickWithMessage("was kicked due to not having FlashlightUs installed.");
    }
}