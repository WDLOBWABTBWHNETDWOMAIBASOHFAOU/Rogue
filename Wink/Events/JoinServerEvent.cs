using System;

namespace Wink
{
    [Serializable()]
    public class JoinServerEvent : Event
    {
        public string ClientName { get; set; }

        public JoinServerEvent(Sender sender) : base(sender)
        {

        }

        public override void OnClientReceive(LocalClient client)
        {
        }

        public override void OnServerReceive(LocalServer server)
        {
            (sender as RemoteClient).ClientName = ClientName;
        }

        public override bool Validate(Level level)
        {
            //TODO: Implement Validation.
            return true;
        }
    }
}
