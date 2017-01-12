using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wink
{
    public class LocalServer : Server, ILocal
    {
        private Dictionary<Client, List<Event>> clientEvents;
        public List<Client> Clients
        {
            get { return new List<Client>(clientEvents.Keys); }
        }

        private List<Living> livingObjects;
        private Level level;
        
        private int turnIndex;

        private List<GameObject> changedObjects;
        public List<GameObject> ChangedObjects
        {
            get { return changedObjects; }
        }

        public Level Level
        {
            get { return level; }
            set
            {
                level = value;
                InitLivingObjects();
                SendOutUpdatedLevel();
            }
        }

        public int LevelIndex
        {
            get { return level.Index; }
        }

        public LocalServer ()
        {
            changedObjects = new List<GameObject>();
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
            level = new Level(levelIndex);
            
            for (int i = 0; i < Clients.Count; i++)
            {
                Client c = Clients[i];
                Player player = new Player(c.ClientName, Level.Layer);
                player.MoveTo(Level.Find("StartTile" + (i + 1)) as Tile);
            }

            InitLivingObjects();
            SendOutUpdatedLevel(true);
        }
        
        public void ProcessEvents(Client c)
        {
            foreach (Event e in clientEvents[c])
            {
                if (e.Validate(Level))
                {
                    e.Sender = c;
                    e.OnServerReceive(this);
                }
            }
            clientEvents[c].Clear();
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
                e = SerializationHelper.Clone(e, this, e.GUIDSerialization);
                IncomingEvent(Clients.Find(c => c is LocalClient), e);
            }
        }

        private void SendOutUpdatedLevel(bool first = false)
        {
            LevelUpdatedEvent e = first ? new JoinedServerEvent(Level) : new LevelUpdatedEvent(Level);
            using (MemoryStream ms = new MemoryStream())
            {
                SerializationHelper.Serialize(ms, e, this, e.GUIDSerialization);
                ms.Seek(0, SeekOrigin.Begin);
                foreach (Client c in Clients)
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
                foreach (Client c in Clients)
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

            Client currentClient = Clients.Find(client => client.Player.GUID == livingObjects[turnIndex].GUID);
            ProcessEvents(currentClient);

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
                turnIndex = (turnIndex + 1) % livingObjects.Count;
                livingObjects[turnIndex].ActionPoints = 4;
                //changedObjects.Add(livingObjects[turnIndex]);
                SendOutUpdatedLevel();
            }
        }

        // very laggy, but does show you what can be hit
        private void Hitable(Player player, TileField tf)
        {
            Enemy dummy = new Enemy(1);
            for(int x = 0; x < tf.Columns; x++)
            {
                for(int y = 0; y < tf.Rows; y++)
                {
                    Tile t = tf[x, y] as Tile;
                    dummy.Position = t.TilePosition.ToVector2() * tf.CellHeight + dummy.Origin;

                    int dx = (int)Math.Abs(player.Position.X - dummy.Position.X) - Tile.TileWidth / 2;
                    int dy = (int)Math.Abs(player.Position.Y - dummy.Position.Y) - Tile.TileHeight / 2;

                    double distance = Math.Abs(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    double reach = Tile.TileWidth * player.Reach;

                    bool withinReach = distance <= reach;

                    if (withinReach && !AttackEvent.Blocked(player, dummy) && t.TileType == TileType.Floor)
                    {
                        //t.spriteAssetName = "empty:65:65:10:DarkRed";
                        //t.LoadSprite();
                    }
                    else
                    {
                        //t.spriteAssetName = t.AssetName;
                        //t.LoadSprite();
                    }
                }
            }
        }
        
        public void EndTurn(Player player)
        {
            if (livingObjects[turnIndex] == player)
            {
                turnIndex = (turnIndex + 1) % livingObjects.Count;
            }
        }

        public override void Reset()
        {
            foreach (Client client in Clients)
            {
                client.Reset();
            }
        }
    }
}
