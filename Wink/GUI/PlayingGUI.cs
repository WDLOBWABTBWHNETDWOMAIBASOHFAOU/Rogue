using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wink
{
    public class PlayingGUI : GameObjectList
    {
        private PlayingMenu playingMenu;
        private Window CharacterScreen;
        private Window Inventory
        
        

        {
            get { return Find(obj => obj is PlayerInventoryAndEquipment) as Window; }
        }

        public PlayingGUI()
        {
            Layer = 1;
            id = "PlayingGui";

            Point screen = GameEnvironment.Screen;
            SpriteFont defaultFont = GameEnvironment.AssetManager.GetFont("Arial12");

            SpriteGameObject topBar = new SpriteGameObject("HUD/topbar", 0, "TopBar", 0, 0);
            Add(topBar);

            playingMenu = new PlayingMenu();
            Rectangle pmBB = playingMenu.BoundingBox;
            playingMenu.Position = new Vector2((screen.X - pmBB.Width) / 2, (screen.Y - pmBB.Height) / 2);
            playingMenu.Visible = false;
            playingMenu.Layer = 100;
            Add(playingMenu);

            SpriteGameObject floor = new SpriteGameObject("empty:75:75:15:Orange", 1, "FloorBG", 0, 0);
            floor.Position = new Vector2((screen.X - floor.Width) / 2, 7.5f);
            Add(floor);

            TextGameObject floorNumber = new TextGameObject("Arial36", 0, 2, "FloorNumber");
            Add(floorNumber);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (inputHelper.KeyPressed(Keys.Escape))
            {
                playingMenu.Visible = !playingMenu.Visible;
            }

            if (inputHelper.KeyPressed(Keys.I))
            {
                Inventory.Visible = !Inventory.Visible;
            }

            if (inputHelper.KeyPressed(Keys.C))
            {
                CharacterScreen.Visible = !CharacterScreen.Visible;
            }
        }
    }
}
