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
        protected string text;
        protected SpriteFont font;
        protected Color color;
        private Action action;

        public Action Action
        {
            get { return action; }
            set { action = value; }
        }

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
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (action != null)
            {
                inputHelper.IfMouseLeftButtonPressedOn(this, action); 
            }
        }
    }
}
