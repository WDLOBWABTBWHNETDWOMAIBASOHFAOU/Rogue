using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class Container:SpriteGameObject,IGUIGameObject
    {
        private InventoryBox iBox;
        private Window iWindow;
        public Container(string asset, GameObjectGrid itemGrid = null, int layer=0, string id=""):base(asset,layer,id)
        {
            setInventory();
        }

        public void InitGUI()
        {
            if (iWindow == null)
            {
                iWindow = new Window(iBox.ItemGrid.Columns * Tile.TileWidth, iBox.ItemGrid.Rows * Tile.TileHeight);
                iWindow.Add(iBox);
                iWindow.Position = new Vector2(300, 300);
                iWindow.Visible = false;

                PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
                gui.Add(iWindow);

            }
        }

        public void setInventory(GameObjectGrid itemGrid = null)
        {
            if (itemGrid == null)
            {
                iBox = new InventoryBox(new GameObjectGrid(2, 4),layer+1,"",cameraSensitivity);
            }
            else
            {
                iBox = new InventoryBox(itemGrid,layer+1, "", cameraSensitivity);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            iBox.Update(gameTime);

            if (Visible)
            {
                //Not sure if it works with mul
                string ClientName = Environment.MachineName;
                Player player = GameWorld.Find("player_" + ClientName) as Player;

                int dx = (int)Math.Abs(player.Position.X - player.Origin.X - Position.X);
                int dy = (int)Math.Abs(player.Position.Y - player.Origin.Y - Position.Y);

                if (!(dx <= Tile.TileWidth && dy <= Tile.TileHeight))
                {
                    iWindow.Visible = false;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
            {
                string ClientName = Environment.MachineName;
                Player player = GameWorld.Find("player_" + Environment.MachineName) as Player;

                int dx = (int)Math.Abs(player.Position.X - player.Origin.X - Position.X);
                int dy = (int)Math.Abs(player.Position.Y - player.Origin.Y - Position.Y);

                if (dx <= Tile.TileWidth && dy <= Tile.TileHeight)
                {
                    iWindow.Visible = !iWindow.Visible;
                }

            };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            base.HandleInput(inputHelper);
        }
    }
}
