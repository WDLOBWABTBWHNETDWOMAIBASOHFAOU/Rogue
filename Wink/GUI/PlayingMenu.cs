using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class PlayingMenu : Window
    {
        Button back;

        public PlayingMenu() : base(350, 500, true, false)
        {
            SpriteFont sf = GameEnvironment.AssetManager.GetFont("Arial26");

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
