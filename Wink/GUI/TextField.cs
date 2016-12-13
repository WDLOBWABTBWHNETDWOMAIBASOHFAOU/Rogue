using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wink
{
    class TextField : SpriteGameObject
    {
        protected bool hasFocus;

        private string content;
        private int cursorPosition;
        private SpriteFont spriteFont;
        private Color color;

        private const string assetName = "empty:300:50:25:Aquamarine";
        private static readonly Point offset = new Point(3, 3);

        public virtual string Text
        {
            get { return content; }
            protected set
            {
                Vector2 valueSize = spriteFont.MeasureString(value) + offset.ToVector2() * 2;
                if (valueSize.X <= BoundingBox.Width && valueSize.Y <= BoundingBox.Height)
                {
                    content = value;
                }
            }
        }

        public virtual bool Editable { get; set; }

        public TextField(SpriteFont spriteFont, Color color, int layer = 0, string id = "", float scale = 1) : base(assetName, layer, id, 0, 0, scale)
        {
            this.content = "";
            this.spriteFont = spriteFont;
            this.color = color;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            //Draw the contents of this textfield.
            spriteBatch.DrawString(spriteFont, content, GlobalPosition + offset.ToVector2(), color);

            //Draw cursor but only ever other hald second.
            if (gameTime.TotalGameTime.Milliseconds % 1000 < 500 && hasFocus && Editable)
            {
                Texture2D tex = GameEnvironment.AssetManager.GetSingleColorPixel(color);
                float cursorX = spriteFont.MeasureString(content.Substring(0, cursorPosition)).X;
                Vector2 cursorPos = GlobalPosition + new Vector2(offset.X + cursorX, offset.Y);
                spriteBatch.Draw(tex, null, new Rectangle(cursorPos.ToPoint(), new Point(2, Height - offset.Y * 2)));
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (inputHelper.MouseLeftButtonPressed())
            {
                Vector2 mouse = inputHelper.MousePosition;
                //If the mouseclick was inside the textfield, we need to move the cursor.
                if (BoundingBox.Contains(mouse))
                {
                    hasFocus = true;

                    if (!Editable)
                    {
                        return;
                    }

                    int pos = 0;
                    float relMouseX = mouse.X - position.X;
                    float distance = relMouseX;

                    //Iterate over possible cursorPositions
                    for (int i = 1; i <= content.Length; i++)
                    {
                        //Calculate the distance from the current cursorposition to the mouseclick
                        string sub = content.Substring(0, i);
                        Vector2 subSize = spriteFont.MeasureString(sub);
                        float newDistance = Math.Abs(relMouseX - subSize.X);

                        //See if the distance is smaller than it was
                        if(newDistance < distance)
                        {
                            //If it was move on to the next one.
                            distance = newDistance;
                            pos = i;
                        }
                        else
                        {
                            //If it wasn't the previous position is where the cursor should be.
                            pos = i-1;
                            break;
                        }
                    }

                    cursorPosition = pos;
                }
                else
                {
                    hasFocus = false;
                }
            }
            HandleKeyInput(inputHelper);
        }

        private void HandleKeyInput(InputHelper inputHelper)
        {
            if (hasFocus)
            {
                Keys[] pressed = inputHelper.GetPressedKeys();

                bool shift = inputHelper.IsKeyDown(Keys.LeftShift) || inputHelper.IsKeyDown(Keys.RightShift);

                foreach (Keys key in pressed)
                {
                    int keyValue = (int)key;
                    if (keyValue >= 65 && keyValue <= 90) //a-z keys
                    {
                        int shiftValue = shift ? 0 : 32;
                        InsertCharachter((char)(keyValue + shiftValue));
                    }
                    else if (shift && shiftTable.ContainsKey(key))//SHIFT + other key
                    {
                        InsertCharachter(shiftTable[key]);
                    }
                    else if (keyTable.ContainsKey(key))//other key
                    {
                        InsertCharachter(keyTable[key]);
                    }
                    else if (key == Keys.Left && cursorPosition > 0)
                        cursorPosition--;
                    else if (key == Keys.Right && cursorPosition < content.Length)
                        cursorPosition++;
                    else if (key == Keys.Back && cursorPosition > 0) //BACKSPACE
                    {
                        content = content.Remove(cursorPosition - 1, 1);
                        cursorPosition--;
                    }
                    else if (key == Keys.Delete && cursorPosition < content.Length)
                        content = content.Remove(cursorPosition, 1);
                }
            }
        }

        private void InsertCharachter(char c)
        {
            string newContent = content.Insert(cursorPosition, new string(new char[] { c }));
            Text = newContent;
            if(Text == newContent)
                cursorPosition++;
        }

        private static readonly Dictionary<Keys, char>keyTable = new Dictionary<Keys, char>()
        {
            { Keys.OemTilde, '`'},
            { Keys.D1, '1'},
            { Keys.D2, '2'},
            { Keys.D3, '3'},
            { Keys.D4, '4'},
            { Keys.D5, '5'},
            { Keys.D6, '6'},
            { Keys.D7, '7'},
            { Keys.D8, '8'},
            { Keys.D9, '9'},
            { Keys.D0, '0'},
            { Keys.NumPad1, '1'},
            { Keys.NumPad2, '2'},
            { Keys.NumPad3, '3'},
            { Keys.NumPad4, '4'},
            { Keys.NumPad5, '5'},
            { Keys.NumPad6, '6'},
            { Keys.NumPad7, '7'},
            { Keys.NumPad8, '8'},
            { Keys.NumPad9, '9'},
            { Keys.NumPad0, '0'},
            { Keys.OemMinus, '-'},
            { Keys.OemPlus, '='},
            { Keys.OemOpenBrackets, '['},
            { Keys.OemCloseBrackets, ']'},
            { Keys.OemBackslash, '\\'},
            { Keys.OemSemicolon, ';'},
            { Keys.OemQuotes, '\''},
            { Keys.OemComma, ','},
            { Keys.OemPeriod, '.'},
            { Keys.Decimal, '.'},
            { Keys.OemQuestion, '/'},
            { Keys.Space, ' '},
        };

        private static readonly Dictionary<Keys, char> shiftTable = new Dictionary<Keys, char>()
        {
            { Keys.OemTilde, '~'},
            { Keys.D1, '!'},
            { Keys.D2, '@'},
            { Keys.D3, '#'},
            { Keys.D4, '$'},
            { Keys.D5, '%'},
            { Keys.D6, '^'},
            { Keys.D7, '&'},
            { Keys.D8, '*'},
            { Keys.D9, '('},
            { Keys.D0, ')'},
            { Keys.OemMinus, '_'},
            { Keys.OemPlus, '+'},
            { Keys.OemOpenBrackets, '{'},
            { Keys.OemCloseBrackets, '}'},
            { Keys.OemBackslash, '|'},
            { Keys.OemSemicolon, ':'},
            { Keys.OemQuotes, '"'},
            { Keys.OemComma, '<'},
            { Keys.OemPeriod, '>'},
            { Keys.OemQuestion, '?'},
        };
    }
}
