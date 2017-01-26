using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    public class Statsscreenbox : GameObjectList
    {
        public GameObjectGrid ItemGrid2
        {
            get { return Find(obj => obj is GameObjectGrid) as GameObjectGrid; }
        }

        public Statsscreenbox(GameObjectGrid itemGrid2, int layer = 0, string id="", float cameraSensitivity = 0) : base(layer, id)
        {
            itemGrid2.CellHeight = Tile.TileHeight;
            itemGrid2.CellWidth = Tile.TileWidth;


            for (int x = 0; x <   itemGrid2.Columns; x++)
            {
                for (int y = 0; y < itemGrid2.Rows ; y++)
                {
                    ItemGrid2.Add(new ItemSlot(), x, y);
                }
            }
            Add(itemGrid2);
        }

        #region Serialization
        public Statsscreenbox(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion
    }


}
