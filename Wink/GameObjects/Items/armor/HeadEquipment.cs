using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class HeadEquipment : ArmorEquipment
    {
        public HeadEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        
        public HeadEquipment(ArmorType armorType, int physicalValue = 0, int magicValue = 0, int reqPenalty = 0, int layer = 0, int stackSize = 1) : base(armorType, physicalValue, magicValue, reqPenalty, layer, stackSize)
        {
        }

        protected override bool MeetsRequirements(Living l)
        {
            return true; //TODO: determine and set requirements
        }

        public override void ItemInfo(ItemSlot caller)
        {
            displayedName = armorType.ToString() + " Helmet";
            base.ItemInfo(caller);
        }
    }
}
