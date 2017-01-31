using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

namespace Wink
{
    public class GameSetupState : GameObjectList
    {
        private class ClientField : GameObjectList
        {
            public ClientField(Client client, int nr)
            {
                // need to check if in multiplayer you can change the type of a different player
                SpriteFont textFieldFont = GameEnvironment.AssetManager.GetFont("Arial26");
                SelectField<SelectPlayer> SelectPlayer = new SelectField<SelectPlayer>(true, textFieldFont, Color.Red);
                SelectPlayer.Options = new List<SelectPlayer>() { new SelectPlayer(PlayerType.Warrior , client), new SelectPlayer(PlayerType.Archer, client), new SelectPlayer(PlayerType.Mage, client), new SelectPlayer(PlayerType.Random, client) };
                
                Add(SelectPlayer);

                Button setTypeButton = new Button("button", "Select Hero", textFieldFont, Color.Black);
                setTypeButton.Position = SelectPlayer.Position + new Vector2(SelectPlayer.Width, 0);
                setTypeButton.Action = () =>
                {
                    ClientPlayerType CPT = new ClientPlayerType(client.ClientName, client.playerType);
                    Server.Send(CPT);
                };
                Add(setTypeButton);

                TextGameObject name = new TrackingTextGameObject<Client>(client, c => nr + ". " + c.ClientName, "Arial26");
                name.Color = Color.White;
                name.Position = setTypeButton.Position + new Vector2(setTypeButton.Width, 0);               
                Add(name);
            }

            private class SelectPlayer : SelectField<SelectPlayer>.OptionAction
            {
                PlayerType pType;
                Client c;

                /// <summary>
                /// select Hero Classes
                /// </summary>
                /// <param name="pType"></param>
                /// <param name="c"></param>
                public SelectPlayer(PlayerType pType, Client c)
                {
                    this.pType = pType;
                    this.c = c;
                }

                public override string ToString()
                {
                    return pType.ToString();
                }

                public void execute()
                {
                    c.playerType = pType;
                }
            }
        }

        public enum GameMode { Singleplayer, MultiplayerClient, MultiplayerHost, MultiplayerServer }

        private TcpListener tcpListener;

        private Server server;
        private List<Client> clients;
        
        private Button startButton;
        private Button backButton;

        public GameSetupState()
        {
            clients = new List<Client>();

            //Create a button to go back to the main menu.
            SpriteFont arial26 = GameEnvironment.AssetManager.GetFont("Arial26");
            backButton = new Button("button", "Back", arial26, Color.Black);
            backButton.Action = () =>
            {
                Reset();
                GameEnvironment.GameStateManager.SwitchTo("mainMenuState");
            };
            backButton.Position = new Vector2(100, Treehugger.Screen.Y - 100);
            Add(backButton);
        }

        private void AddStartGame()
        {
            Point screen = GameEnvironment.Screen;
            SpriteFont arial26 = GameEnvironment.AssetManager.GetFont("Arial26");
            startButton = new Button("button", "Start Game", arial26, Color.Black);
            startButton.Action = () =>
            {
                foreach (Client c in clients)
                {
                    //send player type to the server - extra check to make sure correct type is known
                    ClientPlayerType CPT = new ClientPlayerType(c.ClientName, c.playerType);
                    Server.Send(CPT);
                }
                LocalServer ls = (LocalServer)server;
                ls.SetupLevel(1);

                GameEnvironment.GameStateManager.SwitchTo("playingState");
                GameEnvironment.AssetManager.StopMusic();
            };
            startButton.Position = new Vector2(screen.X - startButton.Width - 50, screen.Y  - startButton.Height - 50);
            Add(startButton);
        }

        private void AddClient(Client c)
        {
            clients.Add(c);
            if (server is LocalServer)
                (server as LocalServer).AddClient(c);

            ClientField cf = new ClientField(c, clients.Count);
            cf.Position = new Vector2(100, 100 * clients.Count);
            Add(cf);
        }

        public void InitializeGameMode(GameMode gameMode)
        {
            PlayingState ps = GameEnvironment.GameStateManager.GetGameState("playingState") as PlayingState;
            Reset();
            ps.Reset();
            ps.CurrentGameMode = gameMode;
            switch (gameMode)
            {
                case GameMode.MultiplayerClient:
                    //Initialize RemoteServer and LocalClient
                    LocalClient lc = new LocalClient(server);
                    AddClient(lc);
                    server = new RemoteServer(lc);
                    ps.SetClientAndServer(lc, server);
                    break;
                case GameMode.MultiplayerHost:
                    //Initialize LocalServer(public) and LocalClient
                    server = new LocalServer();
                    AddClient(new LocalClient(server));
                    ps.SetClientAndServer(clients[0], server);
                    tcpListener = new TcpListener(IPAddress.Any, 29793);
                    tcpListener.Start();
                    AddStartGame();
                    break;
                case GameMode.Singleplayer:
                    //Initialize LocalServer(private) and LocalClient
                    server = new LocalServer();
                    AddClient(new LocalClient(server));
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
            if (tcpListener != null && tcpListener.Pending())
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Client newClient = new RemoteClient((LocalServer)server, tcpClient);
                AddClient(newClient);
            }

            if (server is RemoteServer)
            { //server == remote -> client is local
                clients[0].Update(gameTime);
            }
            else
            { 
                foreach (Client c in clients)
                {
                    (server as LocalServer).ProcessAllNonActionEvents();
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            clients = new List<Client>();
            server = null;
            startButton = null;
            if (tcpListener != null)
                tcpListener.Stop();
            tcpListener = null;
        }
    }
}
