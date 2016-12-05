using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class Level : GameObjectList
    {
        public Level()
        {
            TileField tf = new TileField(10, 10, 0, "TileField");

            Point startTile = new Point(0, 0);

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
