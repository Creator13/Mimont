using System;
using System.Collections;
using System.Linq;
using Mimont.Gameplay;
using Mimont.Netcode.Protocol;
using Networking.Protocol;
using Networking.Server;
using Unity.Networking.Transport;
using UnityEngine;

namespace Mimont.Netcode {
public class MimontServerDebug : Server {
    private readonly PlayerManagerDebug playerManager;
    private readonly TargetCreator targets;
    private readonly float gameTime;
    private float timePassed;

    private static readonly Vector3 DEFAULT = Vector3.positiveInfinity;
    private Vector3[] playerRingPositions = new Vector3[2];

    private GameObject gameTimer;

    public MimontServerDebug(TargetCreator targets, float gameTime) {
        this.targets = targets;
        this.gameTime = gameTime;
        playerManager = new PlayerManagerDebug(this);

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
            case MessageType.GameLost:
            case MessageType.GameWon:
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

    protected override void Clean() {
        targets.Paused = true;
    }


    #region MessageHandlers

    protected virtual void HandleRingCreated(MessageWrapper wrapper) {
        // var ringCreatedMessage = (RingCreatedMessage) wrapper.message;
        //
        // var i = playerManager.PlayerIds.ToList().IndexOf(wrapper.senderId);
        // playerRingPositions[i] = ringCreatedMessage.Position;
        //
        // if (GetPlayerRingDistance() < .3f) {
        //     SendToAll(new GameWonMessage(), playerManager.PlayerIds);
        // }
        // else {
        //     Send(ringCreatedMessage, playerManager.GetOtherPlayerID(wrapper.senderId));
        // }
    }

    protected virtual void HandleRingReleased(MessageWrapper wrapper) {
        // var ringReleasedMessage = (RingReleasedMessage) wrapper.message;
        // var i = playerManager.PlayerIds.ToList().IndexOf(wrapper.senderId);
        // playerRingPositions[i] = DEFAULT;
        // Send(ringReleasedMessage, playerManager.GetOtherPlayerID(wrapper.senderId));
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
        targets.StartSpawning(3.1f);
        StartGameTimer(3.1f);
    }

    private void NotifyTargetSpawned(Vector3 pos1, Vector3 pos2, int tier1, int tier2) {
        Send(new TargetSpawnedMessage {Position = pos1, TierIndex = tier1}, playerManager.PlayerIds[0]);
        // Send(new TargetSpawnedMessage {Position = pos2, TierIndex = tier2}, playerManager.PlayerIds[1]);
    }

    private void BroadcastStartGame() {
        SendToAll(new StartGameMessage(), playerManager.PlayerIds);
    }

    private float GetPlayerRingDistance() {
        return Vector3.Distance(playerRingPositions[0], playerRingPositions[1]);
    }

    private void StartGameTimer(float startDelay) {
        timePassed = 0;
        var totalTime = startDelay + gameTime;

        void TimeUpdate() {
            timePassed += Time.deltaTime;
            if (timePassed > totalTime) {
                SendGameLostMessage();
                updateMethods.Remove(TimeUpdate);
            }
        }

        updateMethods.Add(TimeUpdate);
    }

    private void SendGameLostMessage() {
        SendToAll(new GameLostMessage(), playerManager.PlayerIds);
    }
}
}
