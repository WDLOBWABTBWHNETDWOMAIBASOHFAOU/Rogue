using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    abstract class Living : AnimatedGameObject
    {
        public Living(int layer = 0, string id = "", float scale = 1.0f) : base(layer, id, scale)
        {

        }
        
        public void MoveTo(Tile tile)
        {
            position.X = (tile.TilePosition.X + 1) * Tile.TileWidth - 0.5f * Tile.TileWidth;
            position.Y = (tile.TilePosition.Y + 1) * Tile.TileHeight;
        }
    }
}
