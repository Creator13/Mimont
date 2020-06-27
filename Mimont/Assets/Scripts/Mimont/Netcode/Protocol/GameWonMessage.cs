using Networking.Protocol;

namespace Mimont.Netcode.Protocol {
public class GameWonMessage : Message {
    public GameWonMessage() {
        Type = MessageType.GameWon;
    }
}
}
