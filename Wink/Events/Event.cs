using System;

namespace Wink
{
    [Serializable()]
    public abstract class Event
    {
        public abstract void OnClientReceive(LocalClient client);
        public abstract void OnServerReceive(LocalServer server);

        /// <summary>
        /// This method is called once on the client before sending this event. 
        /// If the event is invalid, it is not sent.
        /// It is called again on the server as anti-cheating measure. (In case people use unofficial or modified clients)
        /// </summary>
        /// <returns>A bool indicating whether or not this event is valid.</returns>
        public abstract bool Validate();
    }
}
