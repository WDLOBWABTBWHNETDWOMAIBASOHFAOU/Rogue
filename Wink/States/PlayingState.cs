using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class PlayingState : IGameLoopObject
    {
        private Server server;
        private Client client;

        GameSetupState.GameMode currentGameMode;
        public GameSetupState.GameMode CurrentGameMode { get { return currentGameMode; } set { currentGameMode = value; } }

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

                // temp gameover check
                Player player = ls.Level.Find((p) => p.GetType() == typeof(Player)) as Player;
                if (player.health<=0)
                {

                    GameOverState gos = GameEnvironment.GameStateManager.GetGameState("gameOverState") as GameOverState;
                    gos.GameMode = currentGameMode;
                    GameEnvironment.GameStateManager.SwitchTo("gameOverState");
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
            
        }
    }
}
