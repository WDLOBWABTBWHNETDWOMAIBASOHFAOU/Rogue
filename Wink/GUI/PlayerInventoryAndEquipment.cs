using Microsoft.Xna.Framework;

namespace Wink
{
    class PlayerInventoryAndEquipment : Window
    {
        //I suggest using an inventory background sprite and using its height and width in the base
        public PlayerInventoryAndEquipment(InventoryBox inventory, GameObjectList equipmentslots) : base(576, 384)//base(inventory.ItemGrid.Columns * Tile.TileHeight, (2 + inventory.ItemGrid.Rows) * Tile.TileHeight)
        {
            //inventory.Position = new Vector2(0, 2 * inventory.ItemGrid.CellHeight);
            inventory.Position = new Vector2(inventory.CellHeight, inventory.CellWidth);
            Add(inventory);
            Add(equipmentslots);
            SetEquipmentPositions();

            SpriteGameObject background = new SpriteGameObject("inventory/background", -1, cameraSensitivity: 0);
            SpriteGameObject boxes = new SpriteGameObject("inventory/boxes", cameraSensitivity: 0);
            Add(background);
            Add(boxes);
            //this is not ideal but works for now
        }

        public void SetEquipmentPositions()
        {
            // set position of individual equipment slots
            Find("weaponSlot").Position = new Vector2(384, 128);
            Find("bodySlot").Position = new Vector2(448, 128);
            Find("ringSlot1").Position = new Vector2(384, 224);
            Find("ringSlot2").Position = new Vector2(448, 224);
            Find("headSlot").Position = new Vector2(416, 64);
        }

        public override void Replace(GameObject replacement)
        {
            base.Replace(replacement);
            if (replacement is RestrictedItemSlot)
                SetEquipmentPositions();
        }
    }
}