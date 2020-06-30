using Networking.Protocol;

namespace Mimont.Netcode.Protocol {
public class GameLostMessage : Message {
    public GameLostMessage() {
        Type = MessageType.GameLost;
    }
}
}
