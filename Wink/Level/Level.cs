using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;

namespace Wink
{
    [Serializable]
    public partial class Level : GameObjectList
    {
        private int levelIndex;
        public int Index
        {
            get { return levelIndex; }
        }

        public Level(int levelIndex) : base(0, "Level")
        {
            string path = "Content/Levels/" + levelIndex + ".txt";
            if (File.Exists(path))
            {
                LoadTiles(path);
            }
            else
            {
                List<Room> rooms = GenerateRooms();
                List<Tuple<Room, Room>> hallwayPairs = GenerateHallwayPairs(rooms);
                GenerateTiles(rooms, hallwayPairs);
            }

            this.levelIndex = levelIndex;
        }

        public Level(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

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

            for (int y = 0; y < textLines.Count; ++y) 
            {
                for (int x = 0; x < width; ++x)
                {
                    Tile t = LoadTile(textLines[y][x], x, y);
                    tf.Add(t, x, y);
                }
            }
            
            Item testItem = new TestItem("empty:64:64:10:Pink", 1, 0);
            List<Tile> floorTiles = tf.FindAll(obj => obj.GetType() == typeof(Tile) && (obj as Tile).TileType == TileType.Floor).Cast<Tile>().ToList();
            floorTiles[Random.Next(floorTiles.Count)].PutOnTile(testItem);

            #region ENEMY CODE (test)
            for (int i = 0; i < 2; i++)
            {
                Enemy testEnemy = new Enemy(0);

                //First find all passable tiles then select one at random.
                List<GameObject> tileCandidates = tf.FindAll(obj => obj is Tile && (obj as Tile).Passable);
                Tile startTile = tileCandidates[GameEnvironment.Random.Next(tileCandidates.Count)] as Tile;

                testEnemy.MoveTo(startTile);
            }
            #endregion

            tf.InitSpriteSheetIndexation();
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                case '.':
                    return new Tile();
                case '1':
                case '2':
                case '3':
                case '4':
                    return LoadStartTile(tileType - 48);
                case '#':
                    return LoadWallTile(x, y);
                case '-':
                    return LoadFloorTile();
                case 'c':
                    return LoadChestTile();
                case 'D':
                    return LoadDoorTile();
                case 'E':
                    return LoadEndTile();
                default:
                    return LoadWTFTile();
            }
        }

        private Tile LoadWallTile(int x, int y, string assetName = "test-wall-sprite2@10x5", string id = "")
        {
            TileField tf = this.Find("TileField") as TileField;
            Tile aboveTile = tf[x, y - 1] as Tile;

            Tile newTile;
            if (aboveTile != null && aboveTile.TileType == TileType.Floor)
            {
                newTile = new Tile("*" + assetName, TileType.Wall, 0, id);
            }
            else
            {
                newTile = new Tile(assetName, TileType.Wall, 0, id);
            }
            return newTile;
        }

        private Tile LoadFloorTile(string id = "", string assetName = "spr_floor")
        {
            Tile t = new Tile(assetName, TileType.Floor, 0, id);
            t.Passable = true;
            return t;
        }

        private Tile LoadStartTile(int number, string assetName = "empty:64:64:10:Red")
        {
            return LoadFloorTile("StartTile" + number, assetName);
        }

        private Tile LoadDoorTile(string assetName = "spr_floor")
        {
            Tile t = LoadFloorTile("", assetName);

            Door door = new Door(t);
            t.PutOnTile(door);
            return t;
        }

        private Tile LoadChestTile(string assetName = "spr_floor")
        {
            Tile t = LoadFloorTile("", assetName);

            Container chest = new Container("empty:64:64:10:Brown");
            t.PutOnTile(chest);
            return t;
        }

        private Tile LoadWTFTile()
        {
            Tile t = new Tile("empty:64:64:10:Black", TileType.Background);
            t.Passable = false;
            return t;
        }

        private Tile LoadEndTile()
        {
            Tile t = LoadFloorTile();
            End end = new End(t, levelIndex, this);
            t.PutOnTile(end);
            return t;
        }
    }
}
