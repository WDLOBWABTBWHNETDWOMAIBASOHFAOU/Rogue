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
            tf.CellWidth = 65;
            tf.CellHeight = 65;

            for (int x = 0; x < tf.Columns; x++)
            {
                for(int y = 0; y < tf.Rows; y++)
                {
                    tf.Add(new Tile("empty65x65", TileType.Normal), x, y);
                }
            }
            Add(tf);


        }
    }
}
