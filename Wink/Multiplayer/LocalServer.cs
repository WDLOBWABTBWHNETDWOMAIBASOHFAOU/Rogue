using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Wink
{
    public class LocalServer : Server
    {
        private List<Client> clients;
        public Level level { get; }
        private List<Living> livingObjects;
        bool levelChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicServer">Wether or not the server can be joined by others.</param>
        public LocalServer(bool publicServer = false)
        {
            clients = new List<Client>();

            Level l = new Level(1);
            level = l;

            livingObjects = level.FindAll(obj => obj is Living).Cast<Living>().ToList();
            livingObjects.Sort((obj1, obj2)=> obj1.Dexterity - obj2.Dexterity);
            livingObjects.ElementAt(0).isTurn = true;

            if (publicServer)
            {
                MakePublic();
            }
        }

        public override void Send(Event e)
        {
            e.OnServerReceive(this);
        }

        public override void AddLocalClient(LocalClient localClient)
        {
            clients.Add(localClient);
            ClientAdded(localClient);
        }

        private void ClientAdded(Client client)
        {
            Player player = new Player(client, level,level.Layer+1);

            addPlayer(player);
        
            SendOutUpdatedLevel();
        }

        void addPlayer(Living player)
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
                e.updatedLevel = level;
                c.Send(e);
            }
        }

        private void MakePublic()
        {
            //setup TCP with masterserver, 
            
        }

        public void Update(GameTime gameTime)
        {
            level.Update(gameTime);

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
    }
}
