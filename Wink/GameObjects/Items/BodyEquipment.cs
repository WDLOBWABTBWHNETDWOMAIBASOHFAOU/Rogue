using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class BodyEquipment : Equipment
    {
        private int armorValue;
        private int strRequirement;
        
        public int ArmorValue { get { return armorValue; } }
        public int StrRequirement { get { return strRequirement; } }

        public BodyEquipment(int armorValue, int strRequirement, string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
            this.armorValue = armorValue;
            this.strRequirement = strRequirement;
        }

        #region Serialization
        public BodyEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            armorValue = info.GetInt32("armorValue");
            strRequirement = info.GetInt32("strRequirement");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("armorValue", armorValue);
            info.AddValue("strRequirement", strRequirement);
        }
        #endregion
    }
}
