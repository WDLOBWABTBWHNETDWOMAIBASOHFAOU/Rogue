using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class Bar : GameObjectList
    {
        class InnerBar : SpriteGameObject
        {
            private int value;
            private int maxValue;

            public InnerBar(ref int value, int maxValue, Color color, int layer = 0, string id = "", float scale = 1) : base("HUD/innerbar", layer, id, 0, 0, scale)
            {
                this.value = value;
                this.maxValue = maxValue;
            }

            public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
            {
                float w = (float)value / (float)maxValue;
                sprite.Draw(spriteBatch, origin, scale, DrawColor, new Rectangle(GlobalPosition.ToPoint(), new Point((int)(w * Width), (int)(8 * scale))));
            }
        }

        private SpriteGameObject outer;
        private SpriteGameObject inner;

        private SpriteFont font;
        private Color color;
        private int value;
        private int maxValue;

        public Bar(ref int value, int maxValue, SpriteFont font, Color color, int layer = 0, string id = "", float scale = 1) : base(layer, id)
        {
            this.font = font;
            this.color = color;
            this.value = value;
            this.maxValue = maxValue;

            inner = new InnerBar(ref value, maxValue, color, layer, id + "_inner", scale);
            inner.DrawColor = color;
            inner.Position = new Vector2(4.5f, 6f);
            Add(inner);
            outer = new SpriteGameObject("HUD/emptybar", layer, id + "_outer", 0, 0, scale);
            outer.DrawColor = color;
            Add(outer);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            outer.Draw(gameTime, spriteBatch, camera);
            inner.Draw(gameTime, spriteBatch, camera);

            Vector2 stringSize = font.MeasureString(value.ToString());
            float x = GlobalPosition.X + outer.Width + 10;
            float y = inner.GlobalPosition.Y - inner.Height / 2;

            spriteBatch.DrawString(font, value.ToString() + "/" + maxValue, new Vector2(x, y), color);
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
