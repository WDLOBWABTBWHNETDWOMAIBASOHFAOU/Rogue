using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class Button : SpriteGameObject
    {
        public bool Pressed { get; private set; }

        public Button(string assetName, int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 1, float scale = 1) : base(assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
        }

        public override void Reset()
        {
            base.Reset();
            Pressed = false;
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Pressed = inputHelper.MouseLeftButtonPressed() && BoundingBox.Contains((int)inputHelper.MousePosition.X, (int)inputHelper.MousePosition.Y);
        }
    }
}
