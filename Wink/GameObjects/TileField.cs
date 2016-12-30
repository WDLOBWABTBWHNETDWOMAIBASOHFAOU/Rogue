using System;
using System.Runtime.Serialization;

namespace Wink
{
    /*
     * This class came from the TickTick Game.
     */
    [Serializable]
    public class TileField : GameObjectGrid
    {
        public TileField(int rows, int columns, int layer = 0, string id = "") : base(rows, columns, layer, id)
        {
            CellWidth = Tile.TileWidth;
            CellHeight = Tile.TileHeight;
        }

        public TileField(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TileType GetTileType(int x, int y)
        {
            if (x < 0 || x >= Columns)
            {
                return TileType.Floor;
            }
            if (y < 0 || y >= Rows)
            {
                return TileType.Background;
            }
            Tile current = Objects[x, y] as Tile;
            return current.TileType;
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
    }
}
