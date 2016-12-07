using Microsoft.Xna.Framework;

namespace Wink
{
    class PlayingGUI : GameObjectList
    {
        public PlayingGUI()
        {
            Layer = 1;

            int screenWidth = GameEnvironment.Screen.X;

            SpriteGameObject topBar = new SpriteGameObject("empty:" + screenWidth + ":100:25:Yellow", 0, "TopBar", 0, 0);
            Add(topBar);

            //Healthbar
            //Manabar
            //Action Points

            SpriteGameObject floor = new SpriteGameObject("empty:85:85:15:Orange", 1, "Floor", 0, 0);
            floor.Position = new Vector2((screenWidth - floor.Width)/2, 7.5f);
            Add(floor);
        }
    }
}
