using System;
using Microsoft.Xna.Framework;

namespace Wink
{
    public class Player : Living
    {
        private Client client;
        protected int exp;
        public MouseSlot mouseSlot;
        
        public Player(Client client, Level level,int layer) : base(level, layer, "player_" + client.ClientName)
        {
            this.client = client;

            mouseSlot = new MouseSlot(layer + 11, "mouseSlot");
            level.Add(mouseSlot);

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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            mouseSlot.Update(gameTime);

            if (exp >= RequiredExperience())
            {
                LevelUp();
            }
        }

        /// <summary>
        /// returns the experience a creature requires for its next level.
        /// </summary>
        /// <returns></returns>
        protected int RequiredExperience()
        {
            double mod = 36.79;
            double pow = Math.Pow(Math.E, Math.Sqrt(creatureLevel));
            int reqExp = (int)(mod * pow);
            return reqExp;
        }

        protected void LevelUp()
        {
            exp -= RequiredExperience();
            creatureLevel++;

            // + some amount of neutral stat points, distriputed by user discresion or increase stats based on picked hero
        } 
    }
}
