using System;
using System.Collections.Generic;
using Mimont.Netcode.Protocol;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

namespace Mimont.Netcode.Client {
internal struct PlayerInfo { }

public class Client : MonoBehaviour {
    public enum ConnectionStatus { Connecting, Connected, Disconnected }

    private NetworkDriver driver;
    private NetworkConnection connection;
    private long timeSinceLastTransmit;
    private JobHandle clientJobHandle;
    private ConnectionStatus connected = ConnectionStatus.Connecting;

    private PlayerInfo playerInfo;

    private readonly Queue<Message> receiveQueue = new Queue<Message>();
    private readonly Queue<Message> sendQueue = new Queue<Message>();

    public MessageEvent[] clientCallbacks = new MessageEvent[Enum.GetNames(typeof(MessageType)).Length];
    public string ConnectionIP { get; private set; }

    public ConnectionStatus Connected {
        get => connected;
        private set {
            connected = value;
            ConnectionStatusChanged?.Invoke(value);
        }
    }

    public event Action<ConnectionStatus> ConnectionStatusChanged;

    public void Connect(string address) {
        if (Connected == ConnectionStatus.Connected) {
            Debug.LogError("Client already connected", this);
        }

        ConnectionIP = address;

        driver = NetworkDriver.Create();
        connection = default;

        NetworkEndPoint endpoint;
        if (!string.IsNullOrEmpty(address)) {
            endpoint = NetworkEndPoint.Parse(address, NetConfig.PORT);
        }
        else {
            endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = NetConfig.PORT;
        }

        connection = driver.Connect(endpoint);

        for (var i = 0; i < clientCallbacks.Length; i++) {
            clientCallbacks[i] = new MessageEvent();
        }
    }

    public void Disconnect() {
        if (Connected == ConnectionStatus.Disconnected) return;

        clientJobHandle.Complete();

        connection.Disconnect(driver);
        connection = default;
        Connected = ConnectionStatus.Disconnected;
        clientJobHandle.Complete();
    }

    private void OnDestroy() {
        Disconnect();
        driver.Dispose();
    }

    private void Update() {
        if (connection == default || Connected == ConnectionStatus.Disconnected) return;

        clientJobHandle.Complete();

        if (!connection.IsCreated) {
            Debug.Log("Something went wrong while connecting");
            return;
        }

        KeepAlive();

        NetworkEvent.Type cmdType;
        while ((cmdType = connection.PopEvent(driver, out var reader)) != NetworkEvent.Type.Empty) {
            if (cmdType == NetworkEvent.Type.Data) {
                var msgType = (MessageType) reader.ReadUShort();
                switch (msgType) {
                    default:
                        Debug.LogError("Unresolved message");
                        break;
                }
            }
            else if (cmdType == NetworkEvent.Type.Disconnect) {
                Debug.Log("Disconnected from server");
                Connected = ConnectionStatus.Disconnected;
            }
        }

        ProcessMessageQueues();

        clientJobHandle = driver.ScheduleUpdate();
    }

    private void EnqueueReceived(Message msg) {
        receiveQueue.Enqueue(msg);
    }

    public void SendMessage(Message msg) {
        sendQueue.Enqueue(msg);
    }

    private void ProcessMessageQueues() {
        // Receive queue
        while (receiveQueue.Count > 0) {
            var msg = receiveQueue.Dequeue();
            clientCallbacks[(int) msg.Type]?.Invoke(msg);
        }

        // Send queue
        while (sendQueue.Count > 0) {
            var msg = sendQueue.Dequeue();

            var writer = driver.BeginSend(connection);
            Message.Send(msg, ref writer);
            driver.EndSend(writer);
            timeSinceLastTransmit = 0;
        }
    }

    private void KeepAlive() {
        timeSinceLastTransmit += (long) (Time.deltaTime * 1000);

        if (timeSinceLastTransmit > NetConfig.KEEP_ALIVE_TIME) {
            // var writer = driver.BeginSend(connection);
            // Message.Send(new NoneMessage(), ref writer);
            // driver.EndSend(writer);

            timeSinceLastTransmit = 0;
        }
    }
}
}
