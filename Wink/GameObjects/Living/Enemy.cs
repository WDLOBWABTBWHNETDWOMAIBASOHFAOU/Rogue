using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public class Enemy : Living
    {
        public Enemy(Level level, int layer) : base(level, layer, "Enemy")
        {
            level.Add(this);

            TileField grid = level.Find("TileField") as TileField;
            Tile ST = grid.Get(GameEnvironment.Random.Next(grid.Columns - 1), GameEnvironment.Random.Next(grid.Rows - 1)) as Tile;
            if (!ST.Passable) while (!ST.Passable)
            {
                ST = grid.Get(GameEnvironment.Random.Next(grid.Columns - 1), GameEnvironment.Random.Next(grid.Rows - 1)) as Tile;
            }
            float tileX = (ST.TilePosition.ToVector2().X + 1) * ST.Height - ST.Height / 2;
            float tileY = (ST.TilePosition.ToVector2().Y + 1) * ST.Width;
            Position = new Vector2(tileX, tileY);
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

            // Turn based testing AI
            if (this.isTurn)
            {
                this.position = position + new Vector2(Tile.TileHeight,0);
                PlayAnimation("die");
                ActionPoints = 0;
            }
        }
    }
}
