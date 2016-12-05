using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class PlayerMoveEvent : Event
    {
        public Player Player { get; set; }
        public Tile Tile { get; set; }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            Player.MoveTo(Tile);
            server.LevelChanged();
        }
    }
}
