using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class ChestArmor : ArmorEquipment
    {
        public ChestArmor(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        
        public ChestArmor(string assetName, string id, ArmorType armorType, int physicalValue = 0, int magicValue = 0, int reqPenalty = 0, int layer = 0, int stackSize = 1) : base(assetName, id, armorType, physicalValue, magicValue, reqPenalty, layer, stackSize)
        {
        }

        protected override bool MeetsRequirements(Living l)
        {
            throw new NotImplementedException();
        }
    }
}
