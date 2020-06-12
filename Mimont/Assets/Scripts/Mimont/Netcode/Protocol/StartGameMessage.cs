using Networking.Protocol;

namespace Mimont.Netcode.Protocol {
public class StartGameMessage : Message {
    public StartGameMessage() {
        Type = MessageType.StartGame;
    }
}
}
