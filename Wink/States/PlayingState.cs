using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class PlayingState : IGameLoopObject
    {
        public enum GameMode { Singleplayer, MultiplayerClient, MultiplayerHost, MultiplayerServer }

        private Server server;
        private Client client;

        public void InitializeGameMode(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.MultiplayerClient:
                    //Initialize RemoteServer and LocalClient
                    break;
                case GameMode.MultiplayerHost:
                    //Initialize LocalServer(public) and LocalClient
                    break;
                case GameMode.Singleplayer:
                    //Initialize LocalServer(private) and LocalClient
                    server = new LocalServer();
                    client = new LocalClient(server);
                    break;
                case GameMode.MultiplayerServer:
                    //Initialize LocalServer(public)
                    break;
            }
        }

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
