using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class BodyEquipment : Equipment
    {
        int armorValue;
        public int ArmorValue { get { return armorValue; } }
        int strRequirement;
        public int StrRequirement { get { return strRequirement; } }
        public BodyEquipment(int armorValue, int strRequirement, string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
            this.armorValue = armorValue;
            this.strRequirement = strRequirement;
        }
    }
}
