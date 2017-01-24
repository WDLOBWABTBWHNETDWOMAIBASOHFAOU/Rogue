using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class InventoryBox : GameObjectList
    {
        public GameObjectGrid ItemGrid
        {
            get { return Find(obj => obj is GameObjectGrid) as GameObjectGrid; }
        }
        
        public InventoryBox(GameObjectGrid itemGrid, int layer = 0, string id = "", float cameraSensitivity = 0) : base(layer, id)
        {
            itemGrid.CellHeight = Tile.TileHeight;
            itemGrid.CellWidth = Tile.TileWidth;

            for (int x = 0; x < itemGrid.Columns; x++)
            {
                for (int y = 0; y < itemGrid.Rows; y++)
                {
                    itemGrid.Add(new ItemSlot("inventory/slot"), x, y);
                }
            }
            Add(itemGrid);
        }

        #region Serialization
        public InventoryBox(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion
        
    } 
}
