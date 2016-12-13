using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wink
{
    class PlayingGUI : GameObjectList
    {
        private PlayingMenu playingMenu;
        private int testValue = 10;

        public PlayingGUI()
        {
            Layer = 1;

            const int barX = 150;

            Point screen = GameEnvironment.Screen;
            SpriteFont defaultFont = GameEnvironment.AssetManager.GetFont("default");
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");

            SpriteGameObject topBar = new SpriteGameObject("HUD/topbar", 0, "TopBar", 0, 0);
            Add(topBar);

            playingMenu = new PlayingMenu();
            Rectangle pmBB = playingMenu.BoundingBox;
            playingMenu.Position = new Vector2((screen.X - pmBB.Width) / 2, (screen.Y - pmBB.Height) / 2);
            playingMenu.Visible = false;
            Add(playingMenu);

            Vector2 HPBarPosition = new Vector2(barX, 14);
            Vector2 MPBarPosition = new Vector2(barX, HPBarPosition.Y + 32);

            //Healthbar
            //Bar<Player> hpBar = new Bar<Player>(thisPlayer, player => player.health, thisPlayer.MaxHealth, textfieldFont, Color.Red, 2, "HealthBar", 2.5f);
            Bar<PlayingGUI> hpBar = new Bar<PlayingGUI>(this, obj => obj.testValue, 100, textfieldFont, Color.Red, 2, "HealthBar", 2.5f);
            hpBar.Position = new Vector2(HPBarPosition.X, HPBarPosition.Y);
            Add(hpBar);

            //Manabar
            Bar<PlayingGUI> mpBar = new Bar<PlayingGUI>(this, obj => obj.testValue, 100, textfieldFont, Color.Blue, 2, "ManaBar", 2.5f);
            mpBar.Position = new Vector2(MPBarPosition.X, MPBarPosition.Y);
            Add(mpBar);

            //Action Points

            SpriteGameObject floor = new SpriteGameObject("empty:85:85:15:Orange", 1, "Floor", 0, 0);
            floor.Position = new Vector2((screen.X - floor.Width)/2, 7.5f);
            Add(floor);
        }

        /// <summary>
        /// TEST CODE
        /// </summary>
        /// <param name="inputHelper"></param>
        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            testValue += inputHelper.ScrollWheelDelta / 100;

            if (inputHelper.KeyPressed(Keys.Escape))
            {
                playingMenu.Visible = !playingMenu.Visible;
            }
        }
    }
}
