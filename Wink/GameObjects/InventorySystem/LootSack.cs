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

            GameObjectList lootlist = enemy.Lootlist;
            for (int i = 0; i < lootlist.Children.Count - 1; i++)
            {
                Item item = lootlist.Children[i] as Item;
                ItemSlot slot = enemy.Inventory[0, i] as ItemSlot;
                slot.ChangeItem(item);
            }
        }

        #region Serialization
        public LootSack(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}
