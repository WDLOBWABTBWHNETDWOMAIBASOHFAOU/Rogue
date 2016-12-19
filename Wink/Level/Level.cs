using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Wink
{
    [Serializable]
    public class Level : GameObjectList
    {
        public Level(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Level(int levelIndex) : base(0, "Level")
        {
            LoadTiles("Content/Levels/" + levelIndex + ".txt");

            /*int rows= 10;
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
            Add(tf);*/
        }

        public void LoadTiles(string path)
        {
            List<string> textLines = new List<string>();
            StreamReader fileReader = new StreamReader(path);
            string line = fileReader.ReadLine();
            int width = line.Length;
            while (line != null)
            {
                textLines.Add(line);
                line = fileReader.ReadLine();
            }
            //timelimit = int.Parse(textLines[textLines.Count - 1]);
            TileField tf = new TileField(textLines.Count, width, 0, "TileField");


            Add(tf);
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < textLines.Count; ++y)
                {
                    Tile t = LoadTile(textLines[y][x], x, y);
                    tf.Add(t, x, y);
                }
            }

            //Putting the item one layer above the inventory box
            int inventoryLayer = layer + 1;
            int itemLayer = layer + 2;

            Item testItem = new TestItem("empty:65:65:10:Pink", 1, itemLayer); 
            // ENEMY CODE (test)
            Enemy testEnemy = new Enemy(this, layer + 1);
            Enemy testEnemy2 = new Enemy(this, layer + 1);
            //Add(testEnemy);
            // END ENEMY CODE (test)
            
            testItem.Position = new Vector2(GameEnvironment.Random.Next(0, tf.Columns - 1) * Tile.TileWidth, GameEnvironment.Random.Next(0, tf.Rows - 1) * Tile.TileHeight);
            Add(testItem);

            InventoryBox inventory = new InventoryBox(3, 6, inventoryLayer);
            inventory.Position = new Vector2((tf.Columns + 1) * tf.CellWidth, 0);
            Add(inventory);
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                case '.':
                    return new Tile();
                case '1':
                    return LoadStartTile();
                case '#':
                    return LoadWallTile("spr_wall", TileType.Wall);
                case '-':
                    return LoadFloorTile("spr_floor", TileType.Normal);
                default:
                    return LoadWTFTile();
                    //return new Tile("");
            }
        }

        private Tile LoadWallTile(string name, TileType tileType)
        {
            Tile t = new Tile("empty:65:65:10:Blue", tileType);
            t.Passable = false;
            return t;
        }

        private Tile LoadFloorTile(string name, TileType tileType)
        {
            Tile t = new Tile("empty:65:65:10:DarkGreen", tileType);
            t.Passable = true;
            return t;
        }

        private Tile LoadStartTile()
        {
            Tile t = new Tile("empty:65:65:10:Red", TileType.Normal, 0, "startTile");
            t.Passable = true;
            return t;
        }

        private Tile LoadWTFTile()
        {
            Tile t = new Tile("empty:65:65:10:Black", TileType.Wall);
            t.Passable = false;
            return t;
        }
    }
}
