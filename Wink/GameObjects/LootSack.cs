using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    [Serializable]
    class LootSack : Container
    {
        public override Point PointInTile
        {
            get { return new Point(16, 16); }
        }

        public override bool BlocksTile
        {
            get { return false; }
        }

        public LootSack(Enemy enemy) : base("empty:32:32:12:Yellow", enemy.FloorNumber, enemy.Inventory)
        {
            for (int i = 0; i < enemy.EquipmentSlots.Children.Count; i++)
            {
                EquipmentSlot equipSlot = enemy.EquipmentSlots.Children[i] as EquipmentSlot;
                ItemSlot slot = enemy.Inventory.ItemGrid[i, 0] as ItemSlot;
                slot.ChangeItem(equipSlot.SlotItem);
            }
        }

        #region Serialization
        public LootSack(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}
