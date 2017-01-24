using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using P = Wink.TileField.Permission;

namespace Wink
{
    /*
     * This class came from the TickTick Game.
     */
    [Serializable]
    public class TileField : GameObjectGrid, ICellGrid
    {
        public int xDim
        {
            get { return Columns; }
        }
        public int yDim
        {
            get { return Rows; }
        }

        public TileField(int rows, int columns, int layer = 0, string id = "") : base(rows, columns, layer, id)
        {
            CellWidth = Tile.TileWidth;
            CellHeight = Tile.TileHeight;
        }

        #region Serialization
        public TileField(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion

        public List<Tile> AllTilesWithObjects
        {
            get
            {
                List<Tile> result = new List<Tile>();
                foreach (GameObject go in Objects)
                {
                    Tile t = go as Tile;
                    if (t.OnTile.Children.Count > 0)
                        result.Add(t);
                }
                return result;
            }
        }

        public override string ToString()
        {
            char[] char1 = new char[(Columns + 1) * Rows];
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    TileType tt = (Get(x, y) as Tile).TileType;
                    char1[y * (Columns + 1) + x] = tt == TileType.Wall ? '#' : tt == TileType.Floor ? '.' : ' ';
                }
                char1[y * (Columns + 1) + Columns] = '\n';
            }
            return new string(char1);
        }

        private static readonly HashSet<TileType> blobTextures = new HashSet<TileType> { TileType.Wall };

