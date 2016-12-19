using System;
using Microsoft.Xna.Framework;

namespace Wink
{
    public class Player : Living
    {
        private Client client;
        protected int exp;
        public MouseSlot mouseSlot;

        private readonly GameObjectGrid itemGrid;
        public GameObjectGrid ItemGrid
        {
            get { return itemGrid; }
        }

        public Player(Client client, Level level, int layer) : base(level, layer, "player_" + client.ClientName)
        {
            this.client = client;

            //Inventory
            mouseSlot = new MouseSlot(layer + 11, "mouseSlot");
            itemGrid = new GameObjectGrid(3, 6);

            SetStats(5, 5, 5, 5, 55);

            level.Add(mouseSlot);
            level.Add(this);

            //Put player on start tile.
            Tile startTile = level.Find("startTile") as Tile;
            Position = startTile.Position - startTile.Origin + Origin;

            //InitAnimation(); not sure if overriden version gets played right without restating
        }

        protected override void InitAnimation(string idleColor = "empty:65:65:10:Magenta")
        {            
            base.InitAnimation(idleColor);
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
