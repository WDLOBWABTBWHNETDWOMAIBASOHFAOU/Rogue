
using System.Runtime.Serialization;

namespace Wink
{
    class Equipment : Item
    {
        public Equipment(string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        { }

        public Equipment(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
