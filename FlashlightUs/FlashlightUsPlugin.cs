global using VentLib.Logging;
global using Object = UnityEngine.Object;
using System.Reflection;
using BepInEx;
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
[BepInIncompatibility("com.discussions.LotusContinued")] // incompatible due to PL's custom options menu, I'm going to add compatibility later.
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
    public const string BuildNumber = "0078";

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
    }

    public FlashlightUsPlugin()
    {
        Instance = this;
        
        VersionControl vc = VersionControl.For(this);
        vc.AddVersionReceiver(OnJoin);
        ModUpdater = ModUpdater.Default();
        ModUpdater.EstablishConnection();
    }

    // we ping instead of relying on vf stuff due to other vf mods possibly interfering with the proper way to check
    public void OnJoin(Version version, PlayerControl player) 
    {
        if (player == null) return;
        
        FlashlightUsNetworking.SendIDPing(player, isFlashlightUs =>
        {
            if (isFlashlightUs)
            {
                log.Info($"{player.name} is using FlashlightUs!");
                Utilities.SendNotification($"{player.name} is using FlashlightUs!", Utilities.LoadSprite("FlashlightUs.assets.logo.png", 500f));
                return;
            }
            
            if (!FlashlightUsOptions.KickUnmoddedPlayersValue)
            {
                log.Info($"{player.name} does not have FlashlightUs installed.");
                Utilities.SendNotification($"{player.name} does not have FlashlightUs installed.", Utilities.LoadSprite("FlashlightUs.assets.logo.png", 500f));
                return;
            }
            
            log.Info($"Kicking unmodded player: {player.name}");
            player.KickWithMessage("was kicked due to not having FlashlightUs installed.");
        });
    }
}