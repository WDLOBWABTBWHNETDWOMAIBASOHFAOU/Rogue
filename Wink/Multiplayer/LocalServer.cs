using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wink
{
    public class LocalServer : Server, ILocal
    {
        private List<Client> clients;
        private List<Living> livingObjects;
        private Level level;
        private int levelIndex;
        
        private int turnIndex;

        private List<GameObject> changedObjects;
        public List<GameObject> ChangedObjects {
            get { return changedObjects; }
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
            get { return levelIndex; }
        }

        public LocalServer ()
        {
            changedObjects = new List<GameObject>();
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

        public void SetupLevel(int levelIndex, List<Client> clients)
        {
            //level = new Level(levelIndex);
            level = new Level();

            this.clients = clients;
            for (int i = 0; i < clients.Count; i++)
            {
                Client c = clients[i];
                Player player = new Player(c.ClientName, Level.Layer);
                player.MoveTo(Level.Find("StartTile" + (i + 1)) as Tile);
            }

            InitLivingObjects();
            SendOutUpdatedLevel(true);
        }

        public void InitLivingObjects()
        {
            livingObjects = Level.FindAll(obj => obj is Living).Cast<Living>().ToList();
            livingObjects.Sort((obj1, obj2) => obj1.Dexterity - obj2.Dexterity);
            //livingObjects.ElementAt(turnIndex).isTurn = true;
        }

        protected override void ReallySend(Event e)
        {
            e = SerializationHelper.Clone(e, this, e.GUIDSerialization);
            
            if (e.Validate(Level))
                e.OnServerReceive(this);
        }

        private void SendOutUpdatedLevel(bool first = false)
        {
            LevelUpdatedEvent e = first ? new JoinedServerEvent(Level) : new LevelUpdatedEvent(Level);
            using (MemoryStream ms = new MemoryStream())
            {
                SerializationHelper.Serialize(ms, e, this, e.GUIDSerialization);
                ms.Seek(0, SeekOrigin.Begin);
                foreach (Client c in clients)
                {
                    c.SendPreSerialized(ms);
                }
            }
        }

        public void SendOutLevelChanges()
        {
            if (changedObjects.Count > 0)
            {
                LevelChangedEvent e = new LevelChangedEvent(changedObjects);
                foreach (Client c in clients)
                {
                    c.Send(e);
                }
                changedObjects.Clear();
            }
        }

        public override void Update(GameTime gameTime)
        {
            Level.Update(gameTime);
            while (!(livingObjects[turnIndex] is Player))
            {
                changedObjects.AddRange(livingObjects[turnIndex].DoAllBehaviour());
                UpdateTurn();
            }

            Client currentClient = clients.Find(client => client.Player == livingObjects[turnIndex]);
            if (currentClient is RemoteClient)
                (currentClient as RemoteClient).ProcessEvents();

            UpdateTurn();
            if (changedObjects.Count > 0)
            {
                SendOutUpdatedLevel();
                changedObjects.Clear();
            }
            //SendOutLevelChanges();
        }

        private void UpdateTurn()
        {
            if (livingObjects[turnIndex].ActionPoints <= 0)
            {
                //livingObjects.ElementAt(turnIndex).isTurn = false;
                turnIndex = (turnIndex + 1) % livingObjects.Count;
                //livingObjects.ElementAt(turnIndex).isTurn = true;
                livingObjects[turnIndex].ActionPoints = 4;
                //changedObjects.Add(livingObjects[turnIndex]);
                SendOutUpdatedLevel();
            }
        }

        public void EndTurn(Player player)
        {
            if (livingObjects[turnIndex] == player)
            {
                turnIndex++;
            }
        }

        public override void Reset()
        {
            foreach (Client client in clients)
            {
                client.Reset();
            }
        }
    }
}
