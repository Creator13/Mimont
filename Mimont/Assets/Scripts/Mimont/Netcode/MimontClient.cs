using System;
using Mimont.Netcode.Protocol;
using Networking.Client;
using Networking.Protocol;
using Unity.Networking.Transport;

namespace Mimont.Netcode {
public class MimontClient : Client {
    public event Action StartGame;
    public event Action PlayerLeft;
    public event Action Disconnected;

    protected override void RegisterCallbacks() {
        callbacks[(int) MessageType.StartGame].AddListener(HandleStartGame);
        callbacks[(int) MessageType.PlayerLeft].AddListener(HandlePlayerLeft);
        callbacks[(int) MessageType.JoinRefused].AddListener(HandleJoinRefused);
        ConnectionStatusChanged += CheckConnectionStatus;
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
                EnqueueReceived(Message.Read<PlayerLeftMessage>(ref reader));
                break;
            case MessageType.JoinRefused:
                EnqueueReceived(Message.Read<JoinRefusedMessage>(ref reader));
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

    private void HandlePlayerLeft(Message msg) {
        var playerLeftMessage = (PlayerLeftMessage) msg;
        PlayerLeft?.Invoke();
    }
    
    private void HandleJoinRefused(Message msg) {
        var joinRefusedMessage = (JoinRefusedMessage) msg;
        connected = ConnectionStatus.Disconnected;
    }

    private void CheckConnectionStatus(ConnectionStatus newStatus) {
        if (newStatus == ConnectionStatus.Disconnected) {
            Disconnected?.Invoke();
        }
    }
}
}
