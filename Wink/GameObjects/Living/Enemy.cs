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
        TileField grid;
        public Enemy(Level level, int layer) : base(level, layer, "Enemy")
        {
            level.Add(this);

            grid = level.Find("TileField") as TileField;
            // This is going to have to be replaced with FindAll but seeing as it's not in this branch and I'm kinda lazy,
            // I'm not doing that now. For now, this works.
            Tile ST = grid[GameEnvironment.Random.Next(grid.Columns - 1), GameEnvironment.Random.Next(grid.Rows - 1)] as Tile;
            while (!ST.Passable)
            {
                ST = grid[GameEnvironment.Random.Next(grid.Columns - 1), GameEnvironment.Random.Next(grid.Rows - 1)] as Tile;
            }
            float tileX = (ST.TilePosition.ToVector2().X + 1) * ST.Height - ST.Height / 2;
            float tileY = (ST.TilePosition.ToVector2().Y + 1) * ST.Width;
            Position = new Vector2(tileX, tileY);
        }

        protected override void InitAnimation(string idleColor = "empty:65:65:10:Magenta")
        {
            base.InitAnimation("empty:65:65:10:Purple");
            PlayAnimation("idle");
        }

        public void GoTo(Player player)
        {
            Tile tempTile = grid[0, 0] as Tile;
            Vector2 selfPos = new Vector2((Position.X + 0.5f*tempTile.Height) / tempTile.Height - 1, Position.Y / tempTile.Width - 1);
            Vector2 playPos = new Vector2((player.Position.X + 0.5f * tempTile.Height) / tempTile.Height - 1, player.Position.Y / tempTile.Width - 1);
            List<Tile> path = Pathfinding.ShortestPath(selfPos, playPos, grid);
            if (path.Count > 1)
            {
                MoveTo(path[0]);
            }
            else
            {
                Attack(player);
            }
        }
    }
}
