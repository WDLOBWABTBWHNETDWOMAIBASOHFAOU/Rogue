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
            SpriteFont textFieldFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");
            SelectField<Resolution> resolutionSelect = new SelectField<Resolution>(true, textFieldFont, Color.Red);
            resolutionSelect.Options = new List<Resolution>() { new Resolution(1024, 586), new Resolution(1440, 825) };
            Add(resolutionSelect);

        }
    }
}
