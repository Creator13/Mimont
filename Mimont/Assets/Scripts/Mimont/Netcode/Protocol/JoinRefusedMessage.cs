using Networking.Protocol;

namespace Mimont.Netcode.Protocol {
public class JoinRefusedMessage : Message {
    public JoinRefusedMessage() {
        Type = MessageType.JoinRefused;
    }
}
}
