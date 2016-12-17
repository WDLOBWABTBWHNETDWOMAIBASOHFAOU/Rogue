using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class PlayingState : IGameLoopObject
    {
        private Server server;
        public Server setServer { set { server = value; } }
        private Client client;
        public Client setClient { set { client = value; } }
        

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if(client != null && client is LocalClient)
            {
                LocalClient lc = (LocalClient)client;
                lc.Draw(gameTime, spriteBatch, camera);
            }
                
        }

        public void Update(GameTime gameTime)
        {
            //First update client so it can read input and send appropriate events.
            if (client is LocalClient && client != null)
            {
                LocalClient lc = (LocalClient)client;
                lc.Update(gameTime);
            }
            //Then update server so it can process events and send back the new level state.
            if (server is LocalServer && server != null)
            {
                LocalServer ls = (LocalServer)server;
                ls.Update(gameTime);
            }
        }

        public void HandleInput(InputHelper inputHelper)
        {
            if (client is LocalClient && client != null)
            {
                LocalClient lc = (LocalClient)client;
                lc.HandleInput(inputHelper);
            }
        }

        public void Reset()
        {
            
        }
    }
}
