using Microsoft.Xna.Framework;
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
        private List<GameObject> changedObjects;

        public List<Client> Clients
        {
            get { return new List<Client>(clientEvents.Keys); }
        }
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
                SendOutUpdatedLevelIf();
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

                /*TEST PLAYER SET UP, REPLACE WHITH SELECTED HERO*/

                //armor
                ItemSlot slot_0_0 =player.Inventory.ItemGrid[0, 0] as ItemSlot;
                slot_0_0.ChangeItem(new BodyEquipment(2, 3, ArmorType.heavy));
                ItemSlot slot_1_0 = player.Inventory.ItemGrid[1, 0] as ItemSlot;
                slot_1_0.ChangeItem(new BodyEquipment(2, 3, ArmorType.normal));
                ItemSlot slot_2_0 = player.Inventory.ItemGrid[2, 0] as ItemSlot;
                slot_2_0.ChangeItem(new BodyEquipment(2, 3, ArmorType.light));
                ItemSlot slot_3_0 = player.Inventory.ItemGrid[3, 0] as ItemSlot;
                slot_3_0.ChangeItem(new BodyEquipment(2, 3, ArmorType.robes));

                //weapons
                ItemSlot slot_0_1 = player.Inventory.ItemGrid[0, 1] as ItemSlot;
                slot_0_1.ChangeItem(new WeaponEquipment(2,WeaponType.melee));
                ItemSlot slot_1_1 = player.Inventory.ItemGrid[1, 1] as ItemSlot;
                slot_1_1.ChangeItem(new WeaponEquipment(2, WeaponType.bow));
                ItemSlot slot_2_1 = player.Inventory.ItemGrid[2, 1] as ItemSlot;
                slot_2_1.ChangeItem(new WeaponEquipment(2, WeaponType.staff));

                //potions and rings
                ItemSlot slot_0_2 = player.Inventory.ItemGrid[0, 2] as ItemSlot;
                slot_0_2.ChangeItem(new Potion(2,10,PotionType.Health));
                ItemSlot slot_1_2 = player.Inventory.ItemGrid[1, 2] as ItemSlot;
                slot_1_2.ChangeItem(new Potion(2, 10, PotionType.Mana));
                ItemSlot slot_2_2 = player.Inventory.ItemGrid[2, 2] as ItemSlot;
                slot_2_2.ChangeItem(new RingEquipment("empty:64:64:10:Pink"));

                //player stats
                player.SetStats(2, 3, 3, 3, 3, 3, 3, 20, 5, 1);

                /*END PLAYER SETUP*/

                (Level.Find("StartTile" + (i + 1)) as Tile).PutOnTile(player);
                player.ComputeVisibility();
            }

            InitLivingObjects();
            SendOutUpdatedLevelIf(true);
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
                e = SerializationHelper.Clone(e, this, e.GUIDSerialization);
                IncomingEvent(Clients.Find(c => c is LocalClient), e);
            }
        }

        private void SendOutUpdatedLevelIf(bool first = false)
        {
            if (changedObjects.Count > 0 || first)
            {
                LevelUpdatedEvent e = first ? new JoinedServerEvent(Level) : new LevelUpdatedEvent(Level);
                SendToAllClients(e);
                changedObjects.Clear();
            }
        }

        private void SendToAllClients(Event e)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                SerializationHelper.Serialize(ms, e, this, e.GUIDSerialization);
                ms.Seek(0, SeekOrigin.Begin);
                foreach (Client c in Clients) 
                    c.SendPreSerialized(ms); 
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
                livingObjects[turnIndex].ComputeVisibility();
                SendOutUpdatedLevelIf();
            }
            else
            {
                changedObjects.AddRange(livingObjects[turnIndex].DoAllBehaviour());
                livingObjects[turnIndex].ComputeVisibility();
            }

            UpdateTurn();

            //SendOutLevelChanges();
        }

        public void ComputeVisibilities()
        {
            foreach (Living l in livingObjects)
                l.ComputeVisibility();
        }

        private void UpdateTurn()
        {
            if (livingObjects[turnIndex].ActionPoints <= 0)
            {
                turnIndex = (turnIndex + 1) % livingObjects.Count;
                livingObjects[turnIndex].ActionPoints = Living.MaxActionPoints;
                changedObjects.Add(livingObjects[turnIndex]);
            }
        }
        
        public void EndTurn(Player player)
        {
            if (livingObjects[turnIndex] == player)
            {
                player.ActionPoints = 0;
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
