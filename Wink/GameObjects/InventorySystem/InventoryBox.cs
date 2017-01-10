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
            testItem3.ChangeItem(new WeaponEquipment(80,1.3,4,2,"empty:65:65:10:Aqua"));

            ItemSlot testItem4 = itemGrid.Get(2, 1) as ItemSlot;
            testItem4.ChangeItem(new BodyEquipment(20, 2, "empty:65:65:10:Brown"));

            ItemSlot testItem5 = itemGrid[2, 0] as ItemSlot;
            testItem5.ChangeItem(new RingEquipment(500000, RingType.dexterity, "empty:65:65:10:DarkGray"));

            ItemSlot testItem6 = itemGrid[1, 0] as ItemSlot;
            testItem6.ChangeItem(new RingEquipment(200, RingType.intelligence, "empty:65:65:10:DarkGray", true));
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            itemGrid.Update(gameTime);
        }
    } 
}
