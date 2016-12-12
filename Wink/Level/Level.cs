using Microsoft.Xna.Framework;

namespace Wink
{
    public class Level : GameObjectList
    {
        public Level()
        {
            int rows= 10;
            int columns=10;
            TileField tf = new TileField(columns, rows, 0, "TileField");
            Point startTile = new Point(GameEnvironment.Random.Next(0, columns - 1), GameEnvironment.Random.Next(0, rows - 1));
            for (int x = 0; x < tf.Columns; x++)
            {
                for(int y = 0; y < tf.Rows; y++)
                {
                    if(startTile.X == x && startTile.Y == y)
                    {
                        tf.Add(new Tile("empty:65:65:10:Red", TileType.Normal, layer, "startTile"), x, y);
                    }
                    else
                    {
                        tf.Add(new Tile("empty:65:65:10:Magenta", TileType.Normal,layer), x, y);
                    }                    
                }
            }
            Add(tf);

            Item testItem = new TestItem("empty:65:65:10:Pink",1,layer+1);
            testItem.Position = new Vector2(GameEnvironment.Random.Next(0, columns - 1) * Tile.TileWidth, GameEnvironment.Random.Next(0, rows - 1) * Tile.TileHeight);
            Add(testItem);

            InventoryBox inventory = new InventoryBox(3,6,layer+1);
            inventory.Position = new Vector2((tf.Columns+1)*tf.CellWidth,0);
            Add(inventory);
        }
    }
}
