using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Wink
{
    public abstract class Server : Sender
    {
        class ServerNotLocalException : Exception { }

        private static Server instance;

        public static void Send(Event e)
        {
            instance.ReallySend(e);
        }

        /// <summary>
        /// Method is used to deserialize based on GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectByGUID(Guid guid)
        {
            if (instance is RemoteServer)
                throw new ServerNotLocalException();

            LocalServer server = instance as LocalServer;
            GameObject obj = server.Level.Find(o => o.GUID == guid);
            return obj;
        }

        public Server()
        {
            //TODO: Currently while using the restart buttons, PlayingState is initialized again, and thus a new Server is constructed. Find out if this is desirable/problematic.
            Debug.WriteLineIf(instance != null, "MULTIPLE SERVER INSTANCES MADE");

            instance = this;
        }

        protected abstract void ReallySend(Event e);

        public abstract void Update(GameTime gameTime);
    }
}
