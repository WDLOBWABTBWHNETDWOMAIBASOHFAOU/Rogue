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
        public BodyEquipment(int armorValue, string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
            this.armorValue = armorValue;
        }
    }
}
