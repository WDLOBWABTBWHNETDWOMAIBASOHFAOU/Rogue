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
        private List<Living> livingObjects;
        private Level level;
        private int levelIndex;

        bool levelChanged;
        private int turnIndex;

        public Level Level
        {
            get { return level; }
            set
            {
                level = value;
                InitLivingObjects();
            }
        }

        public int LevelIndex
        {
            get { return levelIndex; }
        }

        public LocalServer ()
        {
        }

        public void SetupLevel(int levelIndex, List<Client> clients)
        {
            level = new Level(levelIndex);
            //level = new Level();

            this.clients = clients;
            foreach (Client c in clients)
            {
                Player player = new Player(c.ClientName, Level.Layer);
                player.MoveTo(Level.Find("startTile") as Tile);
            }

            InitLivingObjects();
            SendOutUpdatedLevel(true);
        }

        public void InitLivingObjects()
        {
            livingObjects = Level.FindAll(obj => obj is Living).Cast<Living>().ToList();
            livingObjects.Sort((obj1, obj2) => obj1.Dexterity - obj2.Dexterity);
            livingObjects.ElementAt(turnIndex).isTurn = true;
        }

        protected override void ReallySend(Event e)
        {
            if (e.Validate(Level))
            {
                e.OnServerReceive(this);
            }
        }

        private void SendOutUpdatedLevel(bool first = false)
        {
            foreach(Client c in clients)
            {
                //Passing null because whenever a client receives an event it'll always be from the server.
                LevelUpdatedEvent e = first ? new JoinedServerEvent(Level) : new LevelUpdatedEvent(Level);
                c.Send(e);
            }
        }

        public override void Update(GameTime gameTime)
        {
            Level.Root.Update(gameTime);
            UpdateTurn();
            if (levelChanged)
            {
                SendOutUpdatedLevel();
                levelChanged = false;
            }
        }
        
        public void LevelChanged()
        {
            levelChanged = true;
        }

        private void UpdateTurn()
        {
            if (livingObjects.ElementAt(turnIndex).ActionPoints <= 0)
            {
                livingObjects.ElementAt(turnIndex).isTurn = false;
                turnIndex = (turnIndex + 1) % livingObjects.Count;
                livingObjects.ElementAt(turnIndex).isTurn = true;
                livingObjects.ElementAt(turnIndex).ActionPoints = 4;
            }
        }
    }
}
