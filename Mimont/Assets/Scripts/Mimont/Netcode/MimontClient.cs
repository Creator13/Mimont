using System;
using Mimont.Netcode.Protocol;
using Networking.Client;
using Unity.Networking.Transport;

namespace Mimont.Netcode {
public class MimontClient : Client {
    protected override void HandleData(ref DataStreamReader reader) {
        var msgTypeId = reader.ReadUShort();
        var msgType = MessageType.Unresolved;
        try {
            msgType = (MessageType) msgTypeId;
        }
        catch (InvalidCastException e) {
            LogError($"Unresolved message type: {msgTypeId}");
        }
    }
}
}
