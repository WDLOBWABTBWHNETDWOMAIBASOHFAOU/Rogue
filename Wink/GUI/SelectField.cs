using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class SelectField : TextField
    {
        private List<string> options;
        private int currentOptionIndex;
        
        public SelectField(SpriteFont sf, Color color, int layer = 0, string id = "", float scale = 1) : base(sf, color, layer, id, scale)
        {

        }

        public List<string> Options
        {
            get { return options; }
            set {
                options = value;
                base.Text = options[0];
            }
        }

        public sealed override bool Editable
        {
            get { return false; }
            set { }
        }

        public sealed override string Text
        {
            get { return base.Text; }
            protected set
            {
                if (options.Contains(value))
                {
                    base.Text = value;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            //Draw triangle arrows, to go to next value;
            base.Draw(gameTime, spriteBatch, camera);
        }

        private void SetOption(int index)
        {
            currentOptionIndex = index;
            base.Text = options[index];
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            
        }
    }
}
