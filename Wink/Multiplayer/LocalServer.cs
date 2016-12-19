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

        bool levelChanged;
        private int turnIndex;

        public Level Level { get; private set; }
        
        public LocalServer ()
        {
        }

        public void SetupLevel(int level, List<Client> clients)
        {
            Level = new Level(level);

            this.clients = clients;
            foreach (Client c in clients)
            {
                //Player adds itself to level.
                Player player = new Player(c, Level, Level.Layer + 1);
            }

            livingObjects = Level.FindAll(obj => obj is Living).Cast<Living>().ToList();
            livingObjects.Sort((obj1, obj2) => obj1.Dexterity - obj2.Dexterity);
            livingObjects.ElementAt(turnIndex).isTurn = true;

            SendOutUpdatedLevel(true);
        }

        public override void Send(Event e)
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
                LevelUpdatedEvent e = first ? new JoinedServerEvent(null) : new LevelUpdatedEvent(null); 
                e.updatedLevel = Level;
                c.Send(e);
            }
        }

        public void Update(GameTime gameTime)
        {
            Level.Update(gameTime);
            UpdateTurn();
            level.Root.Update(gameTime);
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
                turnIndex = (turnIndex +1)% livingObjects.Count;
                livingObjects.ElementAt(turnIndex).isTurn = true;
                livingObjects.ElementAt(turnIndex).ActionPoints = 4;
            }
        }
    }
}
