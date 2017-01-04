using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class EmptyItem : Item
    {
        public EmptyItem(string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
        }

        public EmptyItem(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
