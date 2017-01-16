using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace Wink
{
    public abstract class Server : Sender
    {
        protected static Server instance;
        
        public static void Send(Event e)
        {
            instance.ReallySend(e);
        }

        public Server()
        {
            //TODO: Currently while using the restart buttons, PlayingState is initialized again, and thus a new Server is constructed. Find out if this is desirable/problematic.
            Debug.WriteLineIf(instance != null, "MULTIPLE SERVER INSTANCES MADE");

            instance = this;
        }
        
        protected abstract void ReallySend(Event e);

        public abstract void Update(GameTime gameTime);
        public abstract void Reset();
    }
}
