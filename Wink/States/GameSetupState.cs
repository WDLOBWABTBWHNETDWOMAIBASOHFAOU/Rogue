using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class GameSetupState:IGameLoopObject
    {
        public enum GameMode { Singleplayer, MultiplayerClient, MultiplayerHost, MultiplayerServer }
        GameMode gameMode;

        public void InitializeGameMode(GameMode gameMode)
        {
            this.gameMode = gameMode;
            switch (gameMode)
            {
                case GameMode.MultiplayerClient:
                    { //Initialize RemoteServer and LocalClient
                        RemoteServer server = new RemoteServer();
                        LocalClient client = new LocalClient(server);
                        break;
                    }
                case GameMode.MultiplayerHost:
                    {//Initialize LocalServer(public) and LocalClient
                        LocalServer server = new LocalServer(true);
                        LocalClient client = new LocalClient(server);
                        break;
                    }
                case GameMode.Singleplayer:
                    {//Initialize LocalServer(private) and LocalClient
                        LocalServer server = new LocalServer();
                        LocalClient client = new LocalClient(server);
                        server.SetupLevel(1);
                        PlayingState ps = GameEnvironment.GameStateManager.GetGameState("playingState") as PlayingState;
                        ps.setServer = server;
                        ps.setClient = client;                        
                        break;
                    }
                case GameMode.MultiplayerServer:
                    {//Initialize LocalServer(public)
                     //server = new LocalServer(true);
                        break;
                    }
            }
        }

        public void HandleInput(InputHelper inputHelper)
        {
            //throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            switch (gameMode)
            {
                case GameMode.MultiplayerClient:
                    throw new NotImplementedException();
                    break;
                case GameMode.MultiplayerHost:
                    throw new NotImplementedException();
                    break;
                case GameMode.Singleplayer:
                    GameEnvironment.GameStateManager.SwitchTo("playingState");
                    break;
                case GameMode.MultiplayerServer:
                    throw new NotImplementedException();
                    break;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            //throw new NotImplementedException();
        }

        public void Reset()
        {
            //throw new NotImplementedException();
        }
    }
}
