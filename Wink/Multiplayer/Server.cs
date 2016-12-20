
using Microsoft.Xna.Framework;

namespace Wink
{
    public abstract class Server : Sender
    {
        
        public Server()
        {
        }

        public abstract void Send(Event e);

        public abstract void Update(GameTime gameTime);
    }
}
