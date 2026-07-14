global using VentLib.Logging;
global using Object = UnityEngine.Object;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FlashlightUs;
using FlashlightUs.Networking;
using FlashlightUs.UI;
using FlashlightUs.UI.Patches;
using HarmonyLib;
using VentLib;
using VentLib.Utilities.Optionals;
using VentLib.Version;
using VentLib.Version.Git;
using VentLib.Version.Updater;
using VentLib.Version.Updater.Github;
using Version = VentLib.Version.Version;

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
    public const bool Debug = false;

    public const string MajorVersion = "1";
    public const string MinorVersion = "3";
    public const string PatchVersion = "0";
    public const string BuildNumber = "0208";

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
        ModUpdater.RegisterReleaseCallback(BeginUpdate, true);
        ModUpdater.EstablishConnection();
    }
    
    public static void BeginUpdate(Release release)
    {
        UnityOptional<ModUpdateMenu>.Of(ModUpdaterPatches.ModUpdateMenu).Handle(o => o.Open(), () => {});
        ModUpdateMenu.AddUpdateItem("FlashlightUs", release.TagName, ex => ModUpdater.Update(errorCallback: ex)!);
        Assembly ventAssembly = typeof(Vents).Assembly;

        if (release.ContainsDLL($"{ventAssembly.GetName().Name!}.dll"))
            ModUpdateMenu.AddUpdateItem("VentFrameworkContinued", null, ex => ModUpdater.Update(ventAssembly, ex)!);
    }

    // we ping instead of relying on vf stuff due to other vf mods possibly interfering with the proper way to check
    public void OnJoin(Version version, PlayerControl player) 
    {
        if (player == null) return;
        
        NetworkManager.SendIDPing(player, isFlashlightUs =>
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