using System;
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

            public InnerBar(int maxValue, int layer = 0, string id = "", float cameraSensitivity = 0, float scale = 1) : base("HUD/innerbar", layer, id, 0, 0, scale)
            {
                this.maxValue = maxValue;
                this.cameraSensitivity = cameraSensitivity;
            }

            public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
            {
                float w = Value / (float)maxValue;
                sprite.Draw(spriteBatch, origin, scale, DrawColor, new Rectangle(GlobalPosition.ToPoint() - (cameraSensitivity * camera.GlobalPosition).ToPoint(), new Point((int)(w * Width), (int)(8 * scale))));
            }
        }

        private SpriteGameObject outer;
        private InnerBar inner;

        private SpriteFont font;
        private Color color;
        private float scale;
        private bool stringVisible;

        public int Width {
            get { return (stringVisible ? (int)font.MeasureString(inner.Value.ToString()).X : 0) + outer.Width; }
        }

        float cameraSensitivity;
        public Bar(T o, Func<T, int> test, int maxValue, SpriteFont font, Color color, int layer = 0, string id = "", float cameraSensitivity = 0, float scale = 1, bool stringVisible = true) : base(layer, id)
        {
            this.cameraSensitivity = cameraSensitivity;
            this.font = font;
            this.color = color;
            this.scale = scale;
            this.stringVisible = stringVisible;

            outer = new SpriteGameObject("HUD/emptybar", layer, id + "_outer", 0, cameraSensitivity, scale);
            outer.DrawColor = color;
            Add(outer);

            inner = new InnerBar(maxValue, layer, id + "_inner", cameraSensitivity, scale);
            inner.DrawColor = color;
            float xdif = (outer.Sprite.Width - inner.Sprite.Width) / 2;
            float ydif = (outer.Sprite.Height - inner.Sprite.Height) / 2; 
            inner.Position = new Vector2(xdif, ydif)*scale;
            Add(inner);

            inner.maxValue = maxValue;
            inner.AddValue(o, test);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            outer.Draw(gameTime, spriteBatch, camera);
            inner.Draw(gameTime, spriteBatch, camera);

            Vector2 stringSize = font.MeasureString(inner.Value.ToString());
            float x = GlobalPosition.X + outer.Width + 10;
            float y = inner.GlobalPosition.Y - inner.Height / 2;

            if (stringVisible)
            {
                spriteBatch.DrawString(font, inner.Value.ToString() + "/" + inner.maxValue, new Vector2(x, y) - (cameraSensitivity * camera.GlobalPosition), color, 0, Vector2.Zero, scale / 2.5f, SpriteEffects.None, 0);
            }
        }
    }
}
