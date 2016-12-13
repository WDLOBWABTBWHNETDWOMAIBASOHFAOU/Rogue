using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public class Player : Living
    {
        private Client client;

        public Player()
        {
            InitAnimation();
        }

        public Player(Client client, Level level) : base(0, "player_" + client.ClientName)
        {
            this.client = client;
            
            level.Add(this);
            MoveTo(level.Find("startTile") as Tile);

            InitAnimation();
        }

        private void InitAnimation()
        {
            LoadAnimation("empty:65:65:10:DarkGreen", "default", true);
            PlayAnimation("default");
        }

    }
}
