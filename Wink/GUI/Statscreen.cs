using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class Statscreen : Window
    {
        public Statscreen(Statsscreenbox statscreen, GameObjectList statsslots): base(statsslots.ItemGrid.Columns * Tile.TileHeight, ((2 + statsslots.ItemGrid.Rows)) * Tile.TileHeight)
        {
            statscreen.Position = new Vector2(0, 4 * statscreen.ItemGrid.CellHeight);

            statsslots.Find("strenghtSlot").Position = Vector2.Zero;
            statsslots.Find("dexteritySlot").Position = new Vector2(0, statsslots.Item.CellHeight);
            statsslots.Find("intelligenceSlots").Position = new Vector2(0, 3 * statsslots.ItemGrid.CellHeight);
            statsslots.Find("vitalitySLots").Position = new Vector2(0, 4 * statsslots.ItemGrid.CellHeight);
            statsslots.Find("wisdomSlots").Position = new Vector2(0, 5 * statsslots.ItemGrid.CellHeight);
            statsslots.Find("luckSlots").Position = new Vector2(0, 6 * statsslots.ItemGrid.CellHeight);

           


            Add(statsslots);
        }
    }
}
