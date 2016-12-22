using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;

namespace Wink
{
    [Serializable]
    public partial class Level : GameObjectList
    {
        public Level(SerializationInfo info, StreamingContext context)
        {
            children = (List<GameObject>)info.GetValue("children", typeof(List<GameObject>));

            position = new Vector2((float)info.GetDouble("posX"), (float)info.GetDouble("posY"));
            velocity = new Vector2((float)info.GetDouble("velX"), (float)info.GetDouble("velY"));
            layer = info.GetInt32("layer");
            id = info.GetString("id");
            visible = info.GetBoolean("vis");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("children", children);

            //Reimplemented this her specifically to not have parent included, since this causes the GUI to be serialized as well.
            //info.AddValue("parent", parent);
            info.AddValue("posX", position.X);
            info.AddValue("posY", position.Y);
            info.AddValue("velX", velocity.X);
            info.AddValue("velY", velocity.Y);
            info.AddValue("layer", layer);
            info.AddValue("id", id);
            info.AddValue("vis", visible);
        }

        public Level(int levelIndex) : base(0, "Level")
        {
            LoadTiles("Content/Levels/" + levelIndex + ".txt");
        }

        public Level()
        {
            Generate();
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
            for(int i = 0; i < 2; i++)
            {
                Enemy testEnemy = new Enemy(layer + 1);
                Add(testEnemy);
                testEnemy.InitPosition();
            }
            // END ENEMY CODE (test)

            testItem.Position = new Vector2(GameEnvironment.Random.Next(0, tf.Columns - 1) * Tile.TileWidth, GameEnvironment.Random.Next(0, tf.Rows - 1) * Tile.TileHeight);
            Add(testItem);

            //Container chest = new Container("EmptyItem:65:65:10");
            


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
                case 'c':
                    return LoadChestTile("spr_ChestTile", TileType.Chest,x,y);
                case 'D':
                    return LoadDoorTile("spr_door", TileType.Door);
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
        private Tile LoadDoorTile(string name, TileType tileType)
        {
            Tile t = new Tile("empty:65:65:10:Orange", tileType);
            t.Passable = false;
            return t;
        }

        private Tile LoadWTFTile()
        {
            Tile t = new Tile("empty:65:65:10:Black", TileType.Wall);
            t.Passable = false;
            return t;
        }
        private Tile LoadChestTile(string name, TileType tileType,int x, int y)
        {
            Tile t = new Tile("empty:65:65:10:White", TileType.Chest);
            Container chest = new Container("empty:65:65:10:Brown");
            chest.Position = new Vector2(x * Tile.TileWidth, y * Tile.TileHeight);
            Add(chest);
            t.Passable = false;
            return t;
        }
    }
}
