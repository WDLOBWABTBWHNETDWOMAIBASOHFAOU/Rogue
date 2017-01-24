using Microsoft.Xna.Framework;

namespace Wink
{
    class PlayerInventoryAndEquipment : Window
    {
        //I suggest using an inventory background sprite and using its height and width in the base
        public PlayerInventoryAndEquipment(InventoryBox inventory, GameObjectList equipmentslots) : base(576, 384)//base(inventory.ItemGrid.Columns * Tile.TileHeight, (2 + inventory.ItemGrid.Rows) * Tile.TileHeight)
        {
            //inventory.Position = new Vector2(0, 2 * inventory.ItemGrid.CellHeight);
            inventory.Position = new Vector2(64, 64);
            Add(inventory);

            // set position of individual equipment slots
            equipmentslots.Find("weaponSlot").Position = new Vector2(384, 128);
            equipmentslots.Find("bodySlot").Position = new Vector2(448, 128);
            equipmentslots.Find("ringSlot1").Position = new Vector2(384, 224);
            equipmentslots.Find("ringSlot2").Position = new Vector2(448, 224);
            equipmentslots.Find("headSlot").Position = new Vector2(416, 64);
            Add(equipmentslots);

            SpriteGameObject background = new SpriteGameObject("inventory/background", -1, cameraSensitivity: 0);

            SpriteGameObject boxes = new SpriteGameObject("inventory/boxes", cameraSensitivity: 0);
            Add(background);
            Add(boxes);
        }
    }
}