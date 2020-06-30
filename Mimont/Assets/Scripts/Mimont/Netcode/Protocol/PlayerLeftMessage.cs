using Networking.Protocol;
using Unity.Networking.Transport;

namespace Mimont.Netcode.Protocol {
public class PlayerLeftMessage : Message {
    public int playerId;

    public PlayerLeftMessage() {
        Type = MessageType.PlayerLeft;
    }

    protected override void SerializeObject(ref DataStreamWriter writer) {
        base.SerializeObject(ref writer);

        writer.WriteInt(playerId);
    }

    protected override void DeserializeObject(ref DataStreamReader reader) {
        base.DeserializeObject(ref reader);
        playerId = reader.ReadInt();
    }
}
}
