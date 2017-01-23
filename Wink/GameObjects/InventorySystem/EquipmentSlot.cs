using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class EquipmentSlot : ItemSlot
    {
        private Type equipmentRestriction;

        public override Type TypeRestriction
        {
            get { return equipmentRestriction; }
        }

        /// <summary>
        /// ItemSlots specificly for items that can be equipped, items that do not match the equipment restriction won't be equipped.
        /// </summary>
        /// <param name="equipmentRestriction"> Specify what type of equipment this slot can hold</param>
        public EquipmentSlot(Type equipmentRestriction, string assetName = "empty:65:65:10:Green", int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 0, float scale = 1) : base(assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
            this.equipmentRestriction = equipmentRestriction;
        }

        #region Serialization
        public EquipmentSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            equipmentRestriction = Type.GetType(info.GetString("equipmentRestriction"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("equipmentRestriction", equipmentRestriction.FullName);
            base.GetObjectData(info, context);
        }
        #endregion
    }        
}
