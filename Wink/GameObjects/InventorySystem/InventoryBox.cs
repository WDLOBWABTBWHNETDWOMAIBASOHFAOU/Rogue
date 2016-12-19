using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wink
{
    public class InventoryBox : GameObjectGrid
    {
        public GameObjectGrid itemGrid;
        public bool isVisible = false;
        public InventoryBox(int rows=3, int columns=6, int layer = 0, float cameraSensitivity = 0.0f, string id = "") : base(rows, columns, layer, id)
        {
            CellHeight = Tile.TileHeight;
            CellWidth = Tile.TileWidth;
            fillGrid();
            itemGrid = new GameObjectGrid(rows, columns, layer + 1, "itemGrid");
            itemGrid.Parent = this;
            itemGrid.CellHeight = CellHeight;
            itemGrid.CellWidth = CellWidth;
            
        }

        public void fillGrid()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Add(new Tile("empty:65:65:10:Gray", TileType.Inventory), x, y);                    
                }
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (inputHelper.KeyPressed(Keys.E))
            {
                isVisible = !isVisible;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (isVisible)
            {
                base.Update(gameTime);
                itemGrid.Update(gameTime);
            }
           
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (isVisible)
            {
                base.Draw(gameTime, spriteBatch, camera);
                itemGrid.Draw(gameTime, spriteBatch, camera);
            }
        }
    } 
}
