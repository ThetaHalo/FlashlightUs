using System;
using System.Reflection;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using VentLib.Networking.RPC;
using VentLib.Options.UI;
using VentLib.Utilities;

namespace FlashlightUs;

public static class Utilities
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(Utilities));
    
    // https://github.com/Lotus-AU/LotusContinued/blob/8a00694ae082339c9abad299945ce1f1522be74c/src/Extensions/AmongUsClientExtensions.cs#L11
    public static void KickWithMessage(this PlayerControl target, string message, bool banPlayer = false)
    {
        if (target == null) return;
        var name = target.name + " ";
        message += "<size=0>"; // removes the "left the game" text.
        target.SetName(name + message);
        RpcV3.Immediate(target.NetId, RpcCalls.SetName).Write(target.Data.NetId).Write(name + message).Send();
        Async.Schedule(() =>
        {
            target.SetName(name + message);
            AmongUsClient.Instance.KickPlayer(target.GetClientId(), banPlayer);
        }, NetUtils.DeriveDelay(0.2f));
    }
    
    // https://github.com/Lotus-AU/LotusContinued/blob/8a00694ae082339c9abad299945ce1f1522be74c/src/Extensions/OptionBuilderExtensions.cs#L40
    public static GameOptionBuilder KeyName(this GameOptionBuilder builder, string key, string name) => builder.Key(key).Name(name);
    
    // // https://github.com/Lotus-AU/LotusContinued/blob/8a00694ae082339c9abad299945ce1f1522be74c/src/Logging/LogManager.cs#L49
    public static void SendNotification(string message, Sprite sprite = null!)
    {
        if (!HudManager.InstanceExists)
        {
            log.Warn("Can't call SendNotification() before HudManager is initialized.");
            return;
        }
        log.Info($"Sending notification: {message}");
        
        NotificationPopper notifier = DestroyableSingleton<HudManager>.Instance.Notifier;
        LobbyNotificationMessage newMessage = UnityEngine.Object.Instantiate<LobbyNotificationMessage>(notifier.notificationMessageOrigin, Vector3.zero, Quaternion.identity, notifier.transform);
        newMessage.transform.localPosition = new Vector3(0f, 0f, -2f);
        newMessage.SetUp("  " + message.Trim(), sprite, Color.white, (Action)(() => notifier.OnMessageDestroy(newMessage)));
        notifier.ShiftMessages();
        notifier.AddMessageToQueue(newMessage);
        SoundManager.Instance.PlaySound(notifier.settingsChangeSound, false, 1f, null);
    }
    
    // https://github.com/Lotus-AU/VentFramework-Continued/blob/998678ca83415b8a22adcd294e7e5a9a37630907/src/Utilities/AssetLoader.cs#L9
    public static Sprite LoadSprite(string path, float pixelsPerUnit = 100f, bool linear = false, int mipMapLevel = 0)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string assemblyName = "FlashlightUs.assets.";

        if (!path.StartsWith(assemblyName)) path = assemblyName + path;
        using (var stream = assembly.GetManifestResourceStream(path))
        {
            if (stream == null)
            {
                log.Fatal($"Resource '{path}' not found.");
                return null!;
            }

            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, true, linear);
            texture.LoadImage(getthestuff(stream));

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.texture.requestedMipmapLevel = mipMapLevel;
            return sprite;
        }
        byte[] getthestuff(System.IO.Stream input)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
    
    public static GameModes GetCurrentGamemode() => GameOptionsManager.Instance.currentGameMode;
    public static bool IsHNS() => 
        GetCurrentGamemode() == GameModes.HideNSeek || GetCurrentGamemode() == GameModes.SeekFools;

    public static bool IsLotusLoaded() =>
        IL2CPPChainloader.Instance.Plugins.ContainsKey("com.discussions.LotusContinued");
}