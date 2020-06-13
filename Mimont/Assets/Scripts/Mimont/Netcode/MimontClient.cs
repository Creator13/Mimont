using System;
using Mimont.Netcode.Protocol;
using Networking.Client;
using Networking.Protocol;
using Unity.Networking.Transport;

namespace Mimont.Netcode {
public class MimontClient : Client {
    private Player otherPlayer;

    public event Action StartGame;
    
    protected override void RegisterCallbacks() {
        callbacks[(int) MessageType.StartGame].AddListener(HandleStartGame);
    }

    protected override void HandleData(ref DataStreamReader reader) {
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
                connected = ConnectionStatus.Disconnected;
                break;
            case MessageType.StartGame:
                EnqueueReceived(Message.Read<StartGameMessage>(ref reader));
                break;
            case MessageType.Unresolved:
            case MessageType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleStartGame(Message msg) {
        var startGameMessage = (StartGameMessage) msg;
        StartGame?.Invoke();
    }
}
}
