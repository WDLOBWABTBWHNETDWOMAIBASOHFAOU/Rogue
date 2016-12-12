using System;

namespace Wink
{
    [Serializable()]
    public abstract class Event
    {
        public abstract void OnClientReceive(LocalClient client);
        public abstract void OnServerReceive(LocalServer server);
    }
}
