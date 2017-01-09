using System;

namespace Wink
{
    [Serializable]
    public class JoinServerEvent : Event
    {
        private string clientName;

        public JoinServerEvent(string clientName) : base()
        {
            this.clientName = clientName;
        }

        public override bool GUIDSerialization
        {
            get { return false; }
        }

        public override void OnClientReceive(LocalClient client)
        {
        }

        public override void OnServerReceive(LocalServer server)
        {
            (Sender as RemoteClient).ClientName = clientName;
        }

        public override bool Validate(Level level)
        {
            //TODO: Implement Validation.
            return true;
        }
    }
}
