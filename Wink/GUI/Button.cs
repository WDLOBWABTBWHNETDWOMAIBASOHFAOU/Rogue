using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class Button : SpriteGameObject
    {
        public bool Pressed { get; private set; }
        private string text;
        private SpriteFont font;
        private Color color;

        public Button(string assetName, string text, SpriteFont font, Color color, int layer = 0, string id = "", int sheetIndex = 0, float scale = 1) : base(assetName, layer, id, sheetIndex, 0, scale)
        {
            this.text = text;
            this.font = font;
            this.color = color;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);

            Vector2 stringSize = font.MeasureString(text);
            float x = (GlobalPosition.X + Width / 2) - stringSize.X / 2;
            float y = (GlobalPosition.Y + Height / 2) - stringSize.Y / 2;

            spriteBatch.DrawString(font, text, new Vector2(x, y), color);
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
