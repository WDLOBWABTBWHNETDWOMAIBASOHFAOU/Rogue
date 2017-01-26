using System;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class End : SpriteGameObject, ITileObject
    {
        private Tile parentTile;
        private int levelIndex;
        private Level level;

        public Tile Tile
        {
            get { return parentTile; }
        }

        public Point PointInTile
        {
            get { return new Point(0, 0); }
        }

        public bool BlocksTile
        {
            get { return false; }
        }

        public End(Tile pTile, int levelIndex, Level level, string asset = "spr_finish", int layer = 0, string id = "") : base(asset, layer, id)
        {
            parentTile = pTile;
            this.levelIndex = levelIndex;
            this.level = level;
        }

        #region Serialization
        public End(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            parentTile = info.TryGUIDThenFull<Tile>(context, "parentTile");
            
            levelIndex = info.GetInt32("levelIndex");
            level = info.GetValue("level", typeof(Level)) as Level;
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
            info.AddValue("levelIndex", levelIndex);
            info.AddValue("level", level);
            base.GetObjectData(info, context);
        }
        #endregion Serialization

        public override void Replace(GameObject replacement)
        {
            if (parentTile != null && parentTile.GUID == replacement.GUID)
                parentTile = replacement as Tile;

            base.Replace(replacement);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
                {
                    Player player = GameWorld.Find(p => p.Id == Player.LocalPlayerName) as Player;

                    Event e = new NextLevelEvent(this, player);
                    Server.Send(e);
                };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            base.HandleInput(inputHelper);
        }
    }
}
