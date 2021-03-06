﻿using System;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Wink
{
    [Serializable]
    public class Door : SpriteGameObject, ITileObject
    {
        private Tile parentTile;
        private bool open;

        public Tile Tile
        {
            get { return parentTile; }
        }

        public Door(Tile pTile, string assetName = "spr_door@2x1", int layer = 0, string id = "") : base(assetName, layer, id)
        {
            parentTile = pTile;
            visible = true;
            layer = 1;
        }

        #region Serialization
        public Door(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            parentTile = info.TryGUIDThenFull<Tile>(context, "parentTile");
            open = info.GetBoolean("open");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationHelper.Variables v = context.GetVars();
            if (v.FullySerializeEverything || v.FullySerialized.Contains(parentTile.GUID))
            {
                info.AddValue("parentTile", parentTile);
            }
            else
            {
                info.AddValue("parentTileGUID", parentTile.GUID.ToString()); 
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

        public void Open()
        {
            open = true;
            SpriteSheetIndex = 1;
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
                    Player player = GameWorld.Find(p => p.Id == Player.LocalPlayerName) as Player;
                    Server.Send(new DoorEvent(this, player));
                };

                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

                base.HandleInput(inputHelper);
            }
        }
    }
}
