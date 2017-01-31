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

        public LootSack(Enemy enemy) : base("Sprites/Containers/unseen_item", "Sprites/Containers/unseen_item", "Sounds/CLICK14A", enemy.FloorNumber, enemy.Inventory)
        {
            scale = 0.5f;
            for (int i = 0; i < enemy.EquipmentSlots.Children.Count-1; i++)
            {
                RestrictedItemSlot equipSlot = enemy.EquipmentSlots.Children[i] as RestrictedItemSlot;
                ItemSlot slot = enemy.Inventory[i, 0] as ItemSlot;
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
