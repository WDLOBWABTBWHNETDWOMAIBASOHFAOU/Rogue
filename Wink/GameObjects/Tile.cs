using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public enum TileType
    {
        Background,
        Normal,
        Wall
    }

    [Serializable]
    public class Tile : SpriteGameObject, ClickableGameObject
    {
        public const int TileWidth = 65;
        public const int TileHeight = 65;

        protected TileType type;

        public Point TilePosition { get { return new Point((int)Position.X / TileWidth, (int)Position.Y / TileHeight); } }

        public Tile(string assetname = "", TileType tp = TileType.Background, int layer = 0, string id = "") : base(assetname, layer, id)
        {
            type = tp;
        }

        public Tile(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            type = (TileType)info.GetValue("type", typeof(TileType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", type);
            base.GetObjectData(info, context);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (type == TileType.Background)
            {
                return;
            }
            base.Draw(gameTime, spriteBatch, camera);
        }

        public TileType TileType
        {
            get { return type; }
        }

        public virtual void OnClick(Server server)
        {
            PlayerMoveEvent pme = new PlayerMoveEvent();
            pme.Player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
            pme.Tile = this;
            server.Send(pme);
        }
    }
}
