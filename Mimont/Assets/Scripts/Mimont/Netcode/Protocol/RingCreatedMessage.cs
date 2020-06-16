using Networking.Protocol;
using Unity.Networking.Transport;
using UnityEngine;

namespace Mimont.Netcode.Protocol {
public class RingCreatedMessage : Message {
    public Vector3 Position { get; set; }

    public RingCreatedMessage() {
        Type = MessageType.RingCreated;
    }

    protected override void SerializeObject(ref DataStreamWriter writer) {
        base.SerializeObject(ref writer);

        writer.WriteFloat(Position.x);
        writer.WriteFloat(Position.y);
        writer.WriteFloat(Position.z);
    }

    protected override void DeserializeObject(ref DataStreamReader reader) {
        base.DeserializeObject(ref reader);

        Position = new Vector3(
            reader.ReadFloat(),
            reader.ReadFloat(),
            reader.ReadFloat()
        );
    }
}
}
