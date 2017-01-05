using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class InventoryBox : GameObjectList
    {
        /// <summary>
        /// The grid that contains the actual items
        /// </summary>
        private GameObjectGrid itemGrid;

        public GameObjectGrid ItemGrid
        {
            get { return itemGrid; }
        }
        
        public InventoryBox(GameObjectGrid itemGrid, int layer = 0, string id = "", float cameraSensitivity = 0) : base(layer, id)
        {
            this.itemGrid = itemGrid;

            itemGrid.CellHeight = Tile.TileHeight;
            itemGrid.CellWidth = Tile.TileWidth;

            for (int x = 0; x < itemGrid.Columns; x++)
            {
                for (int y = 0; y < itemGrid.Rows; y++)
                {
                    itemGrid.Add(new ItemSlot(), x, y);
                }
            }

            // test items REMOVE BEFORE RELEASE!!!
            ItemSlot testItem = itemGrid.Get(0, 0) as ItemSlot;
            testItem.ChangeItem( new TestItem());
            ItemSlot testItem2 = itemGrid.Get(0, 1) as ItemSlot;
            testItem2.ChangeItem(new TestItem());

            ItemSlot testItem3 = itemGrid.Get(1, 1) as ItemSlot;
            testItem3.ChangeItem(new Equipment("empty:65:65:10:Blue"));
            // end test items

            Add(itemGrid);
            
        }
        
        public InventoryBox(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            itemGrid = info.GetValue("itemGrid", typeof(GameObjectGrid)) as GameObjectGrid;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("itemGrid", itemGrid);
        }   

        //private void CheckGrid(GameObjectGrid itemGrid)
        //{
        //    for (int x = 0; x < itemGrid.Columns; x++)
        //    {
        //        for (int y = 0; y < itemGrid.Rows; y++)
        //        {
        //            if ( itemGrid.Get(x,y) == null)
        //            {
        //                itemGrid.Add(new EmptyItem("empty:65:65:10:Gray"),x,y);
        //            }
        //        }
        //    }
        //}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            itemGrid.Update(gameTime);
        }
    } 
}
