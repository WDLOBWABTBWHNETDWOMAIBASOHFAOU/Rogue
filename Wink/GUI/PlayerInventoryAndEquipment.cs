using Microsoft.Xna.Framework;

namespace Wink
{
    public class PlayerInventoryAndEquipment : Window
    {
        //I suggest using an inventory background sprite and using its height and width in the base
        public PlayerInventoryAndEquipment(InventoryBox inventory, GameObjectList equipmentslots) : base(inventory.ItemGrid.Columns * Tile.TileHeight, (2 + inventory.ItemGrid.Rows) * Tile.TileHeight)
        {
            inventory.Position = new Vector2(0, 2 * inventory.ItemGrid.CellHeight);
            Add(inventory);

            // set position of individual equipment slots
            equipmentslots.Find("weaponSlot").Position = Vector2.Zero;
            equipmentslots.Find("bodySlot").Position = new Vector2(inventory.ItemGrid.CellWidth, 0);
            equipmentslots.Find("ringSlot1").Position = new Vector2(inventory.ItemGrid.CellWidth * 3, 0);
            equipmentslots.Find("ringSlot2").Position = new Vector2(inventory.ItemGrid.CellWidth * 4, 0);

            Add(equipmentslots);
        }
    }
}