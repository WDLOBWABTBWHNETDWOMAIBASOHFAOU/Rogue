using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Wink
{
    public class LocalServer : Server
    {
        private List<Client> clients;
        public Level level { get; }
        bool levelChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicServer">Wether or not the server can be joined by others.</param>
        public LocalServer(bool publicServer = false)
        {
            clients = new List<Client>();

            Level l = new Level();
            level = l;


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
            SendOutUpdatedLevel();
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
