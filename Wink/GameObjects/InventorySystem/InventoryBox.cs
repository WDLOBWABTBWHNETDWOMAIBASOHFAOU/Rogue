using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        public InventoryBox(GameObjectGrid itemGrid, int layer = 0, string id = "") : base(itemGrid.Rows, itemGrid.Columns, layer, id)
        {
            CellHeight = Tile.TileHeight;
            CellWidth = Tile.TileWidth;
            FillGrid();
            
            itemGrid.Parent = this;
            itemGrid.CellHeight = CellHeight;
            itemGrid.CellWidth = CellWidth;
            this.itemGrid = itemGrid;
        }

        public InventoryBox(int rows=3, int columns=6, int layer = 0, string id = "") : this(new GameObjectGrid(rows, columns, layer + 1, "itemGrid"), layer, id)
        {
        }

        private void FillGrid()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Add(new Tile("empty:65:65:10:Gray", TileType.Inventory, 0, "", 0), x, y);
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
