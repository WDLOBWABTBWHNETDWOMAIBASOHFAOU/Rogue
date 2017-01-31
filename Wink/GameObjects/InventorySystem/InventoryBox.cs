using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class InventoryBox : GameObjectGrid
    {
        public List<Item> Items
        {
            get
            {
                List<Item> items = new List<Item>();
                foreach (ItemSlot itemSlot in Objects)
                {
                    if (itemSlot.SlotItem != null)
                        items.Add(itemSlot.SlotItem);
                }
                return items;
            }
        }

        public InventoryBox(int rows, int columns, int layer = 0, string id = "", float cameraSensitivity = 0) : base(rows, columns, layer, id)
        {
            CellHeight = Tile.TileHeight;
            CellWidth = Tile.TileWidth;

            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Add(new ItemSlot("inventory/slot"), x, y);
                }
            }
        }

        #region Serialization
        public InventoryBox(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    } 
}
