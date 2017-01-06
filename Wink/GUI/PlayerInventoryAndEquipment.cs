using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Wink
{
    class PlayerInventoryAndEquipment:Window
    {
        //I suggest using an inventory background sprite and using its height and width in the base
        public PlayerInventoryAndEquipment(GameObjectGrid itemGrid, GameObjectList equipmentslots) : base(itemGrid.Columns*Tile.TileHeight,(2+itemGrid.Rows) * Tile.TileHeight)
        {
            InventoryBox inventory = new InventoryBox(itemGrid);
            inventory.Position = new Vector2(0, 2 * itemGrid.CellHeight);
            Add(inventory);

            // set position of individual equipment slots
            Add(equipmentslots);
        }
    }
       
}
