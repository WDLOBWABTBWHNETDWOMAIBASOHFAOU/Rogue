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
                    itemGrid.Add(new ItemSlot(), x, y);
                }
            }

            //TODO: test items REMOVE BEFORE RELEASE!!!
            ItemSlot testItem = itemGrid.Get(0, 0) as ItemSlot;
            testItem.ChangeItem( new TestItem());
            ItemSlot testItem2 = itemGrid.Get(0, 1) as ItemSlot;
            testItem2.ChangeItem(new TestItem());

            ItemSlot testItem3 = itemGrid.Get(1, 1) as ItemSlot;
            testItem3.ChangeItem(new WeaponEquipment("empty:65:65:10:Aqua","testWeapon",40,DamageType.magic,1,2,1,0,0,0.1,0.1,0.1));

            ItemSlot testItem4 = itemGrid.Get(2, 1) as ItemSlot;
            testItem4.ChangeItem(new BodyEquipment("empty:65:65:10:Brown","testArmor",40,40));

            ItemSlot testItem5 = itemGrid[2, 0] as ItemSlot;
            testItem5.ChangeItem(new RingEquipment(200, RingType.intelligence, "empty:65:65:10:Purple"));

            ItemSlot testItem6 = itemGrid[1, 0] as ItemSlot;
            testItem6.ChangeItem(new RingEquipment("empty:65:65:10:Green", 0, reflectEffect: true));

            ItemSlot testItem7 = itemGrid[3, 0] as ItemSlot;
            testItem7.ChangeItem(new Potion("empty:65:65:10:Blue", "healthPotion", PotionType.health, 25));
            // end test items

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
