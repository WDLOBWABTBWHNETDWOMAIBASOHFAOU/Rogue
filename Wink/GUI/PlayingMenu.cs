using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class PlayingMenu : GameObjectList
    {
        Button back;

        public PlayingMenu()
        {
            SpriteFont sf = GameEnvironment.AssetManager.GetFont("TextFieldFont");

            SpriteGameObject bg = new SpriteGameObject("empty:350:500:75:Black", 0, "", 0, 0, 1);
            Add(bg);

            back = new Button("button", "Back to Main Menu", sf, Color.Black);
            back.Position = new Vector2(25, 25);
            Add(back);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            if (back.Pressed)
            {
                GameEnvironment.GameStateManager.SwitchTo("mainMenuState");
            }
        }

    }
}
