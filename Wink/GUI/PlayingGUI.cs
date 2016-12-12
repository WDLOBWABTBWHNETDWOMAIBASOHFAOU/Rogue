using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class PlayingGUI : GameObjectList
    {
        private int testValue = 85;

        public PlayingGUI()
        {
            Layer = 1;

            int screenWidth = GameEnvironment.Screen.X;
            int barX = 150;
            SpriteFont defaultFont = GameEnvironment.AssetManager.GetFont("default");
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");

            SpriteGameObject topBar = new SpriteGameObject("HUD/topbar", 0, "TopBar", 0, 0);
            Add(topBar);
            
            Vector2 outerHPBarPosition = new Vector2(barX, 14);
            Vector2 outerMPBarPosition = new Vector2(barX, outerHPBarPosition.Y + 32);

            //Healthbar
            Bar hpBar = new Bar(ref testValue, 100, textfieldFont, Color.Red, 2, "HealthBar", 2.5f);
            hpBar.Position = new Vector2(outerHPBarPosition.X, outerHPBarPosition.Y);
            Add(hpBar);
            //Manabar
            Bar mpBar = new Bar(ref testValue, 100, textfieldFont, Color.Blue, 2, "ManaBar", 2.5f);
            mpBar.Position = new Vector2(outerMPBarPosition.X, outerMPBarPosition.Y);
            Add(mpBar);
            //Action Points

            SpriteGameObject floor = new SpriteGameObject("empty:85:85:15:Orange", 1, "Floor", 0, 0);
            floor.Position = new Vector2((screenWidth - floor.Width)/2, 7.5f);
            Add(floor);
        }
    }
}
