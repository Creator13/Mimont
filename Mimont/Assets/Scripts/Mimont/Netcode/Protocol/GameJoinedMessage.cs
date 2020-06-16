using Networking.Protocol;

namespace Mimont.Netcode.Protocol {
public class GameJoinedMessage : Message {
    public GameJoinedMessage() {
        Type = MessageType.GameJoined;
    }
}
}
