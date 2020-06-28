using System;
using Mimont.Netcode.Protocol;
using Networking.Client;
using Networking.Protocol;
using Unity.Networking.Transport;
using UnityEngine;

namespace Mimont.Netcode {
public class MimontClient : Client {
    private Gameplay.IPlayer player;

    public event Action StartGame;
    public event Action PlayerLeft;
    public event Action Disconnected;
    public event Action GameWon;
    public event Action GameLost;

    public Gameplay.IPlayer Player {
        get => player;
        set {
            if (player != null) UnregisterFromPlayer();
            player = value;
            if (player != null) RegisterToPlayer();
        }
    }

    protected override void RegisterCallbacks() {
        callbacks[(int) MessageType.StartGame].AddListener(HandleStartGame);
        callbacks[(int) MessageType.PlayerLeft].AddListener(HandlePlayerLeft);
        callbacks[(int) MessageType.JoinRefused].AddListener(HandleJoinRefused);
        callbacks[(int) MessageType.TargetSpawned].AddListener(HandleTargetSpawned);
        callbacks[(int) MessageType.RingCreated].AddListener(HandleOtherRingCreated);
        callbacks[(int) MessageType.RingReleased].AddListener(HandleOtherRingReleased);
        // callbacks[(int) MessageType.GameWon].AddListener(HandleGameWon);
        callbacks[(int) MessageType.GameLost].AddListener(HandleGameLost);
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
            case MessageType.TargetSpawned:
                EnqueueReceived(Message.Read<TargetSpawnedMessage>(ref reader));
                break;
            case MessageType.RingCreated:
                EnqueueReceived(Message.Read<RingCreatedMessage>(ref reader));
                break;
            case MessageType.RingReleased:
                EnqueueReceived(Message.Read<RingReleasedMessage>(ref reader));
                break;
            case MessageType.GameLost:
                EnqueueReceived(Message.Read<GameLostMessage>(ref reader));
                break;
            case MessageType.GameWon:
                EnqueueReceived(Message.Read<GameWonMessage>(ref reader));
                break;
            case MessageType.Unresolved:
            case MessageType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    #region MessageHandlers

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

    private void HandleTargetSpawned(Message msg) {
        var targetSpawnedMessage = (TargetSpawnedMessage) msg;
        Player.AddTarget(targetSpawnedMessage.Position, targetSpawnedMessage.TierIndex);
    }

    private void HandleOtherRingCreated(Message msg) {
        var ringCreatedMessage = (RingCreatedMessage) msg;
        Player.StartOtherRing(ringCreatedMessage.Position);
    }

    private void HandleOtherRingReleased(Message msg) {
        var ringReleasedMessage = (RingReleasedMessage) msg;
        Player.ReleaseOtherRing();
    }

    // private void HandleGameWon(Message msg) {
    //     var gameWonMessage = (GameWonMessage) msg;
    //     GameWon?.Invoke();
    // }

    private void HandleGameLost(Message msg) {
        var gameLostMessage = (GameLostMessage) msg;
        GameLost?.Invoke();
    }

    #endregion


    private void CheckConnectionStatus(ConnectionStatus newStatus) {
        if (newStatus == ConnectionStatus.Disconnected) {
            Disconnected?.Invoke();
        }
    }

    private void NotifyRingCreated(Vector3 position) {
        Send(new RingCreatedMessage {Position = position});
    }

    private void NotifyRingReleased() {
        Send(new RingReleasedMessage());
    }

    private void RegisterToPlayer() {
        if (Player == null) return;

        Player.RingCreated += NotifyRingCreated;
        Player.RingReleased += NotifyRingReleased;
    }

    private void UnregisterFromPlayer() {
        if (Player == null) return;

        Player.RingCreated -= NotifyRingCreated;
        Player.RingReleased -= NotifyRingReleased;
    }
}
}
