using System;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class Door : SpriteGameObject, ITileObject
    {
        private Tile parentTile;
        public bool open;

        public Tile ParentTile { get { return parentTile; } }

        public Door(Tile pTile, string assetName = "spr_door@2x1", int layer = 0, string id = "") : base(assetName, layer, id)
        {
            parentTile = pTile;
            visible = true;
            layer = 1;
        }

        #region Serialization
        public Door(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                parentTile = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("parentTileGUID"))) as Tile; 
            }
            else
            {
                parentTile = info.GetValue("parentTile", typeof(Tile)) as Tile;
            }
            open = info.GetBoolean("open");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                info.AddValue("parentTileGUID", parentTile.GUID.ToString()); 
            }
            else
            {
                info.AddValue("parentTile", parentTile);
            }
            info.AddValue("open", open);
            base.GetObjectData(info, context);
        }
        #endregion

        public override void Replace(GameObject replacement)
        {
            if (parentTile != null && parentTile.GUID == replacement.GUID)
                parentTile = replacement as Tile;

            base.Replace(replacement);
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
                Action onLeftClick = () =>//left click to open a closed door
                {
                    Player player = GameWorld.Find(p => p.Id == Player.LocalPlayerName) as Player;                    
                    OpenDoorEvent oDe = new OpenDoorEvent(player, this);
                    oDe.door = this;
                    Server.Send(oDe);
                };

                inputHelper.IfMouseLeftButtonPressedOn(this, onLeftClick);

                base.HandleInput(inputHelper);
            }
            else
            {
                Action onRightClick = () =>//right click to close a open door
                {
                    Player player = GameWorld.Find(p => p.Id == Player.LocalPlayerName) as Player;
                    OpenDoorEvent oDe = new OpenDoorEvent(player, this);
                    oDe.door = this;
                    Server.Send(oDe);
                };

                inputHelper.IfMouseRightButtonPressedOn(this, onRightClick);

                base.HandleInput(inputHelper);
            }
        }
    }
}
