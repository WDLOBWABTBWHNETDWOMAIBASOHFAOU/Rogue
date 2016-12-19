using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wink
{
    class PlayingGUI : GameObjectList
    {
        private PlayingMenu playingMenu;
        private Window inventory;

        public PlayingGUI()
        {
            Layer = 1;

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

            SpriteGameObject floor = new SpriteGameObject("empty:85:85:15:Orange", 1, "Floor", 0, 0);
            floor.Position = new Vector2((screen.X - floor.Width)/2, 7.5f);
            Add(floor);
        }

        public void AddPlayerGUI(Player player)
        {
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");

            const int barX = 150;
            Vector2 HPBarPosition = new Vector2(barX, 14);
            Vector2 MPBarPosition = new Vector2(barX, HPBarPosition.Y + 32);

            //Healthbar
            Bar<Player> hpBar = new Bar<Player>(player, p => p.health, player.MaxHealth, textfieldFont, Color.Red, 2, "HealthBar", 2.5f);
            hpBar.Position = new Vector2(HPBarPosition.X, HPBarPosition.Y);
            Add(hpBar);

            //Manabar
            Bar<Player> mpBar = new Bar<Player>(player, p => p.mana, player.MaxMana, textfieldFont, Color.Blue, 2, "ManaBar", 2.5f);
            mpBar.Position = new Vector2(MPBarPosition.X, MPBarPosition.Y);
            Add(mpBar);

            //Action Points
            Bar<Player> apBar = new Bar<Player>(player, p => p.ActionPoints, Living.MaxActionPoints, textfieldFont, Color.Yellow, 2, "ActionBar", 2.5f);
            int screenWidth = GameEnvironment.Screen.X;
            Vector2 APBarPosition = new Vector2(screenWidth - barX - apBar.Width, HPBarPosition.Y);
            apBar.Position = new Vector2(APBarPosition.X, APBarPosition.Y);
            Add(apBar);

            GameObjectGrid items = player.ItemGrid;
            inventory = new Window(items.Columns * Tile.TileWidth, items.Rows * Tile.TileHeight);
            inventory.Add(new InventoryBox(player.ItemGrid));
            inventory.Position = new Vector2(300, 300);
            inventory.Visible = false;
            Add(inventory);

            Add(player.mouseSlot);
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
                inventory.Visible = !inventory.Visible;
            }
        }
    }
}
