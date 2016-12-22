
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Wink
{
    public abstract class Server : Sender
    {
        private static Server instance;

        public static void Send(Event e)
        {
            instance.ReallySend(e);
        }

        public Server()
        {
            Debug.WriteLineIf(instance != null, "MULTIPLE SERVER INSTANCES MADE");

            instance = this;
        }

        protected abstract void ReallySend(Event e);

        public abstract void Update(GameTime gameTime);
    }
}
