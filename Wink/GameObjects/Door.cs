using System;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class Door : SpriteGameObject, ITileObject
    {
        private Tile parentTile;
        private bool open;

        public Door(Tile pTile, string assetName = "spr_door@2x1", int layer = 0, string id = "") : base(assetName, layer, id)
        {
            parentTile = pTile;
            visible = true;
            layer = 1;
        }

        public Door(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            parentTile = info.GetValue("parentTile", typeof(Tile)) as Tile;
            open = info.GetBoolean("open");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("parentTile", parentTile);
            info.AddValue("open", open);
        }

        public Point PointInTile
        {
            get { return new Point(0, -32 - 18); }
        }

        public bool BlocksTile
        {
            get { return !open; }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (!open)
            {
                Action onClick = () =>
                {
                    //TODO: Replace this with Event.
                    Player player = GameWorld.Find(p => p is Player) as Player;

                    int dx = (int)Math.Abs(player.Tile.Position.X - parentTile.Position.X);
                    int dy = (int)Math.Abs(player.Tile.Position.Y - parentTile.Position.Y);
                    if (dx <= Tile.TileWidth && dy <= Tile.TileHeight)
                    {
                        SpriteSheetIndex = 1;
                        open = true;
                    }
                };

                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

                base.HandleInput(inputHelper);
            }
        }
    }
}
