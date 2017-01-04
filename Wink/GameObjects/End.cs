using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class End : SpriteGameObject, ITileObject
    {
        Tile parentTile;
        int levelIndex;
        Level level;

        public Point PointInTile
        {
            get { return new Point(0, 0); }
        }

        public bool BlocksTile
        {
            get { return false; }
        }

        public End(Tile pTile, int levelIndex, Level level, string asset = "empty:64:64:10:Yellow", int layer = 0, string id = "") : base(asset, layer, id)
        {
            parentTile = pTile;
            this.levelIndex = levelIndex;
            this.level = level;
        }

        public End(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            parentTile = info.GetValue("parentTile", typeof(Tile)) as Tile;
            levelIndex = info.GetInt32("levelIndex");
            level = info.GetValue("level", typeof(Level)) as Level;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("parentTile", parentTile);
            info.AddValue("levelIndex", levelIndex);
            info.AddValue("level", level);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
                {
                    // correct player when in multiplayer?
                    Player player = GameWorld.Find(p => p is Player) as Player;

                    int dx = (int)Math.Abs(player.Position.X - player.Origin.X - Position.X);
                    int dy = (int)Math.Abs(player.Position.Y - player.Origin.Y - Position.Y);

                    if (dx <= Tile.TileWidth && dy <= Tile.TileHeight)
                    {
                        Event e = new NextLevelEvent();
                        Server.Send(e);
                    }
                };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            base.HandleInput(inputHelper);
        }
    }
}
