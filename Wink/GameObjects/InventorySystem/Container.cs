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
        private Window inventory;
        public Container(string asset, GameObjectGrid itemGrid = null, int layer=0, string id=""):base(asset,layer,id)
        {
            setInventory();
        }

        public void InitGUI()
        {
            Window inventory = new Window(iBox.ItemGrid.Columns * Tile.TileWidth, iBox.ItemGrid.Rows * Tile.TileHeight);
            inventory.Add(iBox);
            inventory.Position = new Vector2(300, 300);
            inventory.Visible = false;
            this.inventory = inventory;

            PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
            gui.Add(inventory);
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
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
            {
                inventory.Visible = !inventory.Visible;

            };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            base.HandleInput(inputHelper);
        }
    }
}
