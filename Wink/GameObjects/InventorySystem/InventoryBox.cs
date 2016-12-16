using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class InventoryBox : GameObjectGrid
    {
        /// <summary>
        /// The grid that contains the actual items
        /// </summary>
        private GameObjectGrid itemGrid;

        public GameObjectGrid ItemGrid {
            get { return itemGrid; }
        }

        public InventoryBox(int rows=3, int columns=6, int layer = 0, string id = "") : base(rows, columns, layer, id)
        {
            CellHeight = Tile.TileHeight;
            CellWidth = Tile.TileWidth;
            FillGrid();

            itemGrid = new GameObjectGrid(rows, columns, layer + 1, "itemGrid");
            itemGrid.Parent = this;
            itemGrid.CellHeight = CellHeight;
            itemGrid.CellWidth = CellWidth;
        }

        private void FillGrid()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Add(new Tile("empty:65:65:10:Gray", TileType.Inventory), x, y);                    
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            itemGrid.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            itemGrid.Draw(gameTime, spriteBatch, camera);
        }
    } 
}
