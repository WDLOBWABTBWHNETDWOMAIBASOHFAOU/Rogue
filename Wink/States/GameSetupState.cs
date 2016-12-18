

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Wink
{
    class GameSetupState : GameObjectList
    {
        public enum GameMode { Singleplayer, MultiplayerClient, MultiplayerHost, MultiplayerServer }

        private TcpListener tcpListener;

        private Server server;
        private List<Client> clients;
        
        private Button startButton;

        public GameSetupState()
        {
            //SelectField for heroes.
            //SelectField<> sf = new SelectField<>();
            clients = new List<Client>();
            
        }

        private void AddStartGame()
        {
            Point screen = GameEnvironment.Screen;
            SpriteFont arial26 = GameEnvironment.AssetManager.GetFont("Arial26");
            startButton = new Button("button", "Start Game", arial26, Color.Black);
            startButton.Position = new Vector2(screen.X - startButton.Width - 50, screen.Y  - startButton.Height - 50);
            Add(startButton);
        }

        public void InitializeGameMode(GameMode gameMode)
        {
            PlayingState ps = GameEnvironment.GameStateManager.GetGameState("playingState") as PlayingState;
            switch (gameMode)
            {
                case GameMode.MultiplayerClient:
                    //Initialize RemoteServer and LocalClient
                    LocalClient lc = new LocalClient(server);
                    clients.Add(lc);
                    server = new RemoteServer(lc);
                    ps.SetClientAndServer(lc, server);
                    break;
                case GameMode.MultiplayerHost:
                    //Initialize LocalServer(public) and LocalClient
                    server = new LocalServer();
                    clients.Add(new LocalClient(server));
                    ps.SetClientAndServer(clients[0], server);
                    tcpListener = new TcpListener(IPAddress.Any, 29793);
                    tcpListener.Start();
                    AddStartGame();
                    break;
                case GameMode.Singleplayer:
                    //Initialize LocalServer(private) and LocalClient
                    server = new LocalServer();
                    clients.Add(new LocalClient(server));
                    ps.SetClientAndServer(clients[0], server);
                    AddStartGame();
                    break;
                case GameMode.MultiplayerServer:
                    //Initialize LocalServer(public)
                    //server = new LocalServer(true);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            /*
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
            */
            if (tcpListener != null && tcpListener.Pending())
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Client newClient = new RemoteClient((LocalServer)server, tcpClient);
                clients.Add(newClient);
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (startButton != null && startButton.Pressed)
            {
                LocalServer ls = (LocalServer)server;
                ls.SetupLevel(1, clients);

                GameEnvironment.GameStateManager.SwitchTo("playingState");
            }
        }
    }
}
