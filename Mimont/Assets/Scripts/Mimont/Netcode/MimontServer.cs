﻿using System;
using System.Collections;
using Mimont.Gameplay;
using Mimont.Netcode.Protocol;
using Networking.Protocol;
using Networking.Server;
using Unity.Networking.Transport;
using UnityEngine;

namespace Mimont.Netcode {
public class MimontServer : Server {
    private readonly PlayerManager playerManager;
    private readonly TargetCreator targets;

    public MimontServer(TargetCreator targets) {
        this.targets = targets;
        playerManager = new PlayerManager(this);
        playerManager.LobbyFull += StartGame;
        playerManager.PlayerLeft += OnPlayerLeft;
    }

    protected override void RegisterCallbacks() {
        callbacks[(int) MessageType.RingCreated].AddListener(HandleRingCreated);
        callbacks[(int) MessageType.RingReleased].AddListener(HandleRingReleased);
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
            case MessageType.None:
                KeepAlive(id);
                break;
            case MessageType.RingCreated:
                EnqueueReceived(Message.Read<RingCreatedMessage>(ref reader), id);
                break;
            case MessageType.RingReleased:
                EnqueueReceived(Message.Read<RingReleasedMessage>(ref reader), id);
                break;
            case MessageType.GameJoined:
            case MessageType.PlayerLeft:
            case MessageType.JoinRefused:
            case MessageType.StartGame:
            case MessageType.TargetSpawned:
                // Client messages, should not be received by server
                LogWarning($"Server received message intended for client. Message was: {msgType.ToString()}");
                break;
            case MessageType.Unresolved:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    #region MessageHandlers

    private void HandleRingCreated(MessageWrapper wrapper) {
        var ringCreatedMessage = (RingCreatedMessage) wrapper.message;
        Send(ringCreatedMessage, playerManager.GetOtherPlayerID(wrapper.senderId));
    }

    private void HandleRingReleased(MessageWrapper wrapper) {
        var ringReleasedMessage = (RingReleasedMessage) wrapper.message;
        Send(ringReleasedMessage, playerManager.GetOtherPlayerID(wrapper.senderId));
    }

    #endregion

    private void OnPlayerLeft(int id) {
        SendToAllExcluding(new PlayerLeftMessage {playerId = id}, id);
        targets.Paused = true;
    }

    private void StartGame() {
        BroadcastStartGame();

        // Start spawnin'
        targets.TargetCreated += NotifyTargetSpawned;
        targets.StartSpawning(3);
    }

    private void NotifyTargetSpawned(Vector3 position, int tier) {
        SendToAll(new TargetSpawnedMessage {
            Position = position,
            TierIndex = tier
        }, playerManager.PlayerIds);
    }

    private void BroadcastStartGame() {
        SendToAll(new StartGameMessage(), playerManager.PlayerIds);
    }
}
}