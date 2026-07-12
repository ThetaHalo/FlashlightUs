using System;
using System.Collections.Generic;
using System.Linq;
using VentLib;
using VentLib.Networking.RPC.Attributes;
using VentLib.Utilities;

namespace FlashlightUs;

public static class ModCalls
{
    public const uint FlashlightUsIdentify = 0452-9;
    public const uint FlashlightUsIdentifyReply = 0453-0;
}

public static class FlashlightUsNetworking
{
    public static readonly HashSet<byte> ConfirmedPlayers = new();
    public static bool HostHasMod = false;

    public static void SendIDPing(PlayerControl player, Action<bool> onResult)
    {
        if (player == null) return;
        HostHasMod = true;
        byte playerId = player.PlayerId;

        ConfirmedPlayers.Remove(playerId);
        Vents.FindRPC(ModCalls.FlashlightUsIdentify)?.Send(new[] { player.GetClientId() });
        StaticLogger.Warn($"ping sent to playerId={playerId}");

        Async.Schedule(() =>
        {
            bool stillInGame = PlayerControl.AllPlayerControls.ToArray().Any(p => p.PlayerId == playerId);
            StaticLogger.Warn($"async finished, stillInGame={stillInGame}, confirmed={ConfirmedPlayers.Contains(playerId)}");
            if (!stillInGame) return;
            onResult(ConfirmedPlayers.Contains(playerId));
        }, NetUtils.DeriveDelay(4f));
    }

    [ModRPC(ModCalls.FlashlightUsIdentify, RpcActors.Host)]
    public static void Identify()
    {
        StaticLogger.Warn("identify");
        Utilities.RunWithLogging(() => HostHasMod = true, "Host has FlashlightUs");
        PlayerControl asker = Vents.GetLastSender(ModCalls.FlashlightUsIdentify);
        if (asker == null) return;
        Vents.FindRPC(ModCalls.FlashlightUsIdentifyReply)?.Send(new[] { asker.GetClientId() });
    }

    [ModRPC(ModCalls.FlashlightUsIdentifyReply, RpcActors.Everyone, RpcActors.Everyone)]
    public static void Reply()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        PlayerControl replier = Vents.GetLastSender(ModCalls.FlashlightUsIdentifyReply);
        if (replier == null) return;
        ConfirmedPlayers.Add(replier.PlayerId);
    }
}