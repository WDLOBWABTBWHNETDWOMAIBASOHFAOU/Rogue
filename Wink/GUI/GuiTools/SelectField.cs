using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Wink
{
    class SelectField<T> : TextField where T : SelectField<T>.OptionAction
    {
        public interface OptionAction
        {
            void execute();
        }

        private List<T> options;
        private int currentOptionIndex;
        private bool executeImmediatly;

        public SelectField(bool executeImmediatly, SpriteFont sf, Color color, int layer = 0, string id = "", float scale = 1) : base(sf, color, layer, id, scale)
        {
            this.executeImmediatly = executeImmediatly;
        }

        public List<T> Options
        {
            get { return options; }
            set {
                options = value;
                base.Text = options[0].ToString();
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
            protected set { }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            //TODO: Draw triangle arrows, to go to next value;
            base.Draw(gameTime, spriteBatch, camera);
        }

        private void SetOption(int index)
        {
            currentOptionIndex = index;
            base.Text = options[index].ToString();
            if (executeImmediatly)
                ExecuteCurrentOption();
        }

        public void NextOption()
        {
            if (currentOptionIndex < options.Count - 1)
                SetOption(++currentOptionIndex);
        }

        public void PreviousOption()
        {
            if (currentOptionIndex > 0)
                SetOption(--currentOptionIndex);
        }

        public void ExecuteCurrentOption()
        {
            options[currentOptionIndex].execute();
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            if (hasFocus)
            {
                Keys[] pressed = inputHelper.GetPressedKeys();
                foreach (Keys key in pressed)
                {
                    if (key == Keys.Left)
                        PreviousOption();
                    else if (key == Keys.Right)
                        NextOption();
                }
            }
        }
    }
}
