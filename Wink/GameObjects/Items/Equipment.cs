using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class Equipment:Item
    {
        public Equipment(string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {

        }
    }
}
