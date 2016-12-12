using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wink.GUI;

namespace Wink
{
    class PlayingGUI : GameObjectList
    {
        public PlayingGUI()
        {
            Layer = 1;

            int screenWidth = GameEnvironment.Screen.X;
            int barX = 150;
            SpriteFont defaultFont = GameEnvironment.AssetManager.GetFont("default");
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");

            SpriteGameObject topBar = new SpriteGameObject("HUD/topbar", 0, "TopBar", 0, 0);
            Add(topBar);
            SpriteGameObject outerHPBar = new SpriteGameObject("HUD/emptyhpbar", 0, "OuterHPBar", 0, 0, 2.5f);
            outerHPBar.Position = new Vector2(barX, 14);
            Add(outerHPBar);
            SpriteGameObject outerMPBar = new SpriteGameObject("HUD/emptympbar", 0, "OuterMPBar", 0, 0, 2.5f);
            outerMPBar.Position = new Vector2(barX, outerHPBar.Position.Y + 32);
            Add(outerMPBar);

            //Healthbar
            SpriteGameObject hpBar = new HealthBar("HUD/innerhpbar", "100/100", textfieldFont, Color.Red, 2, "HealthBar", 0, 2.5f);
            hpBar.Position = new Vector2(outerHPBar.Position.X + 4.5f, outerHPBar.Position.Y + 6);
            Add(hpBar);
            //Manabar
            SpriteGameObject mpBar = new ManaBar("HUD/innermpbar", "100/100", textfieldFont, Color.Blue, 2, "ManaBar", 0, 2.5f);
            mpBar.Position = new Vector2(outerMPBar.Position.X + 4.5f, outerMPBar.Position.Y + 5.5f);
            Add(mpBar);
            //Action Points

            SpriteGameObject floor = new SpriteGameObject("empty:85:85:15:Orange", 1, "Floor", 0, 0);
            floor.Position = new Vector2((screenWidth - floor.Width)/2, 7.5f);
            Add(floor);
        }
    }
}
