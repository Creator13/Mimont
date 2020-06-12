using Networking.Protocol;

namespace Mimont.Netcode.Protocol {
public class NoneMessage : Message {
    public NoneMessage() {
        Type = MessageType.None;
    }
}
}
