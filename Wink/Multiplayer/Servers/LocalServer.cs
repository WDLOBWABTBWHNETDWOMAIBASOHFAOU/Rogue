﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wink
{
    public class LocalServer : Server, ILocal
    {
        public static void SendToClients(Event e)
        {
            if (instance is LocalServer)
                (instance as LocalServer).SendToAllClients(e);
            else
                throw new Exception("Server is not local.");
        }

        private Dictionary<Client, List<Event>> clientEvents;
        private List<Living> livingObjects;
        private Level level;
        private int turnIndex;

        public List<Client> Clients
        {
            get { return new List<Client>(clientEvents.Keys); }
        }
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
            get { return level.Index; }
        }
        private List<Player> Players
        {
            get { return livingObjects.Where(l => l is Player).Cast<Player>().ToList(); }
        }

        public LocalServer ()
        {
            clientEvents = new Dictionary<Client, List<Event>>();
        }

        /// <summary>
        /// Method is used to deserialize based on GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public GameObject GetGameObjectByGUID(Guid guid)
        {
            if (guid == Guid.Empty)
                return null;

            GameObject obj = Level.Find(o => o.GUID == guid);
            return obj;
        }

        public void AddClient(Client client)
        {
            clientEvents.Add(client, new List<Event>());
        }

        public void SetupLevel(int levelIndex)
        {
            //Make the level.
            level = new Level(levelIndex);
            
            //Create a player for each connected client.
            for (int i = 0; i < Clients.Count; i++)
            {
                Client c = Clients[i];
                Player player = new Player(c.ClientName, Level.Layer, c.playerType);
                
                //Put the player on a startTile.
                (Level.Find("StartTile" + (i + 1)) as Tile).PutOnTile(player);
                player.ComputeVisibility();
            }

            InitLivingObjects();
            SendOutUpdatedLevel(true);
        }
        
        public void ProcessAllNonActionEvents()
        {
            foreach (Client c in clientEvents.Keys)
            {
                List<Event> done = new List<Event>();
                foreach (Event e in clientEvents[c])
                {
                    if (!(e is ActionEvent) && e.Validate(Level))
                    {
                        e.Sender = c;
                        if (e.OnServerReceive(this))
                            done.Add(e);
                    }
                }

                foreach (Event e in done)
                    clientEvents[c].Remove(e);
            }
        }

        public void ProcessActionEvents(Client c)
        {
            if (clientEvents[c].Count > 0)
            {
                Event e = clientEvents[c][0];
                if (e is ActionEvent)
                {
                    if (e.Validate(Level))
                    {
                        e.Sender = c;
                        e.OnServerReceive(this);
                    }
                    clientEvents[c].Remove(e);
                }
            }
        }

        public void IncomingEvent(Client c, Event e)
        {
            clientEvents[c].Add(e);
        }

        public void InitLivingObjects()
        {
            livingObjects = Level.FindAll(obj => obj is Living).Cast<Living>().ToList();
            livingObjects.Sort((obj1, obj2) => obj1.Dexterity - obj2.Dexterity);
            turnIndex = livingObjects.Count - 1;
        }

        protected override void ReallySend(Event e)
        {
            if (e.Validate(Level))
            {
                e = SerializationHelper.Clone(e, this);
                IncomingEvent(Clients.Find(c => c is LocalClient), e);
            }
        }

        private void SendOutUpdatedLevel(bool first = false)
        {
            LevelUpdatedEvent e = first ? new JoinedServerEvent(Level) : new LevelUpdatedEvent(Level);
            SendToAllClients(e);
        }

        private bool updatedLevelSent;
        private void SendToAllClients(Event e)
        {
            if (!updatedLevelSent) //After a LevelUpdatedEvent nothing else should be sent, because during deserialization the old level will still be used.
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    SerializationHelper.Serialize(ms, e, this, e.GetFullySerialized(Level));
                    ms.Seek(0, SeekOrigin.Begin);
                    foreach (Client c in Clients)
                        c.SendPreSerialized(ms);
                }

                if (e is LevelUpdatedEvent)
                    updatedLevelSent = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            updatedLevelSent = false;
            //First we update the level so all update methods in all gameobjects get called.
            //This also empties the seenBy in Tile.
            Level.Update(gameTime);
            //Then we let every Living object calculate what tiles it can see, that object is then added to these Tiles' seenBy list.
            ComputeVisibilities();
            
            ProcessAllNonActionEvents();
            
            if (livingObjects[turnIndex] is Player)
            {
                Client currentClient = Clients.Find(client => client.Player.GUID == livingObjects[turnIndex].GUID);
                ProcessActionEvents(currentClient);
            }
            else
            {
                HashSet<GameObject> changedObjects = livingObjects[turnIndex].DoAllBehaviour();
                if (changedObjects.Count > 0)
                    SendToAllClients(new LevelChangedEvent(changedObjects));    
            }

            livingObjects[turnIndex].ComputeVisibility();
            UpdateTurn();
        }

        public void ComputeVisibilities()
        {
            foreach (Player p in Players)
                p.ComputeVisibility();

            foreach (Living l in livingObjects.Where(l => !(l is Player)))
            {
                bool seenByPlayer = false;
                foreach (Player p in Players)
                {
                    if (l.Tile.SeenBy.ContainsKey(p))
                    {
                        seenByPlayer = true;
                        break;
                    }
                }
                if (seenByPlayer)
                    l.ComputeVisibility();
            }
        }

        private void UpdateTurn()
        {
            Living turn = livingObjects[turnIndex];
            livingObjects.RemoveAll(l => l.Health <= 0); //Remove all the dead.
            turnIndex = livingObjects.IndexOf(turn);

            if (livingObjects[turnIndex].ActionPoints <= 0)
            {
                turnIndex = (turnIndex + 1) % livingObjects.Count;
                livingObjects[turnIndex].ActionPoints = Living.MaxActionPoints;
                if (livingObjects[turnIndex] is Player && !updatedLevelSent)
                    SendToAllClients(new LevelChangedEvent(new List<GameObject>() { livingObjects[turnIndex] }));
            }
        }
        
        public void EndTurn(Player player)
        {
            if (livingObjects[turnIndex] == player)
            {
                player.ActionPoints = 0;
                SendToAllClients(new LevelChangedEvent(new List<GameObject>() { player }));
                ComputeVisibilities();
                UpdateTurn();
            }
        }

        public override void Reset()
        {
            foreach (Client client in Clients)
                client.Reset();
        }
    }
}
