using System;
using System.Collections.Generic;
using System.Linq;
using Mimont.Netcode.Protocol;
using UnityEngine;

namespace Mimont.Netcode {
internal class Player {
    public int internalId;
}

internal class PlayerManager {
    private const int MaxPlayers = 2;
    
    private readonly MimontServer server;
    private readonly List<Player> players = new List<Player>();

    internal int[] PlayerIds => players.Select(p => p.internalId).ToArray();

    public event Action LobbyFull;

    public PlayerManager(MimontServer server) {
        this.server = server;
        server.RunningStateChanged += RegisterCallbacks;
    }

    ~PlayerManager() {
        UnregisterCallbacks();
    }

    private void RegisterCallbacks(bool running) {
        if (running) {
            server.ConnectionReceived += HandleNewConnection;
            server.ConnectionRemoved += HandleDisconnected;
        }
        else {
            UnregisterCallbacks();
        }
    }

    private void UnregisterCallbacks() {
        server.ConnectionReceived -= HandleNewConnection;
        server.ConnectionRemoved -= HandleDisconnected;
    }

    private void HandleDisconnected(int id) {
        var player = players.FirstOrDefault(p => p.internalId == id);
        if (player == default) {
            throw new InvalidOperationException("Tried to remove player with an id that is not in any list.");
        }
        

        players.Remove(player);
        server.SendToAllExcluding(new PlayerLeftMessage {playerId = id}, id);
    }

    private void HandleNewConnection(int id) {
        if (AddNewPlayer(id)) {
            server.Send(new GameJoinedMessage(), id);

            if (players.Count == MaxPlayers) {
                LobbyFull?.Invoke();
            }
        }
        else {
            server.Send(new JoinRefusedMessage(), id);
            server.ScheduleKick(id);
        }
    }

    private bool AddNewPlayer(int id) {
        if (players.Any(p => p.internalId == id)) {
            server.LogError("Couldn't add player, found duplicate");
            return false;
        }

        if (players.Count > MaxPlayers) {
            server.LogWarning("Client tried to join, but server is full!");
            return false;
        }

        players.Add(new Player {internalId = id});
        server.Log($"New player connected with id {id}");
        return true;
    }

    public int GetOtherPlayerID(int id) {
        if (!players.Any(p => p.internalId == id)) {
            server.LogError("Not a active player!");
        }

        var ids = PlayerIds.ToList();
        var index = ids.IndexOf(id);
        return 1 - index;
        // 1 - 1 = 0, 1 - 1 = 0, inverting the input
    }
}
}
