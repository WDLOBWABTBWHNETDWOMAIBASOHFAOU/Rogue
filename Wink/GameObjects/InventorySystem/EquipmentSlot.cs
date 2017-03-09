using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    public class EquipmentSlot : RestrictedItemSlot
    {
        private Living living;
        public EquipmentSlot(Type equipmentRestriction, Living partof, string assetName = "empty:65:65:10:Green", int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 0, float scale = 1) : base(equipmentRestriction, assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
            living = partof;
        }
        #region Serialization
        public EquipmentSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            living = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("livingGUID"))) as Living;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("livingGUID", living.GUID.ToString());
        }
        #endregion

        public override void ChangeItem(Item newItem)
        {
            if(SlotItem!=null)
                SlotItem.RemoveBonus(living);
            if(newItem!=null)
                newItem.DoBonus(living);
            base.ChangeItem(newItem);
        }
    }
}
