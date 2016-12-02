using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class LocalServer : Server
    {
        private List<Client> clients;
        private Level level;
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

            }
        }
        
    }
}
