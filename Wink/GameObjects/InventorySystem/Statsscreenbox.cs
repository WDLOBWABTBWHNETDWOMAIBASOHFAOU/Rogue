using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    public class Statsscreenbox : GameObjectList
    {
        public GameObjectGrid ItemGrid2
        {
            get { return Find(obj => obj is GameObjectGrid) as GameObjectGrid; }
        }

        public Statsscreenbox(GameObjectGrid itemgrid2, int layer = 0, string id="", float cameraSensitivity = 0) : base(layer, id)
        {
            itemgrid2.CellHeight = Tile.TileHeight;
            itemgrid2.CellWidth = Tile.TileWidth;


            for (int x = 0; x <   itemgrid2.Columns; x++)
            {
                for (int y = 0; y < itemgrid2.Rows ; y++)
                {
                    ItemGrid2.Add(new ItemSlot(), x, y);
                }
            }
        }
    }


}
