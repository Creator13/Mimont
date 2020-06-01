using System;
using System.Collections.Generic;
using System.Linq;
using Mimont.Netcode.Protocol;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Mimont.Netcode.Server {
public class Server : MonoBehaviour {
    private struct MessageWrapper {
        public Message message;
        public int internalId;
    }

    private NetworkDriver driver;
    private NativeList<NetworkConnection> connections;
    public List<NetworkConnection> Connections => connections.ToArray().ToList();

    private readonly Queue<MessageWrapper> receiveQueue = new Queue<MessageWrapper>();
    private readonly Queue<MessageWrapper> sendQueue = new Queue<MessageWrapper>();
    public MessageEvent[] serverCallbacks = new MessageEvent[Enum.GetNames(typeof(MessageType)).Length];

    private bool isRunning;

    public bool IsRunning {
        get => isRunning;
        private set {
            isRunning = value;
            RunningStateChanged?.Invoke();
        }
    }

    [field: NonSerialized] public event Action RunningStateChanged;
    [field: NonSerialized] public event Action ConnectionsUpdated;

    public bool StartServer() {
        if (IsRunning) {
            Debug.LogWarning("Server already running!");
            return false;
        }

        // Create server
        driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = NetConfig.PORT;

        // Bind to port
        if (driver.Bind(endpoint) != 0) {
            Debug.LogError($"Failed to bind port {NetConfig.PORT}.");
            return false;
        }

        connections = new NativeList<NetworkConnection>(NetConfig.MAX_CONNECTIONS, Allocator.Persistent);

        // // Create callbacks
        // for (var i = 0; i < serverCallbacks.Length; i++) {
        //     serverCallbacks[i] = new MessageEvent();
        // }

        driver.Listen();

        IsRunning = true;

        Debug.Log($"Server listening on port {NetConfig.PORT}");

        return true;
    }

    public void StopServer() {
        if (!IsRunning) return;
        driver.Dispose();
        connections.Dispose();

        IsRunning = false;

        Debug.Log("Stopped server");
    }

    private void OnDestroy() {
        StopServer();
    }

    private void Update() {
        if (!IsRunning) return;

        driver.ScheduleUpdate().Complete();

        // Clean up connections
        for (var i = 0; i < connections.Length; i++) {
            if (!connections[i].IsCreated) {
                connections.RemoveAtSwapBack(i);
                ConnectionsUpdated?.Invoke();
                i++;
            }
        }

        // Accept connections
        NetworkConnection newConnection;
        while ((newConnection = driver.Accept()) != default) {
            connections.Add(newConnection);
            ConnectionsUpdated?.Invoke();
        }

        for (var i = 0; i < connections.Length; i++) {
            if (!connections[i].IsCreated) {
                continue;
            }

            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out var reader)) != NetworkEvent.Type.Empty) {
                if (cmd == NetworkEvent.Type.Data) {
                    var msgType = (MessageType) reader.ReadUShort();
                    switch (msgType) { }
                }
                else if (cmd == NetworkEvent.Type.Disconnect) {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default;
                }
            }
        }

        ProcessMessageQueues();
    }

    private void EnqueueReceived(Message msg, int connId) {
        receiveQueue.Enqueue(new MessageWrapper {
            message = msg,
            internalId = connId
        });
    }

    public void SendMessage(Message msg, int recipientId) {
        sendQueue.Enqueue(new MessageWrapper {
            message = msg,
            internalId = recipientId
        });
    }

    private void ProcessMessageQueues() {
        while (receiveQueue.Count > 0) {
            var msg = receiveQueue.Dequeue();
            serverCallbacks[(int) msg.message.Type]?.Invoke(msg.message);
        }

        while (sendQueue.Count > 0) {
            // TODO upgrade this so that all messages can be sent to the client at once without any more lookups than necessary
            var msg = sendQueue.Dequeue();
            var connection = Connections.First(conn => conn.InternalId == msg.internalId);

            var writer = driver.BeginSend(connection);
            Message.Send(msg.message, ref writer);
            driver.EndSend(writer);
        }
    }
}
}
