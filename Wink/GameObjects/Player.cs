using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class Player : Living
    {
        private Client client;

        public Player(Client client, Level level) : base(0, "player_" + client.clientName)
        {
            this.client = client;

            LoadAnimation("empty:65:65:10:DarkGreen", "default", true);
            level.Add(this);
            MoveTo(level.Find("startTile") as Tile);
            PlayAnimation("default");
        }

    }
}
