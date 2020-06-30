using Networking.Protocol;

namespace Mimont.Netcode.Protocol {
public class RingReleasedMessage : Message {
    public RingReleasedMessage() {
        Type = MessageType.RingReleased;
    }
}
}
