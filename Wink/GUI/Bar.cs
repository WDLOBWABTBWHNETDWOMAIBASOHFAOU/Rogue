using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class Bar<T> : GameObjectList where T : class
    {
        private class InnerBar : SpriteGameObject 
        {
            private Tuple<T, Func<T, int>> value;
            public int maxValue;

            public int Value
            {
                get { return value.Item2.Invoke(value.Item1); }
            }

            public void AddValue(T o, Func<T, int> test) 
            {
                value = new Tuple<T, Func<T, int>>(o , test);
            }

            public InnerBar(int maxValue, int layer = 0, string id = "", float scale = 1) : base("HUD/innerbar", layer, id, 0, 0, scale)
            {
                this.maxValue = maxValue;
            }

            public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
            {
                float w = Value / (float)maxValue;
                sprite.Draw(spriteBatch, origin, scale, DrawColor, new Rectangle(GlobalPosition.ToPoint(), new Point((int)(w * Width), (int)(8 * scale))));
            }
        }

        private SpriteGameObject outer;
        private InnerBar inner;

        private SpriteFont font;
        private Color color;
        private float scale;

        public int Width {
            get { return (int)font.MeasureString(inner.Value.ToString()).X + outer.Width; }
        }
        
        public Bar(T o, Func<T, int> test, int maxValue, SpriteFont font, Color color, int layer = 0, string id = "", float scale = 1) : base(layer, id)
        {
            this.font = font;
            this.color = color;
            this.scale = scale;

            inner = new InnerBar(maxValue, layer, id + "_inner", scale);
            inner.DrawColor = color;
            inner.Position = new Vector2(4.5f, 6f);
            Add(inner);

            this.inner.maxValue = maxValue;

            inner.AddValue(o, test);

            outer = new SpriteGameObject("HUD/emptybar", layer, id + "_outer", 0, 0, scale);
            outer.DrawColor = color;
            Add(outer);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            outer.Draw(gameTime, spriteBatch, camera);
            inner.Draw(gameTime, spriteBatch, camera);

            Vector2 stringSize = font.MeasureString(inner.Value.ToString());
            float x = GlobalPosition.X + outer.Width + 10;
            float y = inner.GlobalPosition.Y - inner.Height / 2;

            spriteBatch.DrawString(font, inner.Value.ToString() + "/" + inner.maxValue, new Vector2(x, y), color);
        }
    }
}
