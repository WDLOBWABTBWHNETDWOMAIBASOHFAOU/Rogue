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
            // This is going to have to be replaced with FindAll but seeing as it's not in this branch and I'm kinda lazy,
            // I'm not doing that now. For now, this works.
            Tile ST = grid[GameEnvironment.Random.Next(grid.Columns - 1), GameEnvironment.Random.Next(grid.Rows - 1)] as Tile;
            while (!ST.Passable)
            {
                ST = grid.Get(GameEnvironment.Random.Next(grid.Columns - 1), GameEnvironment.Random.Next(grid.Rows - 1)) as Tile;
            }
            float tileX = (ST.TilePosition.ToVector2().X + 1) * ST.Height - ST.Height / 2;
            float tileY = (ST.TilePosition.ToVector2().Y + 1) * ST.Width;
            Position = new Vector2(tileX, tileY);
        }

        protected override void InitAnimation()
        {
            base.InitAnimation();
            PlayAnimation("idle");
        }
    }
}
