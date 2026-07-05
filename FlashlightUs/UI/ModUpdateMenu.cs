using System.Reflection;
using FlashlightUs.UI.Patches;
using UnityEngine;
using VentLib;
using VentLib.Utilities.Optionals;
using VentLib.Version.Updater;
using VentLib.Version.Updater.Github;

namespace FlashlightUs.UI;

// based on: https://github.com/Lotus-AU/LotusContinued/blob/8a00694ae082339c9abad299945ce1f1522be74c/src/GUI/Menus/ModUpdateMenu.cs
public class ModUpdateMenu: MonoBehaviour
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(ModUpdateMenu));

    public void Open()
    {
        
    }

    public static void BeginUpdate(Release release)
    {
        UnityOptional<ModUpdateMenu>.Of(ModUpdaterPatches.ModUpdateMenu).Handle(o => o.Open(), () => ModUpdaterPatches.IsReady = true);
        ModUpdateMenu.AddUpdateItem("Lotus", null, ex => ModUpdater.Update(errorCallback: ex)!);
        Assembly ventAssembly = typeof(Vents).Assembly;

        if (release.ContainsDLL($"{ventAssembly.GetName().Name!}.dll"))
            ModUpdateMenu.AddUpdateItem("VentFrameworkContinued", null, ex => ModUpdater.Update(ventAssembly, ex)!);
    }
}