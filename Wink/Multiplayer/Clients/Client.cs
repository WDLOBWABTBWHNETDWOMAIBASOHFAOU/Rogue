
using Microsoft.Xna.Framework;
using System.IO;

namespace Wink
{
    public abstract class Client : Sender
    {
        protected Server server;
        public PlayerType playerType;
        public string ClientName { get; set; }

        public Client(Server server)
        {
            this.server = server;
        }
        
        public abstract void SendPreSerialized(MemoryStream ms);
        public abstract Player Player { get; }
        public abstract void Update(GameTime gameTime);
        public abstract void Reset();
    }
}
