using Networking.Protocol;
using Unity.Networking.Transport;
using UnityEngine;

namespace Mimont.Netcode.Protocol {
public class TargetSpawnedMessage : Message {
    public Vector3 Position { get; set; }
    public int TierIndex { get; set; } = -1;

    public TargetSpawnedMessage() {
        Type = MessageType.TargetSpawned;
    }
    
    protected override void SerializeObject(ref DataStreamWriter writer) {
        base.SerializeObject(ref writer);

        writer.WriteFloat(Position.x);
        writer.WriteFloat(Position.y);
        writer.WriteFloat(Position.z);

        writer.WriteInt(TierIndex);
    }

    protected override void DeserializeObject(ref DataStreamReader reader) {
        base.DeserializeObject(ref reader);

        Position = new Vector3(
            reader.ReadFloat(),
            reader.ReadFloat(),
            reader.ReadFloat()
        );
        TierIndex = reader.ReadInt();
    }
}
}
