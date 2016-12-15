using System;

namespace Wink
{
    [Serializable()]
    public class JoinServerEvent : Event
    {
        public string clientName { get; set; }

        public JoinServerEvent(Sender sender) : base(sender)
        {

        }

        public override void OnClientReceive(LocalClient client)
        {
        }

        public override void OnServerReceive(LocalServer server)
        {

        }

        public override bool Validate(Level level)
        {
            //TODO: Implement Validation.
            return true;
        }
    }
}
