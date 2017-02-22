using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class EquipmentSlot : RestrictedItemSlot
    {
        private Living living;
        public EquipmentSlot(Type equipmentRestriction,Living living, string assetName = "empty:65:65:10:Green", int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 0, float scale = 1) : base(equipmentRestriction, assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
            this.living = living;
        }

        public override void ChangeItem(Item newItem)
        {
            SlotItem.RemoveBonus(living);
            newItem.DoBonus(living);
            base.ChangeItem(newItem);
        }
    }
}
