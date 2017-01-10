
using System;
using Microsoft.Xna.Framework;

namespace Wink
{
    [Serializable]
    class TestItem : Item
    {
        public TestItem(string assetName = "empty:65:65:10:Pink", int stackSize = 3, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
            stackCount = 1;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
