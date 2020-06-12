using System;
using Mimont.Netcode.Protocol;
using Networking.Server;
using Unity.Networking.Transport;

namespace Mimont.Netcode {
public class MimontServer : Server {
    private PlayerManager playerManager;

    public MimontServer() {
        playerManager = new PlayerManager(this);
    }
    
    protected override void HandleData(ref DataStreamReader reader, int id) {
        var msgTypeId = reader.ReadUShort();
        var msgType = MessageType.Unresolved;
        try {
            msgType = (MessageType) msgTypeId;
        }
        catch (InvalidCastException e) {
            LogError($"Unresolved message type: {msgTypeId}");
        }

        switch (msgType) {
            
        }
    }
}
}
