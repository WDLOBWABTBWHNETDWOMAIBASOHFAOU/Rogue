using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class HeadEquipment : Equipment
    {
        public HeadEquipment(int floorNumber = 0, int stackSize = 1, int layer = 0) : base(floorNumber, layer, stackSize)
        {
        }
    }
}
