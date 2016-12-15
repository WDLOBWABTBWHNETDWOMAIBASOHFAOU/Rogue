using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System;

namespace Wink
{
    public class LocalServer : Server
    {
        private List<Client> clients;
        public Level Level { get; }
        private List<Living> livingObjects;
        bool levelChanged;

        private int turnIndex;

        private bool isPublic;
        private TcpListener tcpListener;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicServer">Wether or not the server can be joined by others.</param>
        public LocalServer(bool publicServer = false)
        {
            clients = new List<Client>();

            Level l = new Level(1);
            Level = l;

            livingObjects = Level.FindAll(obj => obj is Living).Cast<Living>().ToList();
            livingObjects.Sort((obj1, obj2)=> obj1.Dexterity - obj2.Dexterity);

            if (publicServer)
            {
                MakePublic();
            }
        }

        public override void Send(Event e)
        {
            if (e.Validate(Level))
            {
                e.OnServerReceive(this);
                UpdateTurn();
            }
        }

        public override void AddLocalClient(LocalClient localClient)
        {
            clients.Add(localClient);
            ClientAdded(localClient);
        }

        private void ClientAdded(Client client)
        {
            Player player = new Player(client, Level,Level.Layer+1);

            AddPlayer(player);
        
            SendOutUpdatedLevel();
        }

        private void AddPlayer(Living player)
        {
            for (int i = 0; i < livingObjects.Count; i++)
            {
                if (livingObjects[i].Dexterity > player.Dexterity)
                {
                    livingObjects.Insert(i, player);
                    return;
                }
            }
            livingObjects.Add(player);
        }

        private void SendOutUpdatedLevel()
        {
            foreach(Client c in clients)
            {
                LevelUpdatedEvent e = new LevelUpdatedEvent();
                e.updatedLevel = Level;
                c.Send(e);
            }
        }

        private void MakePublic()
        {
            isPublic = true;
            tcpListener = new TcpListener(IPAddress.Any, 29793);
            tcpListener.Start();
        }

        public void Update(GameTime gameTime)
        {
            Level.Update(gameTime);

            if (levelChanged)
            {
                SendOutUpdatedLevel();
                levelChanged = false;
            }
            if (isPublic)
            {
                if (tcpListener.Pending())
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    Client newClient = new RemoteClient(this, tcpClient);
                    clients.Add(newClient);
                }
            }
        }
        
        public void LevelChanged()
        {
            levelChanged = true;
        }

        public void UpdateTurn()
        {
            if( livingObjects.ElementAt(turnIndex).ActionPoints <= 0)
            {
                livingObjects.ElementAt(turnIndex).isTurn = false;
                turnIndex = (turnIndex +1)% livingObjects.Count;
                livingObjects.ElementAt(turnIndex).isTurn = true;
                livingObjects.ElementAt(turnIndex).ActionPoints = 4;
            }
        }
    }
}
