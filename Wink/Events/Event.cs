using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public abstract class Event : ISerializable
    {
        public class InvalidEventException : Exception { }

        //Should be set when received, before an OnReceive method is called.
        public Sender Sender { set; get; }

        public Event()
        {
        }

        public Event(SerializationInfo info, StreamingContext context)
        {/*
            string sender = info.GetString("Sender");
            if (sender == "server")
            {
                //Sender =     
            }
            else
            {
                //Sender = 
            }*/
        }

        public abstract bool GUIDSerialization { get; }

        public abstract bool OnClientReceive(LocalClient client);
        public abstract bool OnServerReceive(LocalServer server);

        /// <summary>
        /// This method is called once on the client before sending this event. 
        /// If the event is invalid, it is not sent.
        /// It is called again on the server as anti-cheating measure. (In case people use unofficial or modified clients)
        /// </summary>
        /// <param name="level"></param>
        /// <returns>A bool indicating whether or not this event is valid.</returns>
        public abstract bool Validate(Level level);

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (Sender is Server)
                info.AddValue("Sender", "server");
            else if (Sender is Client)
                info.AddValue("Sender", (Sender as Client).ClientName);
        }
    }
}
