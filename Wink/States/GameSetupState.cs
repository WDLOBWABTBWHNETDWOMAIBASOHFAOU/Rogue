﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Wink
{
    public class GameSetupState : GameObjectList
    {
        private class ClientField : GameObjectList
        {
            public ClientField(Client client, int nr)
            {
                TextGameObject name = new TrackingTextGameObject<Client>(client, c => nr + ". " + c.ClientName, "Arial26");
                name.Color = Color.White;
                Add(name);

                //SelectField for heroes.
                //SelectField<> sf = new SelectField<>();
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
                LocalServer ls = (LocalServer)server;
                ls.SetupLevel(1, clients);
                GameEnvironment.GameStateManager.SwitchTo("playingState");
            };
            startButton.Position = new Vector2(screen.X - startButton.Width - 50, screen.Y  - startButton.Height - 50);
            Add(startButton);
        }

        private void AddClient(Client c)
        {
            clients.Add(c);
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
                server.Update(gameTime);

            foreach (Client c in clients)
            {
                if (c is RemoteClient)
                    (c as RemoteClient).ProcessInitialEvent();
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
