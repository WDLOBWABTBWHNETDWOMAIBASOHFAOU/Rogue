using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    [Serializable]
    class TestItem : Item
    {
        public TestItem(string assetName = "empty:64:64:10:Pink", int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
            stackCount = 1;
        }

        public TestItem(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
