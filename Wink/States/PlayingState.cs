using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class PlayingState : IGameLoopObject
    {
        private Server server;
        private Client client;

        GameSetupState.GameMode currentGameMode;
        public GameSetupState.GameMode CurrentGameMode {
            get { return currentGameMode; }
            set { currentGameMode = value; }
        }

        public void SetClientAndServer(Client client, Server server)
        {
            this.client = client;
            this.server = server;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if(client != null && client is LocalClient)
            {
                LocalClient lc = (LocalClient)client;
                lc.Draw(gameTime, spriteBatch, camera);
            }
        }

        public virtual void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (client != null && client is LocalClient)
            {
                LocalClient lc = (LocalClient)client;
                lc.DrawDebug(gameTime, spriteBatch, camera);
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

            if (server != null)
            {
                //Then update server so it can process events and send back the new level state.
                server.Update(gameTime);

                if (server is LocalServer)
                {
                    LocalServer ls = server as LocalServer;
                    // temp gameover check
                    Player player = ls.Level.Find((p) => p.GetType() == typeof(Player)) as Player;
                    if (player != null && player.Health <= 0)
                    {
                        GameOverState gos = GameEnvironment.GameStateManager.GetGameState("gameOverState") as GameOverState;
                        gos.GameMode = currentGameMode;
                        GameEnvironment.GameStateManager.SwitchTo("gameOverState");
                    }
                }
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
            if (server != null)
                server.Reset();
        }
    }
}
