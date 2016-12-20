
using System;

namespace Wink
{
    [Serializable]
    class TestItem : Item
    {
        public TestItem(string assetName = "empty:65:65:10:Pink", int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
        }
    }
}
