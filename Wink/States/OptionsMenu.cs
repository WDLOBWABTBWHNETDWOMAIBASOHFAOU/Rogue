using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class OptionsMenu : GameObjectList
    {
        Button back;

        private class Resolution : SelectField<Resolution>.OptionAction
        {
            public int width;
            public int height;

            public Resolution(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

            public override string ToString()
            {
                return width + "x" + height;
            }

            public void execute()
            {
                Game1.WindowSize = new Point(width, height);
                Game1.ApplyResolutionSettings();
            }
        }

        public OptionsMenu()
        {
            Point screen = GameEnvironment.Screen;

            SpriteFont textFieldFont = GameEnvironment.AssetManager.GetFont("Arial26");
            SelectField<Resolution> resolutionSelect = new SelectField<Resolution>(true, textFieldFont, Color.Red);
            resolutionSelect.Options = new List<Resolution>() { new Resolution(1024, 586), new Resolution(1440, 825) };
            resolutionSelect.Position = new Vector2((screen.X - resolutionSelect.Width) / 2, 100);
            Add(resolutionSelect);
            
            //Create a button to go back to the main menu.
            back = new Button("button", "Back", textFieldFont, Color.Black);
            back.Action = () =>
            {
                GameEnvironment.GameStateManager.SwitchTo("mainMenuState");
            };
            back.Position = new Vector2(100, screen.Y - 100);
            Add(back);
        }
    }
}
