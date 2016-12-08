using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Wink
{
    public class Player : Living
    {
        private Client client;
        
        public Player(Client client, Level level) : base(level, 0, "player_" + client.clientName)
        {
            this.client = client;
            
            level.Add(this);
            Tile ST = level.Find("startTile") as Tile;
            float tileX = (ST.TilePosition.ToVector2().X+1) * ST.Height- ST.Height/2;
            float tileY = (ST.TilePosition.ToVector2().Y +1)* ST.Width;
            Position = new Vector2(tileX,tileY);
            //InitAnimation(); not sure if overriden version gets played right without restating
        }

        protected override void InitAnimation()
        {
            
            base.InitAnimation();
            PlayAnimation("idle");
        }

    }
}
