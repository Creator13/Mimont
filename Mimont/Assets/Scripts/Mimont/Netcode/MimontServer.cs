using System;
using Mimont.Netcode.Protocol;
using Networking.Server;
using Unity.Networking.Transport;

namespace Mimont.Netcode {
public class MimontServer : Server {
    private PlayerManager playerManager;

    public MimontServer() {
        playerManager = new PlayerManager(this);
        playerManager.LobbyFull += BroadcastStartGame;
    }

    protected override void RegisterCallbacks() {
        
    }

    protected override void HandleData(ref DataStreamReader reader, int id) {
        var msgTypeId = reader.ReadUShort();
        var msgType = MessageType.Unresolved;
        try {
            msgType = (MessageType) msgTypeId;
        }
        catch (InvalidCastException) {
            LogError($"Unresolved message type: {msgTypeId}");
        }

        switch (msgType) {
            case MessageType.GameJoined:
                break;
            case MessageType.PlayerLeft:
                break;
            case MessageType.JoinRefused:
                break;
            case MessageType.StartGame:
                break;
            case MessageType.Unresolved:
                break;
            case MessageType.None:
                KeepAlive(id);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void BroadcastStartGame() {
        SendToAll(new StartGameMessage(), playerManager.PlayerIds);
    }
}
}
