using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public class Level : GameObjectList
    {
        public Level()
        {
            int rows= 10;
            int columns=10;
            TileField tf = new TileField(columns, rows, 0, "TileField");
            Point startTile = new Point(GameEnvironment.Random.Next(0, columns - 1), GameEnvironment.Random.Next(0, rows - 1));
            for (int x = 0; x < tf.Columns; x++)
            {
                for(int y = 0; y < tf.Rows; y++)
                {
                    if(startTile.X == x && startTile.Y == y)
                    {
                        tf.Add(new Tile("empty:65:65:10:Red", TileType.Normal, 0, "startTile"), x, y);
                    }
                    else
                    {
                        tf.Add(new Tile("empty:65:65:10:Magenta", TileType.Normal), x, y);
                    }                    
                }
            }
            Add(tf);


        }
    }
}