        public void InitSpriteSheetIndexation()
        {
            for (int tfX = 0; tfX < Columns; tfX++)
            {
                for (int tfY = 0; tfY < Rows; tfY++)
                {
                    Tile currentTile = this[tfX, tfY] as Tile;
                    if (!blobTextures.Contains(currentTile.TileType))
                        continue;

                    bool[,] boolArray = new bool[3, 3];
                    for (int boolX = -1; boolX <= 1; boolX++)
                    {
                        for (int boolY = -1; boolY <= 1; boolY++)
                        {
                            Tile adjacentTile = this[tfX + boolX, tfY + boolY] as Tile;
                            boolArray[boolX + 1, boolY + 1] = adjacentTile != null ? adjacentTile.TileType == currentTile.TileType : false;
                        }
                    }

                    foreach (KeyValuePair<Permission[,], int> kvp in table)
                    {
                        bool compatible = false;
                        for (int x = 0; x < 3; x++)
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                Permission p = kvp.Key[x, y];
                                bool bothFalse = p == Permission.NotEqual && boolArray[x, y] == false;
                                bool bothTrue = p == Permission.Equal && boolArray[x, y] == true;
                                compatible = p == Permission.Either || bothFalse || bothTrue;
                                if (!compatible)
                                    break;
                            }
                            if (!compatible)
                                break;
                        }
                        if (compatible)
                        {
                            currentTile.Sprite.SheetIndex = kvp.Value;
                            break;
                        }
                    }
                }
            }
        }
        
        public bool IsWall(int x, int y)
        {
            Tile t = grid[x, y] as Tile;
            if (t.Blocked)
                return true;
            return t.TileType == TileType.Wall; //TODO: make separate property in Tile that describes whether or not it obstructs line of sight. (!Passable as placeholder) 
        }

        public void SetLight(int x, int y, float distanceSquared, IViewer seenBy)
        {
            Tile tile = grid[x, y] as Tile;
            tile.IsSeenBy(seenBy as Living, (float)Math.Sqrt(distanceSquared));
        }

        #region Table that maps 3x3 Permission arrays to an index.
        public enum Permission { Either, NotEqual, Equal };

        //Added a Type Alias to make more readable
        private Dictionary<Permission[,], int> table = new Dictionary<Permission[,], int>()
        {
            { new Permission[3, 3] {
                { P.Either,   P.NotEqual,   P.Either },
                { P.NotEqual, P.Either,     P.Equal  },
                { P.Either,   P.NotEqual,   P.Either }
            }, 0 },

            { new Permission[3, 3] {
                { P.Either,   P.NotEqual, P.Either },
                { P.NotEqual, P.Either,   P.Equal  },
                { P.Either,   P.Equal,    P.Equal  }
            }, 1 },
            { new Permission[3, 3] {
                { P.Either,   P.Equal, P.Equal },
                { P.NotEqual,    P.Either,   P.Equal  },
                { P.Either,    P.Equal,    P.Equal  }
            }, 2 },
            { new Permission[3, 3] {
                { P.Either,   P.Equal, P.Equal },
                { P.NotEqual,    P.Either,   P.Equal  },
                { P.Either,    P.NotEqual,    P.Either  }
            }, 3 },

            { new Permission[3, 3] {
                { P.Either,   P.NotEqual, P.Either },
                { P.NotEqual, P.Either,   P.Equal  },
                { P.Either,   P.Equal,    P.NotEqual  }
            }, 4 },
            { new Permission[3, 3] {
                { P.Either,   P.Equal, P.NotEqual },
                { P.NotEqual,    P.Either,   P.Equal  },
                { P.Either, P.Equal,    P.NotEqual  }
            }, 5 },
            { new Permission[3, 3] {
                { P.Either,   P.Equal, P.NotEqual },
                { P.NotEqual,    P.Either,   P.Equal  },
                { P.Either, P.NotEqual,    P.Either  }
            }, 6 },

            { new Permission[3, 3] {
                { P.Equal,    P.Equal,     P.Equal },
                { P.Equal,    P.Either,    P.Equal  },
                { P.Equal,    P.Equal,     P.NotEqual  }
            }, 7 },
            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Equal, P.Equal,    P.NotEqual  }
            }, 8 },
            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Equal, P.Equal,    P.Equal  }
            }, 9 },

            //SECOND ROW
            { new Permission[3, 3] {
                { P.Either,   P.NotEqual,    P.Either },
                { P.Equal, P.Either,   P.Equal  },
                { P.Either,   P.NotEqual,    P.Either }
            }, 10 },

            { new Permission[3, 3] {
                { P.Either,   P.NotEqual,    P.Either },
                { P.Equal, P.Either,   P.Equal  },
                { P.Equal,   P.Equal,    P.Equal }
            }, 11 },
            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.Equal },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Equal,    P.Equal,    P.Equal }
            }, 12 },
            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.Equal },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Either,    P.NotEqual,    P.Either }
            }, 13 },

            { new Permission[3, 3] {
                { P.Either,   P.NotEqual,    P.Either },
                { P.Equal, P.Either,   P.Equal  },
                { P.NotEqual,   P.Equal,    P.NotEqual }
            }, 14 },
            { new Permission[3, 3] {
                { P.Either,   P.NotEqual, P.Either },
                { P.NotEqual, P.Either,   P.NotEqual  },
                { P.Either,   P.NotEqual, P.Either }
            }, 15 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Either, P.NotEqual,    P.Either }
            }, 16 },

            { new Permission[3, 3] {
                { P.Equal,    P.Equal,     P.Equal },
                { P.Equal,    P.Either,    P.Equal  },
                { P.NotEqual,    P.Equal,     P.NotEqual  }
            }, 17 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.NotEqual, P.Equal,    P.NotEqual  }
            }, 18 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Equal, P.Equal,    P.Equal  }
            }, 19 },

            //THIRD ROW
            { new Permission[3, 3] {
                { P.Either,   P.NotEqual,    P.Either },
                { P.Equal, P.Either,   P.NotEqual  },
                { P.Either,   P.NotEqual, P.Either }
            }, 20 },

            { new Permission[3, 3] {
                { P.Either,   P.NotEqual,    P.Either },
                { P.Equal, P.Either,   P.NotEqual  },
                { P.Equal,   P.Equal, P.Either }
            }, 21 },
            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.Either },
                { P.Equal,    P.Either,   P.NotEqual  },
                { P.Equal,   P.Equal, P.Either }
            }, 22 },
            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.Either },
                { P.Equal,    P.Either,   P.NotEqual  },
                { P.Either,   P.NotEqual, P.Either }
            }, 23 },

            { new Permission[3, 3] {
                { P.Either,   P.NotEqual,    P.Either },
                { P.Equal, P.Either,   P.NotEqual  },
                { P.NotEqual,   P.Equal, P.Either }
            }, 24 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.Either },
                { P.Equal,    P.Either,   P.NotEqual  },
                { P.NotEqual,   P.Equal, P.Either }
            }, 25 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.Either },
                { P.Equal,    P.Either,   P.NotEqual  },
                { P.Either,   P.NotEqual, P.Either }
            }, 26 },

            { new Permission[3, 3] {
                { P.Equal,    P.Equal,     P.Equal },
                { P.Equal,    P.Either,    P.Equal  },
                { P.NotEqual,    P.Equal,     P.Equal  }
            }, 27 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.Equal },
                { P.Equal,    P.Either,   P.Equal  },
                { P.NotEqual,    P.Equal,    P.Equal  }
            }, 28 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.Equal },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Equal,    P.Equal,    P.Equal  }
            }, 29 },

            //FOURTH ROW
            { new Permission[3, 3] {
                { P.Either,   P.NotEqual, P.Either },
                { P.NotEqual, P.Either,   P.NotEqual  },
                { P.Either,   P.Equal, P.Either }
            }, 31 },
            { new Permission[3, 3] {
                { P.Either,   P.Equal, P.Either },
                { P.NotEqual,    P.Either,   P.NotEqual  },
                { P.Either,   P.Equal, P.Either }
            }, 32 },
            { new Permission[3, 3] {
                { P.Either,   P.Equal, P.Either },
                { P.NotEqual,    P.Either,   P.NotEqual  },
                { P.Either,   P.NotEqual, P.Either }
            }, 33 },

            { new Permission[3, 3] {
                { P.Either,   P.Equal, P.Equal },
                { P.NotEqual,    P.Either,   P.Equal  },
                { P.Either,    P.Equal,    P.NotEqual }
            }, 34 },
            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Either, P.NotEqual,    P.Either }
            }, 35 },
            { new Permission[3, 3] {
                { P.Either,   P.NotEqual,    P.Either },
                { P.Equal, P.Either,   P.Equal  },
                { P.Equal,   P.Equal,    P.NotEqual }
            }, 36 },

            { new Permission[3, 3] {
                { P.Either,   P.Equal, P.NotEqual },
                { P.NotEqual,    P.Either,   P.Equal  },
                { P.Either, P.Equal,    P.Equal }
            }, 37 },
            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.NotEqual, P.Equal,    P.NotEqual }
            }, 38 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Equal, P.Equal,    P.NotEqual }
            }, 39 },

            //FIFTH ROW
            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.NotEqual, P.Equal,    P.Equal }
            }, 42 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.Equal },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Equal,    P.Equal,    P.NotEqual }
            }, 43 },
            { new Permission[3, 3] {
                { P.Either,   P.NotEqual,    P.Either },
                { P.Equal, P.Either,   P.Equal  },
                { P.NotEqual,   P.Equal,    P.Equal }
            }, 44 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.Either },
                { P.Equal,    P.Either,   P.NotEqual  },
                { P.Equal,   P.Equal, P.Either }
            }, 45 },

            { new Permission[3, 3] {
                { P.Equal,    P.Equal,    P.Either },
                { P.Equal,    P.Either,   P.NotEqual  },
                { P.NotEqual,   P.Equal, P.Either }
            }, 46 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.Equal },
                { P.Equal,    P.Either,   P.Equal  },
                { P.Either,    P.NotEqual,    P.Either }
            }, 47 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.Equal },
                { P.Equal,    P.Either,   P.Equal  },
                { P.NotEqual,    P.Equal,    P.NotEqual }
            }, 48 },
            { new Permission[3, 3] {
                { P.NotEqual, P.Equal,    P.NotEqual },
                { P.Equal,    P.Either,   P.Equal  },
                { P.NotEqual, P.Equal,    P.Equal }
            }, 49 },
        };
        #endregion
    }
}
