using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.Xna.Framework;

namespace Wink
{
    [Serializable]
    public partial class Level : GameObjectList, IGUIGameObject
    {
        private int levelIndex;
        string path;
        public int Index
        {
            get { return levelIndex; }
        }

        public Level(int levelIndex) : base(0, "Level")
        {
            path = "Content/Levels/" + levelIndex + ".txt";

            if(levelIndex % 3 == 0)//for now to repeat boss
            {
                path = "Content/Levels/3.txt";
            }

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

        #region Serialization
        public Level(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            levelIndex = info.GetInt32("levelIndex");
            path = info.GetString("path");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("levelIndex", levelIndex);
            info.AddValue("path", path);
        }
        #endregion

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
            //Putting the item one layer above the inventory box
            int inventoryLayer = layer + 1;
            int itemLayer = layer + 2;

            #region ENEMY CODE (test)
            for (int i = 0; i < 2; i++)
            {
                Enemy testEnemy = new Enemy(0,Index,EnemyType.random, "Enemy" + i);
                testEnemy.SetStats();
                //First find all passable tiles then select one at random.
                List<GameObject> tileCandidates = tf.FindAll(obj => obj is Tile && (obj as Tile).Passable);
                Tile startTile = tileCandidates[GameEnvironment.Random.Next(tileCandidates.Count)] as Tile;
                startTile.PutOnTile(testEnemy);
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
                    return LoadChestTile(levelIndex);
                case 'D':
                    return LoadDoorTile();
                case 'E':
                    return LoadEndTile();
                case 't':
                    return LoadTrapTile("spr_trap", TileType.Floor, x, y);
                case 'B':
                    return LoadBossTile();
                default:
                    return LoadWTFTile();
            }
        }

        private Tile LoadWallTile(int x, int y, string assetName = "test-wall-sprite2@10x5", string id = "")
        {
            TileField tf = Find("TileField") as TileField;
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

        private Tile LoadStartTile(int number, string assetName = "spr_floor")
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

        private Tile LoadTrapTile(string name, TileType tileType, int x, int y)
        {
            Tile t = new Trap("empty:64:64:10:Red");
            t.Passable = true;
            return t;
        }
        
        private Tile LoadChestTile(int floorNumber=1,string assetName = "spr_floor" )
        {
            floorNumber++;
            Tile t = LoadFloorTile("", assetName);
            Container chest = new Container("Sprites/Containers/chest2_closed", "Sprites/Containers/chest2_open", "Sounds/creaking-door-2", levelIndex);
            for (int x = 0; x < chest.IBox.Columns; x++)
            {
                for (int y = 0; y < chest.IBox.Rows; y++)
                {
                    int i = x % 4;
                    int spawnChance;
                    Item newItem;
                    switch (i)
                    {
                        #region cases
                        case 0:
                            spawnChance = 50;
                            newItem = new Potion(floorNumber);
                            break;
                        case 1:
                            spawnChance = 30;
                            newItem = new WeaponEquipment(floorNumber);
                            break;
                        case 2:
                            spawnChance = 30;
                            newItem = new BodyEquipment(floorNumber, 3);
                            break;
                        case 3:
                            spawnChance = 30;
                            newItem = new RingEquipment("empty:64:64:10:Gold");
                            break;
                        default:
                            throw new Exception("wtf");
                            #endregion
                    }
                    if (spawnChance > GameEnvironment.Random.Next(100))
                    {
                        ItemSlot cS = chest.IBox.Get(x, y) as ItemSlot;
                        cS.ChangeItem(newItem);
                    }
                }
            }
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

        private Tile LoadBossTile()
        {
            Tile t = LoadFloorTile();
            Boss boss = new Boss(levelIndex);
            t.PutOnTile(boss);
            return t;
        }

        public void InitGUI(Dictionary<string, object> guiState)
        {
            PlayingGUI pg = GameWorld.Find("PlayingGui") as PlayingGUI;
            SpriteGameObject floor = pg.Find("FloorBG") as SpriteGameObject;
            TextGameObject floorNumber = pg.Find("FloorNumber") as TextGameObject;
            floorNumber.Text = Index.ToString();
            floorNumber.Position = floor.Position + (floor.BoundingBox.Size.ToVector2() - floorNumber.Size) / 2;
        }

        public void CleanupGUI(Dictionary<string, object> guiState)
        {            
        }
    }
}
